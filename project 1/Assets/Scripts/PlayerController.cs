using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // FIELDS

    public float speed;
    private Vector2 velocity = Vector2.zero;
    public float health = 4;
    public float healthRegenSpeed; // how many seconds per point of health

    public Texture2D cursorTex;

    public Gun weapon; // the prefab
    private Gun weaponInstance; // the copy (edit this)
    public int ammo;

    [Header("UI")]
    public Image[] healthOverlays;
    public Image ammoIcon;
    public TextMeshProUGUI ammoText;
    
    // MONO

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<AABBCollider>().OnIntersect += OnCollision;
        Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.Auto);

        PickupWeapon(weapon);
        ammo = weaponInstance.maxAmmo;
    }

    void Update()
    {
        CheckForMovement();

        CheckMouse();
        
        // attack
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            weaponInstance.TryToShoot(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            weaponInstance.Reload();
            ammo -= weaponInstance.maxClip;
        }

        CheckHealth();
        CheckUI();
    }

    void LateUpdate()
    {
        ApplyMovement();
    }


    // METHODS

    private void CheckUI()
    {
        ammoIcon.sprite = weaponInstance.uiAmmoIcon;
        ammoText.text = $"{weaponInstance.ammo} | {ammo}";
    }

    private void CheckHealth()
    {
        if(health < 4 && health > 0)
        {
            health += healthRegenSpeed * Time.deltaTime;
        }

        // make the health effects fade out as health regens
        healthOverlays[0].color = new Color(1, 1, 1, 1 - (health - 3));
        healthOverlays[1].color = new Color(1, 1, 1, 1 - (health - 2));
        healthOverlays[2].color = new Color(1, 1, 1, 1 - (health - 1));
    }

    public void TryToTakeDamage(Enemy attacker)
    {
        if(Vector2.Distance(this.transform.position, attacker.transform.position) < 1)
        {
            Debug.Log("OUCH");
            health -= 1;
        }
    }

    private void OnCollision(AABBCollider col)
    {
        if(col.isSolid)
        {
            Debug.Log("hit a solid");
            velocity.Normalize();
            velocity -= (Vector2)(col.transform.position - this.transform.position).normalized * 10; // kind of works but jittery

            // if(col.transform.position.x > this.transform.position.x)
            // {
            //     velocity -= Vector2.right;
            // }
            
        }
    }

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

    // called in late update, after any collisions may have occurred
    private void ApplyMovement()
    {
        // apply the movement
        this.transform.position += new Vector3(velocity.x, velocity.y, 0).normalized * speed * Time.deltaTime;

        velocity = Vector2.zero; // reset velocity per check or else it accelerates
    }

    // gets input and sets the velocity accordingly
    private void CheckForMovement()
    {

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

        // play walk animation
        if(this.GetComponent<AnimActionPlayer>())
        {
            if(velocity.sqrMagnitude > 0)
            {
                this.GetComponent<AnimActionPlayer>().PlayAction("walk");
            }
            else
            {
                this.GetComponent<AnimActionPlayer>().StopAnimaction("walk");
            }
        }
        
    }
}
