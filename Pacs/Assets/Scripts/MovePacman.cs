using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MovePacman : MonoBehaviour {


    public float speed;
    public float toleranceVir;
    public Text posText;
    public static Vector3 posPacmanCell; 

    //GridLayout gridLayout;
    Tilemap tilemap;


    private Rigidbody2D rb;
    SpriteRenderer pacman_SpriteR;

    private string lastDirection;
    private Vector2 direction = Vector2.zero;

    private float moveHorizontal;
    private float moveVertical;

    // Use this for initialization
    void Start () {

        posPacmanCell = GetComponent<Transform>().position;
        rb = GetComponent<Rigidbody2D>();

        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        //gridLayout = transform.parent.GetComponentInParent<GridLayout>();

        print(tilemap.cellLayout);

        //Fetch the SpriteRenderer from the GameObject
        pacman_SpriteR = GetComponent<SpriteRenderer>();

    }


    void Update()
    {
        Vector2 oldDirection = direction;
        Vector2 newDirection = direction;
        //string dernierDirection = lastDirection;
        //Récupération input
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        //Nouvelle Direction :
        if (moveHorizontal > 0.0) lastDirection = "Right";
        else if (moveHorizontal < 0.0) lastDirection = "Left";
        else if (moveVertical > 0.0) lastDirection = "Up";
        else if (moveVertical < 0.0) lastDirection = "Down";

        //Movement

        //On vérifie si lastdirection est défini, si Pacman est au centre d'une case, et si il a entrée une direction différente de la précédente.
        if (lastDirection != null && estAuCentreDeLaCase() ) {
            //Transform.SetPositionAndRotation(new Vector3((int)transform.position.x, (int)transform.position.y, 0), Quaternion.Identity);

            //On prépare le nouveau vecteur de direction
            switch (lastDirection)
            {
                case "Right":
                    newDirection = Vector2.right;
                    break;
                case "Left":
                    newDirection = Vector2.left;
                    break;
                case "Up":
                    newDirection = Vector2.up;
                    break;
                case "Down":
                    newDirection = Vector2.down;
                    break;
            }


            //On vérifie si il y a un mur ou non dans cette nouvelle direction.
            /* Raycast tire un trait invisible à partir d'une position, vers une direction voulu, et d'une longueur voulu.
             * Puis nous informe si il a eu une collision avec un élément ayant des collisions.
             * On s'en sert ici pour savoir si il y a un mur ou non à côté ou devant Pacman, pour savoir si il peut tourner/avancer.
             * */
            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, newDirection, (float)1.0, LayerMask.GetMask("Wall"));
            //Si il n'y a pas de collision il peut tourner.
            if (hitWall.collider == null)
                direction = newDirection;
            else //Si il ne tourne pas on vérifie qu'il peut encore avancer
            {
                hitWall = Physics2D.Raycast(transform.position, direction, (float)1.0, LayerMask.GetMask("Wall"));
                if (hitWall.collider != null)
                {
                    direction = Vector2.zero;
                }
            }




        }
        
        //Si Pacman n'est pas au centre d'une case, sa seule autre direction valable est un demi-tour, on vérifie ici
        //Si pacman essaye de faire un demie-tour, et si oui, on change sa direction.
        else if (lastDirection != null)
        {
            switch (lastDirection)
            {
                case "Right":
                    if (direction == Vector2.left)
                        direction = Vector2.right;
                    break;
                case "Left":
                    if (direction == Vector2.right)
                        direction = Vector2.left;
                    break;
                case "Up":
                    if (direction == Vector2.down)
                        direction = Vector2.up;
                    break;
                case "Down":
                    if (direction == Vector2.up)
                        direction = Vector2.down;
                    break;
            }
        }

        //On change la direction du pacman.
        rb.velocity = direction * speed;

        //Si Pacman est à l'arret :
        if ( rb.velocity == Vector2.zero ){
            GetComponent<Animator>().enabled = false; //On stop son animation
            GetComponent<AudioSource>().loop = false; //Il arrete de crier Waka Waka
        }
        //Sinon, si l'animation n'est pas déjà en route, on la relance.
        else if ( !GetComponent<Animator>().enabled ) {
            GetComponent<Animator>().enabled = true; 
            GetComponent<AudioSource>().loop = true; //On redémarer son Waka Waka.
            GetComponent<AudioSource>().Play();
        }

        //Si Pacman a changé de direction, on change celle de son sprite.
        if (oldDirection != direction)
            FlipSprite(direction);

        //Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //mouseText.text = "MousePos =" + mousePosition;

        Vector3Int cellPosWorld = tilemap.WorldToCell(transform.position);
        Vector3Int cellPosTilemap = tilemap.WorldToCell(transform.position);
        posPacmanCell = tilemap.GetCellCenterWorld(cellPosWorld);
        posText.text = 
            //"Pacman's Position:x=" + transform.position.x + ",y=" + transform.position.y +
            "\nPacman's gridCell : " + cellPosWorld +
            "\nCell to Pos :" + tilemap.CellToWorld(cellPosWorld)+
            "\nPacman's TileCell : " + cellPosTilemap+
            "\nTilemap Boundaries : " + tilemap.localBounds+
            "\nTilemap size : "+tilemap.size;

    }
    

    public bool estAuCentreDeLaCase()
    {
        //Calcul distance avec le centre de la prochaine case

        //float distance = Vector2.Distance(transform.position, new Vector2(transform.position.x + direction.x, transform.position.y + direction.y));
        float posX = Mathf.Abs(transform.position.x - (int)transform.position.x);
        float posY = Mathf.Abs(transform.position.y - (int)transform.position.y);

        
        if ( posX < toleranceVir && posY < toleranceVir)
            return true;
        else
            return false;
    }


    /*
     * Cette méthode change l'orientation du sprite en fonction de la direction du pacman.
     * */
    public void FlipSprite(Vector2 Dir)
    {
        if ( Dir == Vector2.right ) //Droite
            pacman_SpriteR.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        else if ( Dir == Vector2.left ) //Gauche
            pacman_SpriteR.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        else if ( Dir == Vector2.up ) //Haut
            pacman_SpriteR.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        else if ( Dir == Vector2.down ) //Bas
            pacman_SpriteR.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
    }

}
