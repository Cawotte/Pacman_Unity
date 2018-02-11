
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Blinky_movements : Ghost_movements {


    /*
     * Comportement du fantome Rouge, Blinky :
     * 
     * Blinky a le comportement le plus simple des fantomes :
     *  - En mode Scatter, il tourne dans le coin supérieur gauche de l'écran.
     *  - en mode Chase, il a pour cible Pacman, et donc le poursuis directement.
     * 
     * */



    void Start () {

        //Grid_character init
        tilemap = (GameObject.Find("Tilemap")).GetComponent<Tilemap>();
        Cell = tilemap.WorldToCell(transform.position);
        targetPos = transform.position;
        //Ghost Movement init
        PacTransform = (GameObject.Find("Pacman")).GetComponent<Transform>();
        updatePacPos();

        //Direction fantome
        direction = "Left";
        targetPos = caseDevant();


        //Etat Fantome
        state = 1; //Il commence en Chase
        dejaMort = false;


        //Sprite fantomes
        ghost_SpriteR = gameObject.GetComponent<SpriteRenderer>();
        GhostNormal = ghost_SpriteR.sprite;

        //On initialise le son du fantome:
        fantome_audio = AudioManager.getInstance().Find("Blinky").source;
        fantome_sound = fantome_audio.clip; //Le son du fantome est le clip par défaut défini dans l'inspecteur d'Unity.
        //fantome_afraid est déjà initialisé dans Ghost_movements, la casse mère.
        fantome_audio.PlayDelayed(0.6f);

    }

    // Update is called once per frame
    void Update () {


        /* Debug, sert à faire apparaitre des lignes qui encadre la map pour vérifier les coordonnées des angles.
        Vector3 BottomRight = new Vector3(14, -16, 0);
        Vector3 BottomLeft = new Vector3(-13, -16, 0);
        Vector3 TopLeft = new Vector3(-13, 14, 0);
        Vector3 TopRight = new Vector3(14, 14, 0);

        Debug.DrawLine(TopLeft, TopRight, Color.green);
        Debug.DrawLine(TopRight, BottomRight, Color.green);
        Debug.DrawLine(BottomRight, BottomLeft, Color.green);
        Debug.DrawLine(BottomLeft, TopLeft, Color.green);
        */

        //Mets à jour sa propre position dans Cell et celle du Pacman pour le allerVers.
        updateCell();
        updatePacPos();

        //Si le fantome est détecté hors de la map ( conséquence d'un bug ) on le renvoie au spawn.
        if (estHorsDeLaMap())
            retourAuSpawn();

        //On modifie le volume du bruit du fantome en fct de sa distance avec Pacman, plus il est près, plus il est fort !
        volumeEnFonctionDeDistance();

        switch (state)
        {
            case 0: //Lorsque le fantome est mort
                Dead();
                break;
            case 1: //Lorsqu'il est en mode Chase
                 ChaseAndScatter(PacmanPos);
                break;
            case 2: //Lorsqu'il est en mode Scatter
                ChaseAndScatter(ScatterPos);
                break;
            case 3: //Lorsqu'il est en mode Effrayé
                Frightened();
                break;
        }

    }
}
