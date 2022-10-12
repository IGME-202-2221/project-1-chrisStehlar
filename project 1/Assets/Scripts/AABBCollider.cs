using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBCollider : MonoBehaviour
{
    // FIELDS

    public Vector2 scale = new Vector2(1, 1);
    public Vector2 offset;

    [HideInInspector]
    public Bounds bounds;

    public bool isSolid = false;

    public delegate void AABBCollision(AABBCollider collider); // for collision
    public event AABBCollision OnIntersect; // subscribe other scripts to this

    public delegate void AABBNullCollision(); // to say there are no collisions
    public event AABBNullCollision NullCollision; // called when there are no collisions

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bounds = GetComponentInParent<SpriteRenderer>().bounds;     // check bounds per frame because it is passed by value

        // set up the scale
        Vector3 centerOut = bounds.max - bounds.center; // this is the "unit vector" for the bounds corners

        bounds.max = bounds.center;     // set the bounds to the center
        bounds.min = bounds.max;        // copy the position of bounds.max so they can expand outward from each other

        bounds.max += (Vector3)(centerOut * scale);  // scale the bounds outwards
        bounds.min -= (Vector3)(centerOut * scale);

        bounds.max += (Vector3)offset;      // adjust the bounds to the right spot
        bounds.min += (Vector3)offset;

        // check for collision
        CheckForCollisions();
        
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
        bool intersected = false;
        foreach(AABBCollider other in GameObject.FindObjectsOfType<AABBCollider>())
        {
            if(other != this && !other.transform.IsChildOf(this.transform))
            {
                if(other.bounds.min.x < bounds.max.x && other.bounds.max.x > bounds.min.x && other.bounds.max.y > bounds.min.y && other.bounds.min.y < bounds.max.y)
                {
                    //Debug.DrawRay(bounds.center, Vector2.up, Color.blue, .01f);
                    
                    if(OnIntersect != null)
                        OnIntersect(other);

                    intersected = true;
                    //return; // found a collision, now it's over
                }
            }
        }

        if(NullCollision != null && !intersected)
            NullCollision();

    }
}
