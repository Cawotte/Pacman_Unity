using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Pacman_movements : Grid_character {


    public Text posText;
    
    SpriteRenderer pacman_SpriteR;
    AudioSource pacman_waka;

    private string newDirection;

    private float moveHorizontal;
    private float moveVertical;
    private bool moving;

    [HideInInspector]
    public static int nbVies; //Nombre de vie de Pacman

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
        pacman_waka = AudioManager.getInstance().Find("Pacman_waka").source;

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
    
        //Si pacman n'est pas sur sa case cible
        if (transform.position != targetPos)
        {
            moveToCell(targetPos); //Il avance vers elle
            if (!moving) //Si Pacman était à l'arrêt
            {
                GetComponent<Animator>().enabled = true; //On re-anime son sprite
                pacman_waka.loop = true; //On redémarre son bruit waka_waka.
                pacman_waka.Play();
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
                    pacman_waka.loop = false; //Il arrete de crier Waka Waka
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

    public void death()
    {
        //Les fantomes passent tous en mode Scatter pour 3s.
        GameManager.getInstance().state = 2;
        GameManager.getInstance().timeLeft = 3f;
        nbVies--;
        AudioManager.getInstance().Find("Pacman_Death").source.Play();
        if (nbVies > 0) //Si ce n'était pas la dernière vie de Pacman, lance sa corountine de respawn
            StartCoroutine("RespawnTime");
        //On renvoie pacman à sa position de départ
        //On joue son bruit de mort.
    }

    /* Co-routine, c'est une fonction qui peut s'étendre sur plusieurs exécutions de la fonction Update.
     * "yield return null" est la ligne où la fonction va reprendre son cours à chaque nouvelle exécution d'Update.
     * 
     * Cette co-routine sert à faire disparaitre physiquement et visuellement Pacman pendant 3s lorsqu'il meurt,
     * avant de le faire réapparaitre à son point de spawn.
     * 
     * */
    IEnumerator RespawnTime()
    {
        float timeLeft = 3f;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        while ( timeLeft >= 0f )
        {
            timeLeft -= Time.deltaTime;
            //Debug.Log("Temps restant spawntime: " + timeLeft);
            yield return null;
        }
        //On renvoie pacman à sa position initial
        transform.position = new Vector3(1, -9, 0);
        targetPos = new Vector3(1, -9, 0);

        //Une autre co-routine qui rend invincible Pacman pendant 1.5s pour éviter qu'il se fasse manger d'office par un éventuel fantome.
        StartCoroutine("FrameInvincibilite");

        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    //Rend Pacman invincible (en désactivant son collider) pendant 1.5s. Utilisé lors de son respawn pour éviter qu'il meurt trop vite
    IEnumerator FrameInvincibilite()
    {
        float timeLeft = 1.5f;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        //Debug.Log("Pacman est invincible !");
        while (timeLeft >= 0f)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        //Debug.Log("Pacman n'est plus invincible !");
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    public void setVie(int vie)
    {
        nbVies = vie;
    }
}
