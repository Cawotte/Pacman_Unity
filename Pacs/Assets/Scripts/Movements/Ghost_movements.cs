using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Ghost_movements : Grid_character {

    protected Vector3 PacmanPos;
    protected Vector3Int PacmanPosCell;
    protected Transform PacTransform;

    //Constantes 
    public const float DUREE_CHASE = 20.0f;
    public const float DUREE_SCATTER = 7.0f;
    public const float DUREE_AFRAID = 7.0f;
    public const float DUREE_MORT = 3.0f;
    public const float REDUCTION_VITESSE = 4.0f;
    //La case autour de laquelle le fantome va tourner en mode Scatter.
    public Vector3Int ScatterPos = new Vector3Int(13, 13, 0);
    protected Vector3 PositionDepart;
    
    /* Les angles
     * 
     * BottomRight : (14, -16, 0) fPos (13, -14, 0) Cell
     * BottomLeft : (-13, -15, 0) fPos (-14, -17, 0) Cell
     * TopLeft : (-13, 14,0) fPos (-14, 12, 0) Cell
     * TopRight : (14, 14, 0) fPos (13, 13, 0) Cell
     */

    protected int state;
    /* Un Fantome a 3 Etat :
     * 1 : Chase, il utilise son algorithme de poursuite de Pacman
     * 2 : Scatter, il se diriger vers un des bloc aux angles de la map et tourne autour. 
     *      Cette angle est différent pour chaque fantome, il est définit dans <ScatterPos>
     * 3 : Frightened, le fantome est effrayé, il court dans tout les sens et peut se faire manger.
     */
    protected int oldState; //Quand il est effrayé, on retient son état précédent.

    //protected Vector3Int Cell;

    protected float timeLeft;
    protected bool afraid;

    protected SpriteRenderer ghost_SpriteR;
    protected Sprite GhostNormal;
    public Sprite GhostAfraid;


    /*
    public void Start () {

        //init Cell, moving, targetPos et tilemap.
        base.Start();

        //On recupère les coordonnées de Pacman à tracer :
        PacTransform = (GameObject.Find("Pacman")).GetComponent<Transform>();

        updatePacPos();

        direction = "Left";
        targetPos = caseDevant();

        state = 1;

        

    }
	
	// Update is called once per frame
	void Update () {

        updateCell();
        updatePacPos();

        if (estDansSpawn())
            sortirSpawn();
        else
            allerVers(PacmanPos);

        MouseText.text =
            //"Pacman's Position:x=" + transform.position.x + ",y=" + transform.position.y +
            "\nGhost's direction = " + direction +
            "\nTargetPos = " + targetPos +
            "\nCell = " + Cell;

    } */

    //Renvoie la distance entre deux cellules.
    public float dist(Vector3 posA, Vector3 posB)
    {
        return Mathf.Sqrt(Mathf.Pow(posB.x - posA.x, 2) + Mathf.Pow(posB.y - posA.y, 2));
    }
    //Renvoie les coordonnées de la position (posA ou posB) la plus proche de targetPos.
    public Vector3 plusProche(Vector3 posA, Vector3 posB, Vector3 targetPos)
    {
        if ( dist(posA, targetPos) <= dist(posB, targetPos))
            return posA;
        return posB;
    }

    //Fais avancer le fantome dans le labyrinthe en direction de la case cible, en prenant en compte les murs et
    //choississant une direction à chaque croisement le rapprochant de sa case cible.
    // 
    public void allerVers(Vector3 targetPosition)
    {
        if (transform.position != targetPos)
            moveToCell(targetPos);
        //Si il est sur sa cible, il change de cible.
        else
        {
            if (estCroisement(Cell))
            {
                //Debug.Log("C'est un croisement", gameObject);
                targetPos = caseAdjLaPlusProche(targetPosition);

            }
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
        if ( !isWall(downCell()) && direction != "Up" && downCell() != new Vector3(0, 2, 0) && downCell() != new Vector3(1, 2, 0))
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
        if (!isWall(downCell()) && direction != "Up" && downCell() != new Vector3(0, 2, 0) && downCell() != new Vector3(1, 2, 0) )
            listC.Add(downCell());
        if (!isWall(rightCell()) && direction != "Left")
            listC.Add(rightCell());
        if (!isWall(topCell()) && direction != "Down")
            listC.Add(topCell());
        if (!isWall(leftCell()) && direction != "Right")
            listC.Add(leftCell());

        //On choisit aléatoirement une case parmi celles disponible :
        //Random rnd = new Random();
        int i = UnityEngine.Random.Range(0, listC.Count-1);
        //Debug.Log(i, gameObject);
        //Debug.Log(listC[i], gameObject);

        return listC[i];
    }

    public bool estDansSpawn()
    {
        //Position des angles du spawn
        /*
        Vector3Int TopLeft = new Vector3Int(-4, 1, 0);
        Vector3Int TopRight = new Vector3Int(3, 1, 0);
        Vector3Int BottomRight = new Vector3Int(3, -3, 0);
        Vector3Int BottomLeft = new Vector3Int(-4, -3, 0);
        */

        return (Cell.x <= 3 && Cell.x >= -4 && Cell.y <= 1 && Cell.y >= -3);

        //Coordonnées case en face sorties
        //Left = (-1,2,0)
        //Right = (0,2,0)

    }

    public void sortirSpawn()
    {
        //Il y a deux cases vers lesquels se diriger pour sortir du spawn, le fantome se dirige vers
        //la plus proche d'entre elle
        allerVers(plusProche(new Vector3(-1, 2, 0), new Vector3(0, 2, 0), Cell));

    }

    //Renvoie vrai si la case en argument est un croisement (a 3 cases vide adjacentes ou plus)
    public bool estCroisement(Vector3Int cell)
    {
        int nb_caseAdj = 0;

        if ( !isWall(rightCell()) )
            nb_caseAdj++;
        if ( !isWall(leftCell()) )
            nb_caseAdj++;
        if ( !isWall(downCell()) && downCell() != new Vector3(0, 2, 0) && downCell() != new Vector3(1, 2, 0) )
            nb_caseAdj++;
        if ( !isWall(topCell()) )
            nb_caseAdj++;

        if (nb_caseAdj >= 2)
            return true;
        return false;
    }

    public void updatePacPos()
    {
        PacmanPosCell = tilemap.WorldToCell(PacTransform.position);
        PacmanPos = tilemap.GetCellCenterWorld(PacmanPosCell);
    }

    //Change la variable "direction" du fantome en fonction de sa case actuelle et de la case cible.
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

    public void death()
    {
        state = 0;
        transform.position  = PositionDepart;
        targetPos = transform.position;
        timeLeft = DUREE_MORT;
    }

    public int getState()
    {
        return state;
    }
    public void setState(int newState)
    {
        state = newState;
    }
    public void setTimeLeft(float time)
    {
        timeLeft = time;
    }

    ////////-----------------------------------------------------
    /////////--------------------------------------------------------

        /* Fonction du mode Chase des fantomes. Elle prend en paramètre la position de la case cible vers laquelle
         * le fantome va essayer de se diriger, elle varie en fonction du fantome.
         * 
         * */
    public void Chase(Vector3 cible)
    {
        MouseText.text = "Mode Chase !";
        timeLeft -= Time.deltaTime;


        if (estDansSpawn())
            sortirSpawn();
        else
            allerVers(cible);

        if (timeLeft <= 0.0f)
        {
            faireDemiTour();
            timeLeft = DUREE_SCATTER; //On réinitialise le timer avec la durée de Scatter.
            state = 2;
        }
    }

    public void Scatter()
    {
        MouseText.text = "Mode Scatter !";
        timeLeft -= Time.deltaTime;


        if (estDansSpawn())
            sortirSpawn();
        else
            allerVers(ScatterPos);

        if (timeLeft <= 0.0f)
        {
            faireDemiTour();
            timeLeft = DUREE_CHASE; //On réinitialise le timer avec la durée de Chase
            state = 1;
        }
    }

    public void Frightened()
    {

        MouseText.text = "Mode Frightened !";

        if (!afraid)
        {
            speed -= REDUCTION_VITESSE;
            ghost_SpriteR.sprite = GhostAfraid;
            afraid = true;
            timeLeft = DUREE_AFRAID;
            targetPos = caseDevant();
        }

        timeLeft -= Time.deltaTime;

        if (transform.position != targetPos)
            moveToCell(targetPos);
        else
        {
            if (estCroisement(Cell))
                targetPos = caseAdjAleatoire();
            else
                targetPos = caseDevant();
            updateDirection(targetPos);
        }

        if (timeLeft <= 0.0f)
        {
            ghost_SpriteR.sprite = GhostNormal; //On rétablit son sprite.
            speed += REDUCTION_VITESSE; //On rétabli sa vitesse.
            state = 2; //Il revient à l'état Scatter
            timeLeft = DUREE_SCATTER;
            afraid = false;
        }
    }

    public void Mort()
    {

        timeLeft -= Time.deltaTime;


        if (timeLeft <= 0.0f)
        { //Il renait
            ghost_SpriteR.sprite = GhostNormal; //On rétablit son sprite.
            speed += REDUCTION_VITESSE; //On rétabli sa vitesse.
            state = 1; //Il revient à l'état chase
            timeLeft = DUREE_CHASE;
            afraid = false;
        }
    }


}
        
