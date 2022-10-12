using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// not to be confused with obstacle, barricades are what the monsters come through
public class Barricade : MonoBehaviour
{
    // FIELDS

    public int health; // stages.length
    public Sprite[] stages;
    public float repairDelay; // how long in between repairs
    private float lastRepair;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        lastRepair = Time.time;
        this.GetComponentInChildren<Interactable>().OnInteract += PlayerRepair;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {   
            TakeDamage(1);
        }
        // else if(Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     Repair(1);
        // }
    }

    // METHODS

    private void PlayerRepair(PlayerController player)
    {
        if(Time.time - lastRepair > repairDelay && health < stages.Length)
        {
            Repair(1);
            lastRepair = Time.time;
        }
    }

    public void Repair(int howMuch)
    {
        health += howMuch;
        if(health > stages.Length - 1) // keep in range of sprites
        {
            health = stages.Length - 1;
        }
        UpdateSprite();   
    }

    public void TakeDamage(int howMuch)
    {
        health -= howMuch;
        if(health < 0)  // keep in range of sprites
        {   
            health = 0;
        }
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        this.GetComponent<SpriteRenderer>().sprite = stages[health];
    }

}
