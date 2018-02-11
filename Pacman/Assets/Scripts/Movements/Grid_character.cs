using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

abstract public class Grid_character : MonoBehaviour {

    //var
    public float speed; //Vitesse du personnage

    protected string direction;

    //Pos
    protected Vector3Int Cell;

    //Components
    protected Tilemap tilemap;
    protected Vector3 targetPos;

    //UI
    public Text MouseText;


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

    //Renvoie la distance entre deux cellules.
    public float dist(Vector3 posA, Vector3 posB)
    {
        return Mathf.Sqrt(Mathf.Pow(posB.x - posA.x, 2) + Mathf.Pow(posB.y - posA.y, 2));
    }
    //Renvoie les coordonnées de la position (posA ou posB) la plus proche de targetPos.
    public Vector3 plusProche(Vector3 posA, Vector3 posB, Vector3 targetPos)
    {
        if (dist(posA, targetPos) <= dist(posB, targetPos))
            return posA;
        return posB;
    }

    //Met à jour la position Cell avec la position de la case actuelle.
    //Pas très important d'en faire une fonction à part, c'est surtout pour ne jamais à avoir à manipuler directement le Component tilemap dans les classe filles.
    public void updateCell()
    {
        Cell = tilemap.WorldToCell(transform.position);
    }

    //Booléens

    //Retourne vrai si la case en argument n'est pas une des deux cases de la porte du spawn des fantomes.
    public bool nestPasEntreeSpawn(Vector3 pos)
    {
        return pos != new Vector3(0, 2, 0) && pos != new Vector3(1, 2, 0);
    }

    //Retourne Vrai si la case donné est un mur
    public bool isWall(Vector3Int posCell)
    {
        return tilemap.HasTile(posCell);
    }
    //Retourne vrai si la position donné fait partir d'une case qui est un mur.
    public bool isWall(Vector3 posWorld)
    {
        return tilemap.HasTile(tilemap.WorldToCell(posWorld));
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

    //Permet de savoir si le personnage est encore dans le labyrinthe, utilisé en cas de bug si pacman ou un fantome passe à travers le mur du niveau.
    public bool estHorsDeLaMap()
    {
        if ( Cell.x < -13 || Cell.x > 14 || Cell.y < -16 || Cell.y > 14)
        {
            Debug.Log(gameObject.name + " est sorti de la carte !", gameObject);
            return true;
        }
        return false;
    }

    //Accesseurs

    public void setTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }
    public string getDirection()
    {
        return direction;
    }
}
