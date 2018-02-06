using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

abstract public class Grid_character : MonoBehaviour {

    //var
    public float speed;
    protected bool moving;

    protected string direction;

    //Pos
    protected Vector3Int Cell;

    //Components
    protected Tilemap tilemap;
    protected Vector3 targetPos;

    //UI
    public Text MouseText;

    // Use this for initialization
    /*
    public void Start () {

        tilemap = (GameObject.Find("Tilemap")).GetComponent<Tilemap>();
        Cell = tilemap.WorldToCell(transform.position);
        targetPos = transform.position;
        moving = false;


        Debug.Log("Start.Grid_mov", gameObject);

    } */
	
	/* Update is called once per frame
	protected void Update () {

        updateCell();


    } */


    //Renvoie les positions centrales (world) des cases adjacentes 
    public Vector3 rightCell() {
        return tilemap.GetCellCenterWorld(Cell + Vector3Int.right );
    }
    public Vector3 leftCell()
    {
        return tilemap.GetCellCenterWorld(Cell + Vector3Int.left);
    }
    public Vector3 topCell()
    {
        return tilemap.GetCellCenterWorld(Cell + Vector3Int.up);
    }
    public Vector3 downCell()
    {
        return tilemap.GetCellCenterWorld(Cell + Vector3Int.down);
    }
    public Vector3 caseAdj(string dir)
    {
        switch (dir)
        {
            case "Left":
                return leftCell();
            case "Right":
                return rightCell();
            case "Down":
                return downCell();
            case "Up":
                return topCell();
            default:
                return Vector3Int.zero;
        }
    }
    public bool isOppositeDirection(string dir)
    {
        switch (dir)
        {
            case "Right":
                return direction == "Left";
            case "Left":
                return direction == "Right";
            case "Down":
                return direction == "Up";
            case "Up":
                return direction == "Down";
            default:
                return false;
        }
    }

    public string oppositeDirection(string dir)
    {
        switch (dir)
        {
            case "Right":
                return "Left";
            case "Left":
                return "Right";
            case "Down":
                return "Up";
            case "Up":
                return "Down";
            default:
                Debug.Log("oppositeDirection : CAS DEFAULT !", gameObject);
                return "Top";//PLACEHOLDER
        }
    }

    //Retourne Vrai si la case donné est un mur
    public bool isWall(Vector3Int posCell) {
        return tilemap.HasTile(posCell); }
    //Retourne vrai si la position donné fait partir d'une case qui est un mur.
    public bool isWall(Vector3 posWorld){
        return tilemap.HasTile(tilemap.WorldToCell(posWorld)); }

    //Déplace gameObject vers la case Cell donné (Vector3Int issue de "tilemap.WorldToCell(transform.position)" ) 
    public void moveToCell(Vector3Int posCell)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, tilemap.GetCellCenterWorld(posCell), step);
    }

    //Déplace gameObject vers la position (transform.position) donnée
    //Utilisé si on a déjà le centre d'une case.
    public void moveToCell(Vector3 pos)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, pos, step);
    }

    //Met à jour la position Cell avec la position de la case actuelle.
    //Pas très important d'en faire une fonction à part, c'est surtout pour ne jamais à avoir à manipuler directement tilemap dans les classe filles.
    //(Cette classe est une boite à outil qui nous sert à mieux construire les déplacements en grille)
    public void updateCell()
    {
        Vector3Int CellActuelle = tilemap.WorldToCell(transform.position);
        if (Cell != CellActuelle)
            Cell = CellActuelle;
    }

    public void setTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }
    public string getDirection()
    {
        return direction;
    }
}
