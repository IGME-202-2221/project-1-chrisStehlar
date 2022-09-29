using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // FIELDS

    [HideInInspector]
    public GameObject target; // what they are trying to attack
    public float health;
    public float speed;
    private Vector2 velocity;
    private AStar astar;
    private bool hasDestination = false;
    public float targetCheckRate;
    private float lastTimeTargetChecked; // how many seconds before next check
    private bool attacking = false;
    private float timeStartedAttacking;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        astar = GetComponent<AStar>();
        lastTimeTargetChecked = Time.time- targetCheckRate;

        target = GameObject.Find("player");
    }

    // Update is called once per frame
    void Update()
    {
        if(health > 0)
        {
            // wait a little while before checking for the enemy position again as to not overburden
            // the cpu with thousands of a* calls
            if(Time.time - lastTimeTargetChecked > targetCheckRate)
            {
                SetDestination(target.transform.position);
                lastTimeTargetChecked = Time.time;
            }
            
            // if in attacking range then attack
            if(Vector2.Distance(target.transform.position, this.transform.position) < astar.minDistanceToTarget)
            {
                //Debug.Log("attack");
                Attack();
            }
            else
            {
                this.GetComponent<AnimActionPlayer>().StopAnimaction("attack");

                TryMoveOnPath();
            }
            
        }

    }

    // METHODS

    private void Attack()
    {
        if(!attacking)
        {
            // start animation
            this.GetComponent<AnimActionPlayer>().StopAnimaction();
            this.GetComponent<AnimActionPlayer>().PlayAction("attack");
            attacking = true;
            timeStartedAttacking = Time.time;
        }
        else
        {
            if(Time.time - timeStartedAttacking > this.GetComponent<AnimActionPlayer>().GetActionLength("attack"))
            {
                attacking = false;
                Debug.Log("attack now");
            }
        }
    }

    public void TakeDamage(int howMuch)
    {
        health -= howMuch;

        if(health < 1)
        {
            this.GetComponent<AnimActionPlayer>().StopAnimaction(); // stop whatever was happening
            this.GetComponent<AnimActionPlayer>().PlayAction("die");    // play death animaction
            Destroy(this.gameObject, this.GetComponent<AnimActionPlayer>().GetActionLength("die"));

            this.GetComponent<SpriteRenderer>().sortingOrder = 0; // go behind the alive creatures

            Destroy(this.GetComponent<AABBCollider>()); // delete the collider because collisions check even when disabled as of right now
        }
        
    }

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
