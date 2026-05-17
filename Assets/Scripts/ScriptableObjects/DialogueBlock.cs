// DialogueBlock.cs
using System;
using UnityEngine;

[Serializable]
public class DialogueBlock
{
    public DialogueBlockType blockType;
    public Sprite characterSprite;
    // Si blockType == Fixed
    [TextArea(2, 4)]
    public string fixedPhrase;

    // Si blockType == Emotional
    public EmotionalResponse[] emotionalResponses;

    public bool TryGetEmotionalResponse(EmotionType emotion, out EmotionalResponse result)
    {
        result = Array.Find(emotionalResponses, r => r.emotion == emotion);
        return result != null;
    }
}