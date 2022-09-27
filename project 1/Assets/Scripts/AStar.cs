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
    public float minDistanceToTarget;

    // for the PathTo function
    private HashSet<AStarNode> openCells = new HashSet<AStarNode>();
    private HashSet<AStarNode> closedCells = new HashSet<AStarNode>();
    private Dictionary<Vector3Int, AStarNode> createdCells = new Dictionary<Vector3Int, AStarNode>(); // convert form world to cell

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

        path = new Stack<Vector2>();
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
            // pre set up
            endPos = targetPos;

            createdCells.Clear();
            openCells.Clear();
            closedCells.Clear();

            // commence the pathfinding algorithm

            startNode = new AStarNode(grid.CellToWorld(grid.WorldToCell(this.transform.position)) + new Vector3(grid.cellSize.x / 2, grid.cellSize.y / 2, 0));
            createdCells.Add(grid.WorldToCell(startNode.position), startNode);
            openCells.Add(startNode);

            // a* pathing algorithm as found here: https://youtu.be/-L-WgKMFuhE?t=461
            for(int i = 0; i < 100; i++)
            {
                AStarNode currentNode = GetLowestFCost();
                CloseCell(currentNode);

                // check if it reached the target
                if(Vector3.Distance(currentNode.position, targetPos) < minDistanceToTarget)
                {
                    Debug.Log("path found");
                    path.Clear(); // remove the old path
                    StackPath(currentNode); // save the new one
                    DrawPath(currentNode);
                    break;
                }


                // check the neighbor cells
                //  5   6   7
                //  4   x   0
                //  3   2   1
                
                Vector2[] positionsToCheck = {
                    new Vector2(currentNode.position.x + (grid.cellSize.x), currentNode.position.y), // right
                    new Vector2(currentNode.position.x + (grid.cellSize.x), currentNode.position.y - (grid.cellSize.y)), // bot right
                    new Vector2(currentNode.position.x, currentNode.position.y - (grid.cellSize.y)),
                    new Vector2(currentNode.position.x - (grid.cellSize.x), currentNode.position.y - (grid.cellSize.y)),
                    new Vector2(currentNode.position.x - (grid.cellSize.x), currentNode.position.y),
                    new Vector2(currentNode.position.x - (grid.cellSize.x), currentNode.position.y + (grid.cellSize.y)),
                    new Vector2(currentNode.position.x, currentNode.position.y + (grid.cellSize.y)),  // top
                    new Vector2(currentNode.position.x + (grid.cellSize.x), currentNode.position.y + (grid.cellSize.y)), // top right
                };

                for(int c = 0; c < positionsToCheck.Length; c++)
                {
                    // as long as the tile is traversable and not closed (but it has to exist too)
                    if(IsValidTarget(positionsToCheck[c])) //&& (createdCells.ContainsKey(grid.WorldToCell(positionsToCheck[c])) || !closedCells.Contains(createdCells[grid.WorldToCell(positionsToCheck[c])])))
                    {
                        AStarNode nodeToCheck;

                        // make the new node if it doesnt exist
                        if(!createdCells.ContainsKey(grid.WorldToCell(positionsToCheck[c])))
                        {
                            nodeToCheck = new AStarNode(positionsToCheck[c], currentNode, targetPos); // the current node is the start node
                            createdCells.Add(grid.WorldToCell(nodeToCheck.position), nodeToCheck);
                            openCells.Add(nodeToCheck);
                        }
                        // else update the node to reflect a better path
                        else
                        {
                            nodeToCheck = createdCells[grid.WorldToCell(positionsToCheck[c])];

                            // if the path would be shorter with the current node as the parent, then switch the parent
                            if(nodeToCheck.g_cost > (nodeToCheck.g_cost + Vector3.Distance(currentNode.position, nodeToCheck.position)) && closedCells.Contains(nodeToCheck.parent))
                            {
                                nodeToCheck.UpdateGCost(currentNode);
                                //Debug.Log("updated node");
                            }
                            
                        }  

                    }
                }

            }
            
            // foreach(AStarNode node in openCells)
            // {
            //     Debug.DrawRay(node.position, Vector2.up * 0.25f, Color.green, 10f);
            // }

            // foreach(AStarNode node in closedCells)
            // {
            //     Debug.DrawRay(node.position, Vector2.up * 0.25f, Color.red, 10f);
            // }

        }
        else
        {
            Debug.Log("Target in invalid location.");
        }
    }

    private void StackPath(AStarNode node)
    {
        if(node.parent != null)
        {
            path.Push(node.position);
            StackPath(node.parent);
        }
    }


    private void DrawPath(AStarNode node)
    {
        if(node.parent != null)
        {
            Debug.DrawLine(node.position, node.parent.position, Color.cyan, 10f);
            DrawPath(node.parent);
        }
    }

    private void CloseCell(AStarNode cell)
    {
        closedCells.Add(cell);
        openCells.Remove(cell);
    }

    // create cells around a source cell
    // this is useful for making new cells only when needed, instead of pregenerating too many
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

        Debug.Log("is the source cell indexable? " + createdCells.ContainsKey(grid.WorldToCell(source.position)));

        for(int i = 0; i < positionsToCheck.Length; i++)
        {
            if(!createdCells.ContainsKey(grid.WorldToCell(positionsToCheck[i])))
            {
                // if there is a pathable tile there then open it up
                if(IsValidTarget(positionsToCheck[i]))
                {
                    AStarNode nodeToAdd = new AStarNode(positionsToCheck[i], startNode, endPos);
                    createdCells.Add(grid.WorldToCell(nodeToAdd.position), nodeToAdd);
                    openCells.Add(nodeToAdd);
                    
                }
            }
            else
            {
                Debug.Log("already a node there: " + positionsToCheck[i]);
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
        parent = startNode;

        g_cost = Vector3.Distance(this.position, startNode.position) + startNode.g_cost;
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

    public void UpdateGCost(AStarNode newStartNode)
    {
        parent = newStartNode;
        g_cost = Vector3.Distance(this.position, newStartNode.position) + newStartNode.g_cost;
        f_cost = h_cost + g_cost;
    }
}

