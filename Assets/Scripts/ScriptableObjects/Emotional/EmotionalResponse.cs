// EmotionalResponse.cs
using System;
using UnityEngine;

[Serializable]
public class EmotionalResponse
{
    public EmotionType emotion;
    [TextArea(2, 4)]
    public string phrase;
    public float successModifier; // exito += successModifier
}