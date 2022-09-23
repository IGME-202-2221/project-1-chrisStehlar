using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // FIELDS

    //public Vector2 direction = Vector2.right;
    public float speed = 1f;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        if(this.GetComponent<AABBCollider>())
        {
            this.GetComponent<AABBCollider>().OnIntersect += TryToDamage;
        }
    }

    void Update()
    {
        this.transform.position += new Vector3(transform.right.x, transform.right.y, 0).normalized * speed * Time.deltaTime;
    }


    // METHODS

    private void TryToDamage(AABBCollider target)
    {
        if(!target.GetComponent<PlayerController>())
        {
            Debug.Log("just shot " + target);
            Destroy(this.gameObject);
        }
        
    }
}
