using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimActionPlayer : MonoBehaviour
{
    // FIELDS

    public AnimAction[] animactions;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            PlayAction("shoot");
        }
    }

    // METHODS

    public void PlayAction(string actionName)
    {
        AnimAction actionToPlay = null;
        foreach(AnimAction animaction in animactions)
        {
            if(animaction.actionName == actionName) // name and actionName !=
            {
                actionToPlay = animaction;
            }
        }

        if(actionToPlay == null)
        {
            Debug.LogError("No animaction with name '" + actionName + "'");
        }

        StartCoroutine(PlayAnimaction(actionToPlay));

    }

    private IEnumerator PlayAnimaction(AnimAction animaction)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        for(int i = 0; i < animaction.frames.Length; i++)
        {
            renderer.sprite = animaction.frames[i].sprite;
            yield return new WaitForSeconds(animaction.frames[i].duration);
        }
        
        renderer.sprite = animaction.baseSprite;
    }
}
