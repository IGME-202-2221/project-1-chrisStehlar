using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGun : MonoBehaviour
{
    // FIELDS

    public Gun gun;
    public int pointsToBuy;
    public int pointsToBuyAmmo;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Interactable>().OnInteract += GiveGun;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // METHODS

    public void GiveGun(PlayerController who)
    {
        if(who.points > pointsToBuy)
        {
            who.points -= pointsToBuy;
            who.PickupWeapon(gun);
        }

    }
}
