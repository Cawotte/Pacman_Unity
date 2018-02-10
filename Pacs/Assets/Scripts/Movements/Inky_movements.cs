using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Inky_movements : Ghost_movements
{


    /*
    * Comportement du fantome Bleu, Inky :
    * 
    * Inky a le comportement le plus spécifique des fantomes :
    *  - En mode Scatter, il tourne dans le coin inférieur droit de l'écran
    *  - en mode Chase, il a une case cible assez particulière a calculer. Il travaille de concert avec Blinky (fantome rouge) pour attraper Pacman :
    *          On trace un vecteur entre la position de Blinky, et la position de la case 2 cases devant Pacman.
    *          On double la longueur de ce vecteur.
    *          Ce vecteur pointe alors sur la case cible d'Inky.
    * 
    * */



        //Nécessaire pour connaitre la position de Blinky, et la direction de Pacman pour le calcul de la case cible.
    Transform BlinkyTrans;
    Vector3 BlinkyPos;
    Grid_character PacmanScript;
    string directionPac;


    // Use this for initialization
    void Start()
    {

        //Grid_character init
        tilemap = (GameObject.Find("Tilemap")).GetComponent<Tilemap>();
        Cell = tilemap.WorldToCell(transform.position);
        targetPos = transform.position;
        //Ghost Movement init
        PacTransform = (GameObject.Find("Pacman")).GetComponent<Transform>();
        updatePacPos();

        //Direction fantome
        direction = "Up";
        targetPos = caseDevant();

        //Etat Fantome
        state = 1; //Il commence en Chase
        dejaMort = false;

        //Sprite fantomes
        ghost_SpriteR = gameObject.GetComponent<SpriteRenderer>();
        GhostNormal = ghost_SpriteR.sprite;

        //On recupere la référence du script de Pacman, pour récupérer sa direction.
        BlinkyTrans = (GameObject.Find("Blinky")).GetComponent<Transform>();
        PacmanScript = (GameObject.Find("Pacman")).GetComponent<Grid_character>();

        //On initialise le son du fantome:
        fantome_audio = AudioManager.getInstance().Find("Inky").source;
        fantome_sound = fantome_audio.clip; //Le son du fantome est le clip par défaut défini dans l'inspecteur d'Unity.
        //fantome_afraid est déjà initialisé dans Ghost_movements, la casse mère.
        fantome_audio.Play();


    }

    // Update is called once per frame
    void Update()
    {

        //Mets à jour sa propre position dans Cell et celle du Pacman pour le allerVers.
        updateCell();
        updatePacPos();
        BlinkyPos = BlinkyTrans.position;
        directionPac = PacmanScript.getDirection();

      
        switch (state)
        {
            case 0: //Lorsque le fantome est mort
                Dead();
                break;
            case 1: //Lorsqu'il est en mode Chase
                ChaseAndScatter(positionVecteurBlinkyPacman());
                break;
            case 2: //Lorsqu'il est en mode Scatter
                ChaseAndScatter(ScatterPos);
                break;
            case 3: //Lorsqu'il est en mode Effrayé
                Frightened();
                break;
        }

    }

    public Vector3 deuxCasesDevantPacman()
    {
        switch (directionPac)
        {
            case "Right":
                return PacmanPos + (Vector3.right * 2);
            case "Left":
                return PacmanPos + (Vector3.left * 2);
            case "Down":
                return PacmanPos + (Vector3.down * 2);
            case "Up":
                return PacmanPos + (Vector3.up * 2);
            default:
                Debug.Log("quatreCasesDevant : CAS DEFAULT !", gameObject);
                return PacmanPos;
        }
    }

    //Fonction propre à Pinky, qui renvoie le centre de la case 4 cases devant Pacman :
    public Vector3 positionVecteurBlinkyPacman()
    {
        Vector3 posP = deuxCasesDevantPacman();
        /*
         * On trace un vecteur entre la position de Blinky et deux cases devant Pacman, on double la taille de ce vecteur, et on renvoie son 
         * extrémité, qui sera la cible de Inky */
        Vector3 vecteur = new Vector3(posP.x - BlinkyPos.x, posP.y - BlinkyPos.y, 0)*2;

        Vector3 caseCible = BlinkyPos + vecteur;
        Debug.DrawLine(BlinkyPos, caseCible, Color.cyan);

        return caseCible;

    }
}

