using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Pinky_movements : Ghost_movements {


     /*
     * Comportement du fantome Rose, Pinky :
     * 
     * comportement de pinky :
     *  - En mode Scatter, elle tourne dans le coin supérieur droit de l'écran
     *  - en mode Chase, elle a pour cible la case 4 cases devant Pacman. Elle essaye donc de le devancer.
     * 
     * */


        //Pinky a besoin de ces attributs pour récupérer la direction de Pacman et savoir dans quel direction
        //compter 4 cases.
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
        PacmanScript = (GameObject.Find("Pacman")).GetComponent<Grid_character>();

        //On initialise le son du fantome:
        fantome_audio = AudioManager.getInstance().Find("Pinky").source;
        fantome_sound = fantome_audio.clip; //Le son du fantome est le clip par défaut défini dans l'inspecteur d'Unity.
        //fantome_afraid est déjà initialisé dans Ghost_movements, la casse mère.
        fantome_audio.PlayDelayed(0.3f);


    }

    // Update is called once per frame
    void Update()
    {

        //Mets à jour sa propre position dans Cell et celle du Pacman pour le allerVers.
        updateCell();
        updatePacPos();
        directionPac = PacmanScript.getDirection();


        //On modifie le volume du bruit du fantome en fct de sa distance avec Pacman, plus il est près, plus il est fort !
        volumeEnFonctionDeDistance();


        switch (state)
        {
            case 0: //Lorsque le fantome est mort
                Dead();
                break;
            case 1: //Lorsqu'il est en mode Chase
                ChaseAndScatter(quatreCasesDevantPacman());
                break;
            case 2: //Lorsqu'il est en mode Scatter
                ChaseAndScatter(ScatterPos);
                break;
            case 3: //Lorsqu'il est en mode Frightened
                Frightened();
                break;
        }

    }

    //Fonction propre à Pinky, qui renvoie le centre de la case 4 cases devant Pacman :
    public Vector3 quatreCasesDevantPacman()
    {
        switch (directionPac)
        {
            case "Right":
                return PacmanPos + (Vector3.right * 4);
            case "Left":
                return PacmanPos + (Vector3.left * 4);
            case "Down":
                return PacmanPos + (Vector3.down * 4);
            case "Up":
                return PacmanPos + (Vector3.up * 4);
            default:
                Debug.Log("quatreCasesDevant : CAS DEFAULT !", gameObject);
                return PacmanPos;
        }
    }
}

