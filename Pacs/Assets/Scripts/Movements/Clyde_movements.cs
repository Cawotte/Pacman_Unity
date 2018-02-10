using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Clyde_movements : Ghost_movements
{

    /*
     * Comportement du fantome Orange, Clyde :
     * 
     * Contrairement aux autres fantomes, il n'obéit pas au timer pour ses modes, mais en fonction de sa distance avec Pacman.
     * 
     * Si Clyde est à 8 cases ou plus de Pacman, il est en mode Chase et a pour cible Pacman, comme Blinky le fantome orange.
     * Si Clyde est à moins de 8 cases de Pacman, il est alors en mode Scatter et part tourner dans le coin inférieur gauche de l'écran.
     * 
     * */

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
        direction = "Left";
        targetPos = caseDevant();

        //Etat Fantome
        state = 1;
        dejaMort = false;

        //Sprite fantomes
        ghost_SpriteR = gameObject.GetComponent<SpriteRenderer>();
        GhostNormal = ghost_SpriteR.sprite;

        //On initialise le son du fantome:
        fantome_audio = AudioManager.getInstance().Find("Clyde").source;
        fantome_sound = fantome_audio.clip; //Le son du fantome est le clip par défaut défini dans l'inspecteur d'Unity.
        //fantome_afraid est déjà initialisé dans Ghost_movements, la casse mère.
        fantome_audio.Play();

    }

    // Update is called once per frame
    void Update()
    {

        //Mets à jour sa propre position dans Cell et celle du Pacman pour les calculs.
        updateCell();
        updatePacPos();

        //Comportement différente en fonction de l'état du fantome
        // 1 : Chase, 2 : Scatter, 3 : Frightened.
        // Il change de mode entre Scatter et Chase périodiquement, 7s de Scatter pour 20s de Chase. Frightened se déclenche uniquement si Pacman mange une super boulette.
        // Quand le fantome change de mode entre Chase/Scatter, il fait immédiatement demi-tour. 



        if (state == 3)
        { //Mode Frightened : Le fantome est ralenti, sensible, et se déplace aléatoirement.
          //On utilise un autre compteur pour calculer la durée de Frightened car timeLeft doit être en Pause.
            Frightened();

        }
        else if ( state == 0 )//Sinon il est "mort"
        {
            Dead();
        }
        else if (distanceEntreClydeEtPacman() >= 8.0f)
        { //Mode Chase :  poursuis Pacman.

            ChaseAndScatter(PacmanPos);
        }
        else
        { //Mode Scatter : Il va roder dans l'angle de la map qui lui est attribué.

            ChaseAndScatter(ScatterPos);
        }



    }
    

    public float distanceEntreClydeEtPacman()
    {
        return dist(transform.position, PacmanPos);
    }
}

