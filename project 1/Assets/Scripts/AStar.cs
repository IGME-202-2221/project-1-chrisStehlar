using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// component responsible for all the pathfinding for npcs

public class AStar : MonoBehaviour
{
    // FIELDS

    // meta info that shouldnt change
    public Grid grid; // the pathable and none pathable tiles
    private Tilemap tilemap;
    public TileBase[] pathableTiles;
    private HashSet<TileBase> hashPathables;

    // for the PathTo function
    private HashSet<AStarNode> openCells = new HashSet<AStarNode>();
    private HashSet<AStarNode> closedCells = new HashSet<AStarNode>();
    private Dictionary<Vector2, AStarNode> createdCells = new Dictionary<Vector2, AStarNode>();

    private AStarNode startNode;
    private Vector2 endPos;
    private Stack<Vector2> path;

    public GameObject testObj;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        // set up all the pathable tiles
        hashPathables = new HashSet<TileBase>();
        foreach(TileBase tile in pathableTiles)
        {
            hashPathables.Add(tile);
        }

        tilemap = grid.GetComponentInChildren<Tilemap>(); // could definitely cause problems if the tilemap is passed by value
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            //Debug.Log("Got that tile im lookin for? " + hashPathables.Contains(testTile));
            //IsValidTarget(testObj.transform.position);
            PathTo(testObj.transform.position);
        }
    }

    // METHODS

    public void PathTo(Vector2 targetPos)
    {
        
        if(IsValidTarget(targetPos))
        {
            endPos = targetPos;

            createdCells.Clear();
            openCells.Clear();
            closedCells.Clear();

            // commence the pathfinding algorithm
            // Vector3 startCell = grid.CellToWorld(grid.WorldToCell(targetPos)) + new Vector3(grid.cellSize.x / 2, grid.cellSize.y / 2, 0);
            // Vector3 currentCell = startCell;
            // openCells.Add(startCell);

            startNode = new AStarNode(grid.CellToWorld(grid.WorldToCell(this.transform.position)) + new Vector3(grid.cellSize.x / 2, grid.cellSize.y / 2, 0));
            createdCells.Add(startNode.position, startNode);
            openCells.Add(startNode);

            AStarNode currentNode = startNode;

            CloseCell(currentNode);
            OpenSurroundingCells(currentNode);
            currentNode = GetLowestFCost();

            CloseCell(currentNode);
            OpenSurroundingCells(currentNode);
            currentNode = GetLowestFCost();

            foreach(AStarNode node in openCells)
            {
                Debug.DrawRay(node.position, Vector2.up * 0.25f, Color.green, 10f);
            }

            foreach(AStarNode node in closedCells)
            {
                Debug.DrawRay(node.position, Vector2.up * 0.25f, Color.red, 10f);
            }

            Debug.DrawRay(GetLowestFCost().position, Vector2.up * 0.3f, Color.cyan, 10f);


        }
        else
        {
            Debug.Log("Target out of range.");
        }
    }

    private void CloseCell(AStarNode cell)
    {
        closedCells.Add(cell);
        openCells.Remove(cell);
    }

    private void OpenSurroundingCells(AStarNode source)
    {
        Vector2[] positionsToCheck = {
            new Vector2(source.position.x + (grid.cellSize.x), source.position.y), // right
            new Vector2(source.position.x + (grid.cellSize.x), source.position.y - (grid.cellSize.y)), // bot right
            new Vector2(source.position.x, source.position.y - (grid.cellSize.y)),
            new Vector2(source.position.x - (grid.cellSize.x), source.position.y - (grid.cellSize.y)),
            new Vector2(source.position.x - (grid.cellSize.x), source.position.y),
            new Vector2(source.position.x - (grid.cellSize.x), source.position.y + (grid.cellSize.y)),
            new Vector2(source.position.x, source.position.y + (grid.cellSize.y)),  // top
            new Vector2(source.position.x + (grid.cellSize.x), source.position.y + (grid.cellSize.y)), // top right
        };

        // check the surrounding cells adjacent to the source, see if they exist
        //  5   6   7
        //  4   x   0
        //  3   2   1

        for(int i = 0; i < positionsToCheck.Length; i++)
        {
            if(!createdCells.ContainsKey(positionsToCheck[i]))
            {

                // if there is a pathable tile there then open it up
                if(IsValidTarget(positionsToCheck[i]))
                {
                    AStarNode nodeToAdd = new AStarNode(positionsToCheck[i], startNode, endPos);
                    openCells.Add(nodeToAdd);
                }
            }
            else
            {
                Debug.Log("already a node there");
            }
        }

    }

    private AStarNode GetLowestFCost()
    {

        AStarNode lowestCost = new AStarNode(float.MaxValue); // fake node with max f_cost

        foreach(AStarNode node in openCells)
        {
            if(node.f_cost < lowestCost.f_cost)
            {
                lowestCost = node;
            }
        }

        return lowestCost;
    }

    public bool IsValidTarget(Vector2 target)
    {
        Vector3Int intTarget = grid.WorldToCell(target); //new Vector3Int(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), 0);
        //Vector3 cellCenter = grid.CellToWorld(intTarget) + new Vector3(grid.cellSize.x / 2, grid.cellSize.y / 2, 0);
        //Debug.DrawRay(cellCenter, Vector2.up, Color.red, 10f); // from world to cell to world again will place it at the bottom left of the tile

        if(hashPathables.Contains(tilemap.GetTile(intTarget)))
        {
            return true;
        }
        else
        {
            return false;
        }
        //Debug.Log("Is " + tilemap.GetTile(intTarget) + " valid?" + hashPathables.Contains(tilemap.GetTile(intTarget)));
    }

}

public class AStarNode
{
    // FIELDS

    public AStarNode parent;
    public Vector3 position;
    public float g_cost; // distance from start node
    public float h_cost; // distance from target node
    public float f_cost; // sum of g and h cost

    // CONSTRUCTORS

    // for the start/end node
    public AStarNode(Vector3 position)
    {
        this.position = position;
        parent = null;

        g_cost = 0;
        h_cost = 0;
        f_cost = h_cost + g_cost;
    }

    // for any other node
    public AStarNode(Vector3 position, AStarNode startNode, Vector2 targetPos)
    {
        this.position = position;
        parent = null;

        g_cost = Vector3.Distance(this.position, startNode.position);
        h_cost = Vector3.Distance(this.position, targetPos);
        f_cost = h_cost + g_cost;
    }

    // stand in node for trying to get another node with a lower f_cost
    // BAD PRACTICE find another solution later
    public AStarNode(float f_cost)
    {
        this.position = Vector3.zero;
        parent = null;

        g_cost = 0;
        h_cost = 0;
        this.f_cost = f_cost;
    }

    // METHODS
}

