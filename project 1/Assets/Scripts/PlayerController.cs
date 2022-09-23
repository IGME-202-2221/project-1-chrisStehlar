using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // FIELDS

    public float speed;
    private Vector2 velocity;

    public Texture2D cursorTex;
    
    // MONO

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForMovement();

        CheckMouse();

        this.transform.position += new Vector3(velocity.x, velocity.y, 0).normalized * speed * Time.fixedDeltaTime;

    }

    // METHODS

    // checks to see where the mouse is in order to orient the player correctly
    private void CheckMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        this.GetComponent<SpriteRenderer>().flipX = mousePos.x < this.transform.position.x;
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
