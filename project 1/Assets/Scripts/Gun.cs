using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // FIELDS

    public Bullet bullet;
    public float shotCooldown;

    private float lastShotTime;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        lastShotTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // METHODS

    public void TryToShoot(Vector2 direction)
    {
        if(Time.time - lastShotTime > shotCooldown)
        {
            Debug.Log("shoot");
            GameObject bulletObj = Instantiate(bullet.gameObject, this.transform.position, this.transform.rotation);
            bulletObj.transform.right = direction;

            lastShotTime = Time.time;
        }
    }

}
