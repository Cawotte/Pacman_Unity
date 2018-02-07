using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pacman_collisions : MonoBehaviour {
    

    

    void OnTriggerEnter2D (Collider2D coll) {

        if (/*gameObject.tag == "Player" &&*/ coll.gameObject.tag == "Pellet" )
        {
            GameManager.Score++; //A changer
            GameManager.numPellet--;
            Destroy(coll.gameObject);
        }

        if (coll.gameObject.tag == "SuperPellet")
        {
            //Debug.Log("SuperPellet mangée !", gameObject);
            GameManager.Score++; //A changer
            GameManager.numPellet--;
            Destroy(coll.gameObject);

            //Tout les fantomes deviennent effrayés.
            Component[] GhostScripts;
            GhostScripts = GameObject.Find("Fantomes").GetComponentsInChildren<Ghost_movements>();
            foreach (Ghost_movements script in GhostScripts)
                script.setState(3);

        }

        if (/*coll.gameObject.tag == "Player" &&*/ coll.gameObject.tag == "Ghost")
        {
            int stateGhost = coll.gameObject.GetComponent<Ghost_movements>().getState();
            if (stateGhost != 3) //Si le fantome n'est pas effrayé, Pacman meurt.
            {
                GameObject.Find("Pacman").GetComponent<Transform>().position = new Vector3(1, -9, 0);
                GameObject.Find("Pacman").GetComponent<Pacman_movements>().setTargetPos( new Vector3(1, -9, 0) );
                //PlaceHolder de la mort, Pacman est juste TP à sa position de départ.
            }
            else //Le Fantome est effrayé, on le mange.
            {
                //PlaceHolder de la mort, le fantome est renvoyé dans la maison.
                /*
                coll.gameObject.GetComponent<Transform>().position = new Vector3(0, 0, 0);
                coll.gameObject.GetComponent<Ghost_movements>().setTargetPos(new Vector3(0, 0, 0));
                coll.gameObject.GetComponent<Ghost_movements>().setState(2);
                */

                coll.gameObject.GetComponent<Ghost_movements>().death();
            }
        }
    }
}
