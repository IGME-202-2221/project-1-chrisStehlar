using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    // FIELDS
    public Image panel;
    private bool transitioning = false;
    private Color targetColor;
    //private float speed;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transitioning)
        {
            

            if(panel.color == targetColor)
            {
                transitioning = false;
            }
        }
    }

    // METHODS

    public void FadeFromTo(Color from, Color to, float howLong)
    {
        targetColor = to;
        panel.color = from;
        
    }
}
