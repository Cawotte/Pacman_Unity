using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pacman_collisions : MonoBehaviour {

    /*
     * Cette classe est appellé dès que Pacman entre en collision avec un autre élément :
     * Si c'est un point, il le mange et augmente son score.
     * Si c'est un super point, il le mange, augmente son score et effraie les fantomes.
     * Si c'est un fantome, soit Pacman meurt et perds des points, soit le fantome est effrayé et pacman le mange et gagne des points.
     **/
    GameObject pacman;
    GameObject gameManager;

    void Start()
    {
        pacman = GameObject.Find("Pacman");
        gameManager = GameObject.Find("GameManager");
    }

    void OnTriggerEnter2D (Collider2D coll) {

        if ( coll.gameObject.tag == "Pellet" )
        {
            GameManager.Score+= 10; //A changer
            GameManager.numPellet--;
            Destroy(coll.gameObject);
        }

        if ( coll.gameObject.tag == "SuperPellet")
        {
            GameManager.Score+= 50; //A changer
            GameManager.numPellet--;
            Destroy(coll.gameObject); //On détruit le point.

            //On règle l'état à 3 pour Frightened dans le GameManager, a pour conséquence d'effrayer tout les fantomes.
            gameManager.GetComponent<GameManager>().Frighten();

        }

        if ( coll.gameObject.tag == "Ghost")
        {

            int stateGhost = coll.gameObject.GetComponent<Ghost_movements>().getState();

            if (stateGhost != 3) //Si le fantome n'est pas effrayé, Pacman meurt.
            {
                pacman.GetComponent<Transform>().position = new Vector3(1, -9, 0);
                pacman.GetComponent<Pacman_movements>().setTargetPos( new Vector3(1, -9, 0) );
                GameManager.Score -= 250;
                //PlaceHolder de la mort, Pacman est juste TP à sa position de départ.
            }
            else //Le Fantome est effrayé, on le mange.
            {
                coll.gameObject.GetComponent<Ghost_movements>().death();
                GameManager.Score += 200;
            }
        }
    }
}
