using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // FIELDS

    public float speed;
    private Vector2 velocity;

    public Texture2D cursorTex;

    public Gun weapon;
    private Gun weaponInstance;
    
    // MONO

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.Auto);

        PickupWeapon(weapon);
    }

    void Update()
    {
        CheckForMovement();

        CheckMouse();

        this.transform.position += new Vector3(velocity.x, velocity.y, 0).normalized * speed * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            //this.GetComponent<Gun>().TryToShoot(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            weaponInstance.TryToShoot(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        }

    }


    // METHODS

    public void PickupWeapon(Gun target)
    {
        if(weapon.transform.IsChildOf(this.transform))
            Destroy(weapon.gameObject);     // out with the old weapon
        weapon = target;                // in with the new (then it gets updated in CheckMouse())

        if(!weapon.transform.IsChildOf(this.transform))
        {
            weaponInstance = Instantiate(weapon.gameObject, this.transform.position, this.transform.rotation, this.transform).GetComponent<Gun>();
        }
    }

    private void SetWeaponOrientation(Vector2 direction, bool flip)
    {
        // switch sides that the gun is on based on the mouse position
        if(flip)
            weaponInstance.transform.localPosition = new Vector2(-weaponInstance.offset.x, weaponInstance.offset.y);
        else
            weaponInstance.transform.localPosition = new Vector2(weaponInstance.offset.x, weaponInstance.offset.y);
        // flip the sprite upside down on the other side so that it looks right
        weaponInstance.GetComponent<SpriteRenderer>().flipY = flip;
        
        // make the gun face the mouse
        weaponInstance.transform.right = direction;
    }

    // checks to see where the mouse is in order to orient the player correctly
    private void CheckMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        this.GetComponent<SpriteRenderer>().flipX = mousePos.x < this.transform.position.x;

        SetWeaponOrientation(Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position, mousePos.x < this.transform.position.x);
    }

    // gets input and sets the velocity accordingly
    private void CheckForMovement()
    {
        velocity = Vector2.zero; // reset velocity per check or else it accelerates

        if(Input.GetKey(KeyCode.W))
        {
            velocity += Vector2.up;
        }

        if(Input.GetKey(KeyCode.S))
        {
            velocity -= Vector2.up;
        }

        if(Input.GetKey(KeyCode.A))
        {
            velocity -= Vector2.right;
        }

        if(Input.GetKey(KeyCode.D))
        {
            velocity += Vector2.right;
        }
    }
}
