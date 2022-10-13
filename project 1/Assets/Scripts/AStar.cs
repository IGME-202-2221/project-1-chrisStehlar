using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// component responsible for all the pathfinding for npcs

public class AStar : MonoBehaviour
{
    // FIELDS

    // meta info that shouldnt change
    public Grid grid;
    private Tilemap tilemap;
    public TileBase[] pathableTiles;    // a public facing list of pathable tiles
    private HashSet<TileBase> hashPathables;    // a private list of the pathable tiles used to quickly retrieve them with hashing
    public float minDistanceToTarget;
    public int maxPathChecks;

    // for the PathTo function
    private HashSet<AStarNode> openCells = new HashSet<AStarNode>();
    private HashSet<AStarNode> closedCells = new HashSet<AStarNode>();
    private Dictionary<Vector3Int, AStarNode> createdCells = new Dictionary<Vector3Int, AStarNode>(); // convert form world to cell

    [HideInInspector]
    public Stack<Vector2> path;

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
        
    }

    // METHODS

    public bool PathTo(Vector2 targetPos)
    {
        
        if(IsValidTarget(targetPos))
        {
            // pre set up

            createdCells.Clear();
            openCells.Clear();
            closedCells.Clear();

            // commence the pathfinding algorithm

            AStarNode startNode = new AStarNode(grid.CellToWorld(grid.WorldToCell(this.transform.position)) + new Vector3(grid.cellSize.x / 2, grid.cellSize.y / 2, 0));
            createdCells.Add(grid.WorldToCell(startNode.position), startNode);
            openCells.Add(startNode);

            // a* pathing algorithm as found here: https://youtu.be/-L-WgKMFuhE?t=461 (with some tweaks)
            for(int i = 0; i < maxPathChecks; i++)
            {
                AStarNode currentNode = GetLowestFCost();
                CloseCell(currentNode);

                // check if it reached the target
                if(Vector3.Distance(currentNode.position, targetPos) < minDistanceToTarget)
                {
                    path.Clear(); // remove the old path
                    StackPath(currentNode); // save the new one
                    DrawPath(currentNode);
                    return true;
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
        }
        
        return false; // if no path found return false
    }

    // called when the path is found, adds all the locations into a stack
    private void StackPath(AStarNode node)
    {
        if(node.parent != null)
        {
            path.Push(node.position);
            StackPath(node.parent);
        }
    }

    // a debug method to draw the path on a blue line, called when the path is finished
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

    // a search algorithm that finds the lowest f_cost in the open nodes
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

    // checks if the location falls on a walkable tile
    public bool IsValidTarget(Vector2 target)
    {
        Vector3Int intTarget = grid.WorldToCell(target);

        if(hashPathables.Contains(tilemap.GetTile(intTarget)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

// a helper class to handle the internal data for different astar nodes
public class AStarNode
{
    // FIELDS

    public AStarNode parent;
    public Vector3 position;
    public float g_cost; // distance from start node (via it's parent's path back to the start)
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

    // stand in node for trying to get another node with a lower f_cost, used in FindLowestFCost()
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

    // used to reparent this node to the newStartNode which likely has a shorter path
    public void UpdateGCost(AStarNode newStartNode)
    {
        parent = newStartNode;
        g_cost = Vector3.Distance(this.position, newStartNode.position) + newStartNode.g_cost;
        f_cost = h_cost + g_cost;
    }
}

