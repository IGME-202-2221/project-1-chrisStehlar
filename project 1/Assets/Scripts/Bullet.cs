using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // FIELDS

    public Vector2 direction;
    public float speed;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += new Vector3(direction.x, direction.y, 0).normalized * speed * Time.fixedDeltaTime;
    }

    // METHODS
}
