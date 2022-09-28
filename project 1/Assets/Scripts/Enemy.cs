using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // FIELDS

    public GameObject target; // what they are trying to attack
    public float health;
    public float speed;
    private Vector2 velocity;
    private AStar astar;
    private bool hasDestination = false;
    public float targetCheckRate;
    private float lastTimeTargetChecked; // how many seconds before next check

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        astar = GetComponent<AStar>();
        lastTimeTargetChecked = Time.time- targetCheckRate;
    }

    // Update is called once per frame
    void Update()
    {

        // wait a little while before checking for the enemy position again as to not overburden
        // the cpu with thousands of a* calls
        if(Time.time - lastTimeTargetChecked > targetCheckRate)
        {
            SetDestination(target.transform.position);
            lastTimeTargetChecked = Time.time;
        }
        
        TryMoveOnPath();
    }

    // METHODS

    // the private method that handles the enemy's velocity
    private void TryMoveOnPath()
    {
        
        if(hasDestination && astar.path.Count > 0)
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
                    //Debug.Log("reached destination");
                }
                
            }
            // else move to the path node
            else
            {
                velocity = astar.path.Peek() - new Vector2(this.transform.position.x, this.transform.position.y);

                this.GetComponent<AnimActionPlayer>().PlayAction("walk");
            }
        }
        else
        {
            velocity = Vector2.zero;
            this.GetComponent<AnimActionPlayer>().StopAnimaction("walk");
        }

        this.transform.position += new Vector3(velocity.x, velocity.y, 0).normalized * speed * Time.deltaTime;
    }

    // the outward facing method that sets up where to go
    public void SetDestination(Vector2 position)
    {
        hasDestination = true;

        // pathfind the position

        astar.PathTo(position);
        
        // adjust sprite
        GetComponent<SpriteRenderer>().flipX = position.x > this.transform.position.x;
    }

}
