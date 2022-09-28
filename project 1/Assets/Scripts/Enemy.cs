using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // FIELDS

    public GameObject target; // what they are trying to attack
    public float speed;
    private Vector2 velocity;
    private AStar astar;
    private bool hasDestination = false;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        astar = GetComponent<AStar>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            SetDestination(target.transform.position);
        }
        
        TryMoveOnPath();
    }

    // METHODS

    // the private method that handles the enemy's velocity
    private void TryMoveOnPath()
    {
        if(hasDestination)
        {
            // if at a path node
            if(Vector2.Distance(this.transform.position, astar.path.Peek()) < 0.1f)
            {
                if(astar.path.Count > 1)
                {   
                    astar.path.Pop();
                }
                else
                {
                    hasDestination = false;
                    Debug.Log("reached destination");
                }
                
            }
            // else move to the path node
            else
            {
                velocity = astar.path.Peek() - new Vector2(this.transform.position.x, this.transform.position.y);
            }
        }
        else
        {
            velocity = Vector2.zero;
        }

        this.transform.position += new Vector3(velocity.x, velocity.y, 0).normalized * speed * Time.deltaTime;
    }

    // the outward facing method that sets up where to go
    public void SetDestination(Vector2 position)
    {
        hasDestination = true;

        // pathfind the position

        astar.PathTo(position);
    }

}
