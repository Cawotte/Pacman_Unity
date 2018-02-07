using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Clyde_movements : Ghost_movements
{

    //Grid_character PacmanScript;
    //string directionPac;

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
        state = 1; //Il commence en Chase
        afraid = false;
        //timeLeft = DUREE_CHASE;
        PositionDepart = tilemap.LocalToWorld(Cell);

        //Sprite fantomes
        ghost_SpriteR = gameObject.GetComponent<SpriteRenderer>();
        GhostNormal = ghost_SpriteR.sprite;

        //On recupere la référence du script de Pacman, pour récupérer sa direction.
        //PacmanScript = (GameObject.Find("Pacman")).GetComponent<Grid_character>();


    }

    // Update is called once per frame
    void Update()
    {

        //Mets à jour sa propre position dans Cell et celle du Pacman pour le poursuivre.
        updateCell();
        updatePacPos();
        //directionPac = PacmanScript.getDirection();

        //Debug.DrawLine(PacmanPos, quatreCasesDevantPacman(), Color.red);
        //Debug.DrawLine(Cell, quatreCasesDevantPacman(), Color.green);

        //Comportement différente en fonction de l'état du fantome
        // 1 : Chase, 2 : Scatter, 3 : Frightened.
        // Il change de mode entre Scatter et Chase périodiquement, 7s de Scatter pour 20s de Chase. Frightened se déclenche uniquement si Pacman mange une super boulette.
        // Quand le fantome change de mode entre Chase/Scatter, il fait immédiatement demi-tour. 



        if (state == 3)
        { //Mode Frightened : Le fantome est ralenti, sensible, et se déplace aléatoirement.
          //On utilise un autre compteur pour calculer la durée de Frightened car timeLeft doit être en Pause.

            MouseText.text = "Mode Frightened !";

            //Si il vient d'entrer en mode Frightened
            if (!afraid)
            {
                speed -= REDUCTION_VITESSE;
                ghost_SpriteR.sprite = GhostAfraid;
                afraid = true;
                timeLeft = DUREE_AFRAID;
                targetPos = caseDevant();
            }

            timeLeft -= Time.deltaTime;


            //poursuivre(ScatterPos);

            //Le Fantome choisit une direction aléatoire lorsqu'il arrive à un croisement.
            if (transform.position != targetPos)
            {
                //Debug.Log("transform != targetpos", gameObject);
                moveToCell(targetPos);
            }
            else
            {
                //Debug.Log("else", gameObject);
                if (estCroisement(Cell))
                {
                    //Debug.Log("C'est un croisement!", gameObject);
                    targetPos = caseAdjAleatoire();
                }
                else
                    targetPos = caseDevant();
                updateDirection(targetPos);
            }

            //Lorsque la durée de l'effraiement du Fantome est terminée :
            if (timeLeft <= 0.0f)
            {
                ghost_SpriteR.sprite = GhostNormal; //On rétablit son sprite.
                speed += REDUCTION_VITESSE; //On rétabli sa vitesse.
                state = 1; //Il revient à l'état Scatter
                timeLeft = DUREE_SCATTER;
                afraid = false;
            }

        }
        else if ( state == 0 )//Sinon il est "mort"
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
        else if (distanceEntreClydeEtPacman() >= 8.0f)
        { //Mode Chase :  poursuis Pacman.

                if (estDansSpawn())
                    sortirSpawn();
                else
                    poursuivre(PacmanPos);
        }
        else
        { //Mode Scatter : Il va roder dans l'angle de la map qui lui est attribué.

            if (estDansSpawn())
                sortirSpawn();
            else
                poursuivre(ScatterPos);
        }

    }
    

    public float distanceEntreClydeEtPacman()
    {
        return dist(transform.position, PacmanPos);
    }
}

