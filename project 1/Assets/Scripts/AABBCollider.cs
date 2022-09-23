using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBCollider : MonoBehaviour
{
    // FIELDS

    public float scale = 1f;

    [HideInInspector]
    public Bounds bounds;

    public delegate void AABBCollision(AABBCollider collider); // for collision
    public event AABBCollision OnIntersect; // subscribe other scripts to this

    private bool intersecting;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        intersecting = false;
    }

    // Update is called once per frame
    void Update()
    {
        bounds = GetComponentInParent<SpriteRenderer>().bounds;     // check bounds per frame because it is passed by value

        // set up the scale
        Vector3 centerOut = bounds.max - bounds.center; // this is the "unit vector" for the bounds corners

        bounds.max = bounds.center;     // set the bounds to the center
        bounds.min = bounds.max;        // copy the position of bounds.max so they can expand outward from each other

        bounds.max += centerOut * scale;  // scale the bounds outwards
        bounds.min -= centerOut * scale;

        // check for collision
        CheckForCollisions();

        // update behavior based on collision
        // if(intersecting)
        // {
        //     GetComponentInParent<SpriteRenderer>().color = Color.red;
        // }
        // else
        // {
        //     GetComponentInParent<SpriteRenderer>().color = Color.white;
        // }
        // THIS WAS FOR TESTING, USE THE EVENT SYSTEM FROM NOW ON
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(153/255f, 255/255f, 102/255f); // light green

        Vector3 boundsWidth = new Vector3(bounds.max.x - bounds.min.x, 0, 0);
        Vector3 boundsHeight = new Vector3(0, bounds.max.y - bounds.min.y, 0);

        // top left to top right
        Gizmos.DrawLine(bounds.max - boundsWidth, bounds.max);
        // top right to bot right
        Gizmos.DrawLine(bounds.max, bounds.min + boundsWidth);
        // bot right to bot left
        Gizmos.DrawLine(bounds.min + boundsWidth, bounds.min);
        // bot left to top left
        Gizmos.DrawLine(bounds.min, bounds.max - boundsWidth);
    }

    // METHODS

    private void CheckForCollisions()
    {
        foreach(AABBCollider other in GameObject.FindObjectsOfType<AABBCollider>())
        {
            if(other != this)
            {
                if(other.bounds.min.x < bounds.max.x && other.bounds.max.x > bounds.min.x && other.bounds.max.y > bounds.min.y && other.bounds.min.y < bounds.max.y)
                {
                    //Debug.DrawRay(bounds.center, Vector2.up, Color.blue, .01f);
                    intersecting = true;
                    
                    if(OnIntersect != null)
                        OnIntersect(other);

                    return; // found a collision, now it's over
                }
            }
        }

        intersecting = false; // if there was no valid collisions, then intersecting is not true
    }
}
