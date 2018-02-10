using System.Collections;
using System.Collections.Generic;
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
        fantome_audio.Play();

    }

    // Update is called once per frame
    void Update () {

        //Mets à jour sa propre position dans Cell et celle du Pacman pour le allerVers.
        updateCell();
        updatePacPos();

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
