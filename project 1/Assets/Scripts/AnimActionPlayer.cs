using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimActionPlayer : MonoBehaviour
{
    // FIELDS

    public AnimAction[] animactions;

    private AnimAction playingAnimaction = null;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // METHODS

    public void StopAnimaction(string actionName)
    {
        if(playingAnimaction != null)
        {
            if(playingAnimaction.actionName == actionName)
            {
                GetComponent<SpriteRenderer>().sprite = playingAnimaction.baseSprite;
                playingAnimaction = null;
            }
        }

    }

    public void PlayAction(string actionName)
    {
        if(playingAnimaction == null)
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

            playingAnimaction = actionToPlay;
            StartCoroutine(PlayAnimaction(actionToPlay));
        }

    }

    private IEnumerator PlayAnimaction(AnimAction animaction)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        for(int i = 0; i < animaction.frames.Length; i++)
        {
            if(playingAnimaction != null)
            {
                renderer.sprite = animaction.frames[i].sprite;
                yield return new WaitForSeconds(animaction.frames[i].duration);
            }
            else
            {
                renderer.sprite = animaction.baseSprite;
                break;
            }
            
        }
        
        renderer.sprite = animaction.baseSprite;
        playingAnimaction = null;
    }
}
