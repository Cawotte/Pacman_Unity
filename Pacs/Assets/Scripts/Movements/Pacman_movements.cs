using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Pacman_movements : Grid_character {


    public Text posText;
    
    SpriteRenderer pacman_SpriteR;

    private string newDirection;

    private float moveHorizontal;
    private float moveVertical;
    private bool moving;

    private int nbVies; //Nombre de vie de Pacman

    // Use this for initialization
    void Start()
    {

        //init Cell, moving, targetPost et tilemap.
        tilemap = (GameObject.Find("Tilemap")).GetComponent<Tilemap>();
        Cell = tilemap.WorldToCell(transform.position);
        targetPos = transform.position;
        moving = false;

        direction = "Right";
        newDirection = direction;

        //Récupère le sprite Renderer Pacman.
        pacman_SpriteR = GetComponent<SpriteRenderer>();

        nbVies = 3;
    }


    void Update()
    {

        updateCell();

        //On enregistre la dernière direction donnée en input:
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal > 0.0) newDirection = "Right";
        else if (moveHorizontal < 0.0) newDirection = "Left";
        else if (moveVertical > 0.0) newDirection = "Up";
        else if (moveVertical < 0.0) newDirection = "Down";

        //Si la nouvelle direction est un demi-tour, on change immédiatement de direction pour ne pas avoir à attendre la prochaine case.
        if (isOppositeDirection(newDirection))
            changerDirection();
    
        //Pacman avance vers la case cible si il n'est pas déjà dessus.
        if (transform.position != targetPos)
        {
            moveToCell(targetPos);
            if (!moving) //Si Pacman était à l'arrêt
            {
                GetComponent<Animator>().enabled = true;
                GetComponent<AudioSource>().loop = true; //On redémarer son Waka Waka.
                GetComponent<AudioSource>().Play();
                moving = true;
            }
        }
        else //Si il est déjà dessus, on change de cible.
        {
            //Si newDirection est une direction valide ( sans mur et n'est pas l'entrée du spawn des fantomes ), c'est la nouvelle direction.
            if (newDirection != direction && !isWall(caseAdj(newDirection)) /*&& nestPasEntreeSpawn(caseAdj(newDirection))*/ )
                changerDirection();
            //Sinon tant que la direction actuelle est valide il continue vers celle-ci.
            else if (!isWall(caseAdj(direction)))
                targetPos = caseAdj(direction);
            else //Si Pacman ne bouge pas
            {
                //Si il vient de s'arrêter
                if (moving)
                {
                    moving = false;
                    GetComponent<Animator>().enabled = false; //On stop son animation
                    GetComponent<AudioSource>().loop = false; //Il arrete de crier Waka Waka
                }
            }
        }

        //Debug
        posText.text = "Pacman's Cell : " + Cell + //Vector3Int
            "\nPacman's pos : " + transform.position; //Vector3
    }

    //Change la direction de pacman, met à jour son sprite, et la direction vers laquelle il se dirige.
    public void changerDirection()
    {
        direction = newDirection; //On change la direction
        targetPos = caseAdj(direction); //On change la cible
        turnSprite(direction);//On update le sprite
    }


    /*
     * Cette méthode change l'orientation du sprite en fonction de la direction du pacman.
     * */
    public void turnSprite(string dir)
    {
        switch (dir)
        {
            case "Right":
                pacman_SpriteR.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case "Left":
                pacman_SpriteR.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                break;
            case "Down":
                pacman_SpriteR.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                break;
            case "Up":
                pacman_SpriteR.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
        }
    }

    public int getVie()
    {
        return nbVies;
    }
    public void setVie(int vie)
    {
        nbVies = vie;
    }
}
