using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // FIELDS

    //public Vector2 direction = Vector2.right;
    public float speed = 1f;
    public float timeToDespawn;
    private float timeSpawned;
    public int damage;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        if(this.GetComponent<AABBCollider>())
        {
            this.GetComponent<AABBCollider>().OnIntersect += TryToDamage;
        }

        timeSpawned = Time.time;
    }

    void Update()
    {
        // move forward constantly
        this.transform.position += new Vector3(transform.right.x, transform.right.y, 0).normalized * speed * Time.deltaTime;

        // despawn if for some reason this bullet goes off into infinity
        if(Time.time - timeSpawned > timeToDespawn)
        {
            Destroy(this.gameObject);
        }
    }


    // METHODS

    private void TryToDamage(AABBCollider target)
    {
        if(!target.GetComponent<PlayerController>() && !target.GetComponent<Bullet>())
        {
            Debug.Log("just shot " + target);

            if(target.GetComponent<Enemy>())
            {
                target.GetComponent<Enemy>().TakeDamage(damage);
            }

            Destroy(this.gameObject);
        }
        
    }
}
