// NPCDialogueSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogueSO", menuName = "Dialogue/NPC Dialogue")]
public class NPCDialogueSO : ScriptableObject
{
    public string speakerName;
    public Sprite speakerSprite;
    public DialogueBlock[] blocks;
}

