using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Ghost_movements : Grid_character {

    /* *
     * Cette classe hérite de Grid_character et contient l'ensemble des nouveaux attributs et méthodes 
     * permettant de contrôler le comportement fantomes. Il y a a beaucoup de méthodes différentes pour simplifier
     * un maximum la compréhension du code et s'abstraire le plus possible de la grille, Tilemap ou calculs de coordonnées.
     * 
     * Les scripts contrôlant les fantomes hérite chacun de cette classe. Voici un résumé du comportement des fantomes, fidèle au jeu Pacman original.
     * Les Fantomes ont 3 Etats :
     * - Le mode Chase, où le fantome poursuit Pacman suivant son propre algorithme, et qui dure 20s, après quoi il passe en mode Scatter.
     * - Le mode Scatter, où chaque fantôme va aller tourner dans un angle de la map qui lui est attribué, il dure 7s et après on passe en mode Chase à nouveau.
     * - Le mode Frightened, où le fantome est "effrayé", il est ralenti, avance aléatoirement, et peut se faire manger par Pacman. Il dure 7s. On repasse en mode Chase ensuite.
     * 
     * Quand un fantome est mangé, il est téléporté à son spawn et est à l'état "Mort" pendant 3s, où il tourne dans son spawn. Puis repasse en mode Chase.
     * 
     * L'Etat des fantomes est contrôlé par le GameManager, qui possède un timer et qui va changer l'état de chaque fantome dès que celui-ci atteint 0.
     * 
     * */

    //Informations Pacman
    protected Vector3 PacmanPos;
    protected Vector3Int PacmanPosCell;
    protected Transform PacTransform;

    //Constantes
    public const float DUREE_MORT = 3.0f;
    public const float REDUCTION_VITESSE = 4.0f;
    public const float VITESSE_INITIAL = 6.0f;

    //La case autour de laquelle le fantome va tourner en mode Scatter, elle dépend du fantome.
    public Vector3Int ScatterPos;

    protected int state;
    /*La variable qui contient l'état actuel du fantome :
     * 0 = Mort
     * 1 = Chase
     * 2 = Scatter
     * 3 = Frightened */
     

    protected float timeFrightened; //Timer pour l'état de mort des fantomes, car cette état est propre à chaque fantome et non commun.
    protected bool dejaMort; //Permet de savoir si un Fantome viens tout juste de mourir

    //Attributs en rapport avec le Sprite du fantome
    protected SpriteRenderer ghost_SpriteR; //Permet de manipuler le sprite du fantome
    protected Sprite GhostNormal; //Contient son sprite habituelle
    public Sprite GhostAfraid; //Contient son sprite effrayé.

    //Attributs contenant le son actuel du fantome.
        /* Notes : l'audio est géré par l'AudioManager, qui contient dans un array tout les composants AudioSource
         * pour faciliter certaines actions tel que pour couper le son de tout le jeu.
         * Ainsi, l'AudioSource de chaque object du jeu récupérera son composant depuis l'AudioManager, qui est
         * un singleton en public.
         * */

    protected AudioSource fantome_audio;
    /* Le clip audio de l'AudioSource va changer en fonction de l'état du fantome, ses deux
     * variables vont stocker le clip pour pouvoir le remettre ensuite */

    //Protégé car propre à chaque fantome
    protected AudioClip fantome_sound;
    //Public car le même pour chaque fantome.
    public AudioClip frightened_sound; 



    // --------------- METHODES -------------

    /*  Déplacement d'un fantome :
     *      Le déplacement d'un fantome se fait comme dans le jeu original :
     *      - Un fantome ne peut pas faire demi-tour, sauf si il change de mode entre Scatter et Chase ( C'est l'indicateur visuel permettant de savoir que le mode a changé )
     *      - Un fantome avance tout droit si il est dans un couloir.
     *      - Si le fantome a atteint un croisement, il choisit d'aller dans la direction où la première case après le croisement est la plus proche de la case cible du fantome.
     *              - La case cible du fantome dépend de l'algorithme de poursuite du fantome, si il est en mode Scatter, c'est un angle de la map, si
     *              il est en mode Chase, ça dépend du fantome. Par exemple Blinky le fantome rouge a toujours la position de Pacman en cible.
     *              
     *     On fera remarquer qu'avec ce mode de déplacement, le fantome cherchera à se rapprocher comme il peut de sa case cible en fonction des restrictions qui lui sont imposés :
     *     C'est à dire que ce n'est pas grave si la case cible est un mur ou hors du plateau, car il essayera de prendre le chemin qui lui rapproche le plus de celle-ci, et ne se dirige pas
     *     directement vers celle-ci.
     * */


    //Fais avancer le fantome dans le labyrinthe en direction de la case cible, en prenant en compte les murs et
    //choississant une direction à chaque croisement le rapprochant de sa case cible.
    public void allerVers(Vector3 targetPosition)
    {
        /* La cible est la case adjacente 
         * 
         * */
        //Si il n'est pas sur sa cible, avance vers elle.
        if (transform.position != targetPos)
            moveToCell(targetPos);
        //Si il est sur sa cible, il change de cible.
        else
        {
            if (estCroisement(Cell))
                targetPos = caseAdjLaPlusProche(targetPosition);
            //Si il n'est pas à un croisement, il ne peut qu'avancer, donc il avance.
            else
                targetPos = caseDevant();
            updateDirection(targetPos);
        }


    }
    /* 
     * Renvoie la position de la cellule adjacente à la cellule actuelle la plus proche de la cellule cible.
     * On l'utilise dès qu'on arrive à un croisement.
     */
    public Vector3 caseAdjLaPlusProche(Vector3 targetPos)
    {
        List<Vector3> listC = new List<Vector3>();

        //On ajoute chaque cellule adjacente qui n'est pas un mur ou dans la direction opposé au fantomes à la liste des choix possibles.
        //(Un fantome ne fait pas de demi-tours)
        if ( !isWall(downCell()) && direction != "Up" && nestPasEntreeSpawn(downCell()))
            listC.Add( downCell() );
        if ( !isWall(rightCell()) && direction != "Left")
            listC.Add( rightCell() );
        if ( !isWall(topCell()) && direction != "Down")
            listC.Add(topCell());
        if ( !isWall(leftCell()) && direction != "Right")
            listC.Add(leftCell());

        //Ensuite on renvoie la cellule qui est la plus proche de la cible.
        /* Comme on appelle cette fonction uniquement à un croisement, il y a obligatoirement 1, 2 ou 3 valeurs
         */
        if (listC.Count == 1) //Cas virage.
            return listC[0];
        else if (listC.Count == 2)
            return plusProche(listC[0], listC[1], targetPos);
        else
            return plusProche(listC[0], plusProche(listC[1], listC[2], targetPos), targetPos);

        /* Précision : En cas d'égalité, il y a une priorité Up > Left > Down, qui
         * est ici géré par l'ordre dans lequel on a ajouté les case + l'implémentation de plusProche.
         */

    }

    //Renvoie la case en face de la case actuelle, en utilisant la direction du fantome.
    public Vector3 caseDevant()
    {
        switch(direction)
        {
            case "Right":
                return rightCell();
            case "Left":
                return leftCell();
            case "Down":
                return downCell();
            case "Up":
                return topCell();
            default:
                Debug.LogWarning("Cas impossible !");
                return topCell(); //Placeholder, cas impossible normalement, à changer par la case d'angle du fantome.
        }
    }

    //Le ghost fait immédiatement demi-tour, est uniquement utilisé lors du changement d'état entre Chase et Scatter
    //
    public void faireDemiTour()
    {
        //Inverse la direction, et change immédiatement sa case cible par celle derrière lui.
        direction = oppositeDirection(direction);
        targetPos = caseAdj(direction);
    }

    //Utilisé lorsque le fantome est en mode "Frightened", à chaque croisement il choisit une direction aléatoire.
    public Vector3 caseAdjAleatoire()
    {
        List<Vector3> listC = new List<Vector3>();

        //On ajoute chaque cellule adjacente qui n'est pas un mur ou dans la direction opposé au fantomes à la liste des choix possibles.
        //(Un fantome ne fait pas de demi-tours)
        if (!isWall(downCell()) && direction != "Up" && nestPasEntreeSpawn(downCell()))
            listC.Add(downCell());
        if (!isWall(rightCell()) && direction != "Left")
            listC.Add(rightCell());
        if (!isWall(topCell()) && direction != "Down" && nestPasEntreeSpawn(topCell()) )
            listC.Add(topCell());
        if (!isWall(leftCell()) && direction != "Right")
            listC.Add(leftCell());

        //On choisit aléatoirement une case parmi celles possibles :
        if (listC.Count == 1)
            return listC[0];
        return listC[UnityEngine.Random.Range(0, listC.Count - 1)];
    }

    //
    public void volumeEnFonctionDeDistance()
    {
        float distAvecPacman = dist(Cell, PacmanPos);
        if (distAvecPacman >= 20f)
            fantome_audio.volume = 0;
        else
        {
            fantome_audio.volume = 1 - 0.3f*(distAvecPacman / 20f);
        }
    }



    // ----- Methodes principales de comportement et de déplacements des fantomes.

    /* Fonction du mode Chase et Scatter des Fantomes.
     * Le Fantome va se diriger vers la position donnée en cible, soit on donne la position de la cible à poursuivre pour Chase,
     * soit la position ScatterPos pour se diriger dans l'angle comme en mode Scatter
     * */
    public void ChaseAndScatter(Vector3 cible)
    {
        
        Debug.DrawLine(transform.position, cible, Color.red);

        if (estDansSpawn())
            sortirSpawn();
        else
            allerVers(cible);

    }

    //Quand le fantome est effrayé, il se déplace aléatoirement.
    public void Frightened()
    {
        allerVers(caseAdjAleatoire());
    }

    //Dirige le comportement du fantome lorsqu'il est mort. Si il n'étais pas mort, lance un chrono de DUREE_MORT secondes
    //pendants lesquels le fantome se déplacera aléatoirement dans l'enclos avant de reprendre son état normal.
    public void Dead()
    {

        if (!dejaMort)
        {
            dejaMort = true;
            timeFrightened = DUREE_MORT;
        }

        timeFrightened -= Time.deltaTime;

        allerVers(caseAdjAleatoire());
        if (timeFrightened < 0.0f)
        {
            dejaMort = false;
            state = 1;
            nestPlusEffraye();
        }
    }

    //Fais diriger le fantome vers la sortie de son spawn. Utile car la fonction de base caseAdjLaPlusProche() a du mal a
    //fonctionner correctement lorsque le fantome n'est pas dans un couloir.
    public void sortirSpawn()
    {
        //Il y a deux cases vers lesquels se diriger pour sortir du spawn, le fantome se dirige vers
        //la plus proche d'entre elle
        //Vector3 cellCible;

        if (transform.position != targetPos)
            moveToCell(targetPos);
        else
        {
            if (Cell.x == -1 || Cell.x == 0)
                targetPos = topCell();
            else if (Cell.x < 0)
                targetPos = rightCell();
            else
                targetPos = leftCell();

            updateDirection(targetPos);
        }

    }


    // ---------- Methode de changement d'état ---------

    //Réduit la vitesse du fantome et affiche son sprite Frightened. 
    //Utilisé par le gameManager pour éviter d'avoir à se servir de multiple get/set.
    public void deviensEffraye()
    {
        ghost_SpriteR.sprite = GhostAfraid;
        speed -= REDUCTION_VITESSE;

        //On change son clip sonore
        fantome_audio.Stop(); //On arrete son bruit actuel
        fantome_audio.clip = frightened_sound; //On le replace par le bruit effrayé
        fantome_audio.Play(); //On joue le nouveau son
    }
    //Remet le fantome a sa vitesse initial et re-affiche son sprite normal.
    public void nestPlusEffraye()
    {
        ghost_SpriteR.sprite = GhostNormal; 
        speed = VITESSE_INITIAL;

        //On change son clip sonore
        fantome_audio.Stop(); //On arrete son bruit actuel
        fantome_audio.clip = fantome_sound; //On le replace par le bruit effrayé
        fantome_audio.Play(); //On joue le nouveau son
    }    
    
    //Le Fantome meurt : On le mets à l'état 0 = mort, et on le renvoie dans l'enclos du spawn.
    //Appellé lorsqu'il y a collision avec Pacman et un fantome effrayé.
    public void death()
    {
        state = 0;
        transform.position = Vector3.zero;
        targetPos = transform.position;

        //On coupe son son
        fantome_audio.Stop();
        //On joue le bruit de Pacman qui mange un fantome.
        AudioManager.getInstance().Find("Ghost_Eaten").source.Play();
    }



    // --------- Rafraichissement-------------
    //Mets à jour les variables avec les informations sur les coordonnées actuelle de Pacman, pour les algorithme de poursuite.
    public void updatePacPos()
    {
        PacmanPosCell = tilemap.WorldToCell(PacTransform.position);
        PacmanPos = tilemap.GetCellCenterWorld(PacmanPosCell);
    }

    //Met à jour la variable "direction" du fantome en fonction de sa case actuelle et de la case cible.
    public void updateDirection(Vector3 targetPos)
    {
        Vector3 cellCentre = tilemap.GetCellCenterWorld(Cell);
        if (cellCentre == targetPos)
            return;
        if (cellCentre.x < targetPos.x)
            direction = "Right";
        else if (cellCentre.x > targetPos.x)
            direction = "Left";
        else if (cellCentre.y < targetPos.y)
            direction = "Up";
        else if (cellCentre.y > targetPos.y)
            direction = "Down";
    }

    // -------------Booléens  -----------------

    //Renvoie vrai si la case en argument est un croisement (a 3 cases vide adjacentes ou plus), on compte l'entrée du spawn comme un mur.
    public bool estCroisement(Vector3Int cell)
    {
        int nb_caseAdj = 0;

        if (!isWall(rightCell()))
            nb_caseAdj++;
        if (!isWall(leftCell()))
            nb_caseAdj++;
        if (!isWall(downCell()) && nestPasEntreeSpawn(downCell()))
            nb_caseAdj++;
        if (!isWall(topCell()))
            nb_caseAdj++;

        if (nb_caseAdj >= 2)
            return true;
        return false;
    }

    //Renvoie vrai si le fantome est dans l'enclos du spawn
    public bool estDansSpawn()
    {
        return (Cell.x <= 3 && Cell.x >= -3 && Cell.y <= 1 && Cell.y >= -3);

        //Information utiles :
        /* Position des angles du spawn
        Vector3Int TopLeft = new Vector3Int(-4, 1, 0);
        Vector3Int TopRight = new Vector3Int(3, 1, 0);
        Vector3Int BottomRight = new Vector3Int(3, -3, 0);
        Vector3Int BottomLeft = new Vector3Int(-4, -3, 0);        
        //Coordonnées case en face sorties
        //Left = (-1,2,0)
        //Right = (0,2,0)

           /* Les angles de la map
     * 
     * BottomRight : (14, -16, 0) fPos (13, -14, 0) Cell
     * BottomLeft : (-13, -15, 0) fPos (-14, -17, 0) Cell
     * TopLeft : (-13, 14,0) fPos (-14, 12, 0) Cell
     * TopRight : (14, 14, 0) fPos (13, 13, 0) Cell
     */

        
    }

    //------------------ Accesseurs --------------------
    //Ces accesseurs sont utilisé par le GameManager qui gère l'état des fantomes en fonction du chrono.
    public int getState()
    {
        return state;
    }
    public void setState(int newState)
    {
        state = newState;
    }

}
        
