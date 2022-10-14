using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // FIELDS

    [HideInInspector]
    public GameObject target; // what they are trying to attack
    public float health;
    public int pointsOnHit;
    public int pointsOnKill;
    public float speed;
    private Vector2 velocity;
    private AStar astar;
    private bool hasDestination = false;
    public float targetCheckRate;
    private float lastTimeTargetChecked; // how many seconds before next check
    private bool attacking = false;
    private float timeStartedAttacking;
    private bool pastBarricade;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<AABBCollider>().OnIntersect += OnCollision;

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
                if(target.GetComponent<AABBCollider>())
                {
                    // path towards the center of the bounding box (the legs of the character)
                    SetDestination(Vector2.Lerp(target.GetComponent<AABBCollider>().bounds.max, target.GetComponent<AABBCollider>().bounds.min, 0.5f));
                }
                else
                {
                    SetDestination(target.transform.position);
                }

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

    void LateUpdate()
    {
        ApplyMovement();
    }

    // METHODS

    // called in late update, after any collisions may have occurred
    private void ApplyMovement()
    {
        // apply the movement
        //this.transform.position += new Vector3(velocity.x, velocity.y, 0).normalized * speed * Time.deltaTime;
        this.transform.Translate(new Vector3(velocity.x, velocity.y, 0).normalized * speed * Time.deltaTime);

        velocity = Vector2.zero; // reset velocity per check or else it accelerates
    }

    private void OnCollision(AABBCollider col)
    {
        // avoid bumping/clipping into each other
        if(col.GetComponent<Enemy>())
        {
            //Debug.DrawLine(this.transform.position, col.transform.position, Color.magenta, 0.1f);
            Vector2 collisionVector = this.transform.position - col.transform.position;
            this.transform.Translate(collisionVector.normalized  * Time.deltaTime);
        }

        CheckForBarricade(col);
    }

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
                //Debug.Log("attack now");

                // if the target is the palyer
                if(target.GetComponent<PlayerController>())
                {
                    target.GetComponent<PlayerController>().TryToTakeDamage(this);
                } 
                // if the target is the barricade then try and destroy it then target the player
                else if(target.GetComponent<Barricade>())
                {
                    target.GetComponent<Barricade>().TakeDamage(1);
                    if(target.GetComponent<Barricade>().health <= 0)
                    {
                        target = GameObject.Find("player");
                    }
                }
            }
        }
    }

    public void TakeDamage(int howMuch)
    {
        health -= howMuch;

        GameObject.Find("player").GetComponent<PlayerController>().points += pointsOnHit;

        if(health < 1)
        {
            this.GetComponent<AnimActionPlayer>().StopAnimaction(); // stop whatever was happening
            this.GetComponent<AnimActionPlayer>().PlayAction("die");    // play death animaction
            Destroy(this.gameObject, this.GetComponent<AnimActionPlayer>().GetActionLength("die"));

            this.GetComponent<SpriteRenderer>().sortingOrder = 0; // go behind the alive creatures

            GameObject.Find("player").GetComponent<PlayerController>().points += pointsOnKill;

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

    }

    private void CheckForBarricade(AABBCollider col)
    {
        if(!pastBarricade)
        {
            if(col.GetComponent<Barricade>() && col.GetComponent<Barricade>().health > 0)
            {
                //Debug.Log("im at the barricade");
                target = col.gameObject;
                pastBarricade = true;
            }
        }
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
