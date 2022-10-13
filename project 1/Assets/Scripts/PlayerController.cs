using System.Collections;
using System.Collections.Generic;
using System;
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

    public int points;
    public float pointsIncrementSpeed;
    private float pointsFloat;

    public Texture2D cursorTex;

    public Gun weapon; // the prefab
    private Gun weaponInstance; // the copy (edit this)
    public int ammo;

    private bool inInteractable = false;
    private TextMeshProUGUI interactableText;

    [Header("UI")]
    public Image[] healthOverlays;
    public Image ammoIcon;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI pointsText;
    
    // MONO

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<AABBCollider>().OnIntersect += OnCollision;
        Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.Auto);

        interactableText = GameObject.Find("Canvas").transform.Find("HUD - Panel").transform.Find("interactionText").GetComponent<TextMeshProUGUI>();

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
        CheckForInteractions();
        ApplyMovement();
    }


    // METHODS

    private void CheckForInteractions()
    {
        if(!inInteractable)
        {
            interactableText.text = "";
        }

        inInteractable = false; // set false at first and then check to update the interactable text in the OnCollision method
    }

    private void CheckUI()
    {
        ammoIcon.sprite = weaponInstance.uiAmmoIcon;
        ammoText.text = $"{weaponInstance.ammo} | {ammo}";

        // increase the points with a little animation
        int currentPointsInText = Int32.Parse(pointsText.text);
        if(currentPointsInText < points)
        {
            pointsFloat += pointsIncrementSpeed * Time.deltaTime;
        }
        else if(currentPointsInText > points)
        {
            pointsFloat -= pointsIncrementSpeed * Time.deltaTime;
        }
        pointsText.text = (int)pointsFloat + "";

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
        // only take damage if you are alive
        if(health > 0)
        {
            if(Vector2.Distance(this.transform.position, attacker.transform.position) < 1)
            {
                //Debug.Log("OUCH");
                health -= 1;
            }

            if(health <= 0) // death
            {
                Camera.main.GetComponent<CameraController>().TriggerWander(true);
            }
        }
        // else // already dead
        // {

        // }
        
    }

    private void OnCollision(AABBCollider col)
    {

        if(col.isSolid)
        {
            //Debug.Log("hit a solid");

            velocity.Normalize();

            Vector2 colCenter = Vector2.Lerp(col.bounds.max, col.bounds.min, 0.5f);

            // if coming in from the side
            if(this.transform.position.x > col.bounds.max.x || this.transform.position.x < col.bounds.min.x)
            {
                if(colCenter.x > this.transform.position.x)
                {
                    velocity -= Vector2.right;
                }
                else
                {
                    velocity += Vector2.right;
                }
            }
            // coming in from the top or bottom
            else
            {
                if(colCenter.y > this.transform.position.y)
                {
                    velocity -= Vector2.up;
                }
                else
                {
                    velocity += Vector2.up;
                }
            }
            
        }

        if(col.GetComponent<Interactable>())
        {
            inInteractable = true;
        }
    }

    public void PickupWeapon(Gun target)
    {
        //if(weapon.transform.IsChildOf(this.transform))
        //    Destroy(weapon.gameObject);     // out with the old weapon
        if(weaponInstance != null)
            Destroy(weaponInstance.gameObject);
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
