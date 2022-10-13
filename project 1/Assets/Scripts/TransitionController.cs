using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    // FIELDS
    public Image panel;
    private bool transitioning = false;
    private Color startColor;
    private Color targetColor;
    //private float speed;
    private float startTransition;
    private float transitionDuration;

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
            panel.color = Color.Lerp(startColor, targetColor, (Time.time - startTransition) / transitionDuration);

            if((Time.time - startTransition) / transitionDuration >= 1f)
            {
                transitioning = false;
            }
        }
    }

    // METHODS

    // called externally
    public void FadeFromTo(Color from, Color to, float howLong)
    {
        targetColor = to;
        startColor = from;

        transitioning = true;

        transitionDuration = howLong;
        startTransition = Time.time;
        
    }
}
