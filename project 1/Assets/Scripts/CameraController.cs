using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // FIELDS

    public GameObject target;
    public Vector3 offset;

    private bool isWandering;
    public float speed;
    private bool hasPath = false;
    private Vector2 velocity;
    private AStar astar;

    public Vector2[] cameraWanderNodes;
    private int wanderNodeIndex = 0;


    // MONO

    // Start is called before the first frame update
    void Start()
    {
        astar = GetComponent<AStar>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isWandering)
        {   
            // track the target
            this.transform.position = target.transform.position;
            this.transform.Translate(offset);
        }
        else
        {
            Wander();
        }
        
    }

    // METHODS

    private void Wander()
    {
        if(!hasPath)
        {
            // get the next path node
            if(wanderNodeIndex >= cameraWanderNodes.Length - 1)
            {
                wanderNodeIndex = 0;
            }
            else
            {
                wanderNodeIndex++;
            }

            // move there
            astar.PathTo(cameraWanderNodes[wanderNodeIndex]);
            hasPath = true;
        }
        else
        {
            // check if at the path node
            if(Vector2.Distance(this.transform.position, astar.path.Peek()) < 1)
            {
                
                astar.path.Pop();

                if(astar.path.Count <= 0)
                {
                    hasPath = false;
                } 
                
            }
            // if not then move towards it
            else
            {
                velocity = (astar.path.Peek() - (Vector2)this.transform.position).normalized;
                this.transform.position += (Vector3)velocity * speed * Time.deltaTime;
            }

            
        }
    }

    public void TriggerWander(bool value)
    {
        isWandering = value;

        if(isWandering)
        {
            // move to the closest wander node
            Vector2 wanderSpot = new Vector2(float.MaxValue, float.MaxValue); // some insanely large value

            for(int i = 0; i < cameraWanderNodes.Length; i++)
            {
                // get the closest wander node so that we can move there and then start moving on the path
                if(Vector2.Distance(this.transform.position, wanderSpot) > Vector2.Distance(this.transform.position, cameraWanderNodes[i]))
                {
                    wanderSpot = cameraWanderNodes[i];
                    wanderNodeIndex = i;
                }
            }

            hasPath = true;
            astar.PathTo(wanderSpot);
        }
        
    }

}
