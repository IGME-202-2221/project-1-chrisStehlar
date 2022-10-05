using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    // FIELDS

    public string prompt;
    private TextMeshProUGUI tmpText;

    public delegate void Interaction(PlayerController interactor);
    public event Interaction OnInteract;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        tmpText = GameObject.Find("Canvas").transform.Find("HUD - Panel").transform.Find("interactionText").GetComponent<TextMeshProUGUI>();

        this.GetComponent<AABBCollider>().OnIntersect += ServeInteractable;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // METHODS

    // serves up the prompt and event call if triggered
    private void ServeInteractable(AABBCollider col)
    {
        if(col.GetComponent<PlayerController>())
        {
            tmpText.text = prompt;

            if(Input.GetKeyDown(KeyCode.E))
            {
                if(OnInteract != null)
                {
                    OnInteract(col.GetComponent<PlayerController>());
                }
            }
        }
    }

}
