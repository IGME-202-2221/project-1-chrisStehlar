using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // FIELDS

    public Bullet bullet;
    public float shotCooldown;

    private float lastShotTime;

    public float shotCount;
    public Vector2 offset;
    public float shotAngle; // all the bullets fit in this angle

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
            float currentShotAngle =  0 - (shotAngle / 2);

            for(int i = 0; i < shotCount; i++)
            {
                GameObject bulletObj = Instantiate(bullet.gameObject, this.transform.position, this.transform.rotation);

                bulletObj.transform.right = Quaternion.Euler(0 ,0, currentShotAngle) * direction; // the direction vector with an offset
                
                currentShotAngle += (shotAngle / shotCount);    // increase the angle offset
            }

            lastShotTime = Time.time;
        }
    }

}
