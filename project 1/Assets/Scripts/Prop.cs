using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    // FIELDS

    private AABBCollider col;
    private int zOrder;
    public float fadeAmount;    // how much to fade when something walks in front of it
    private AABBCollider[] childCols;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        col = this.GetComponent<AABBCollider>();
        col.OnIntersect += RenderInFront;
        col.NullCollision += RenderNormally;

        zOrder = this.GetComponent<SpriteRenderer>().sortingOrder;

        childCols = GetComponentsInChildren<AABBCollider>();
        //Debug.Log(childCol.gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // METHODS

    // called when a collision happens with the sprite part (not the actually solid child)
    private void RenderInFront(AABBCollider otherCol)
    {
        if(otherCol.GetComponent<SpriteRenderer>())
        {
            SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.5f);

            renderer.sortingOrder = otherCol.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
    }

    // called when there are no collisions on this object
    private void RenderNormally()
    {
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);

        renderer.sortingOrder = zOrder;
    }

}
