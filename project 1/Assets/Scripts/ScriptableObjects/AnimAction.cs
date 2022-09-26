using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animaction")]
public class AnimAction : ScriptableObject
{
    // FIELDS

    public string actionName;   // generic name to be referenced in state machines (ex. "shoot", "attack", "walk", "die")
    public Sprite baseSprite; // what this thing goes back to when it isnt animated
    public Frame[] frames;
    
}

[System.Serializable]
public struct Frame
{
    public Sprite sprite;
    public float duration;  // in milliseconds

    public Frame(Sprite sprite, float duration)
    {
        this.sprite = sprite;
        this.duration = duration;
    }
}
