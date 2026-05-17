using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueBlock))]
public class DialogueBlockDrawer : PropertyDrawer
{
    private const float SPACING = 2f;

    private float GetFixedPhraseHeight(SerializedProperty property)
    {
        var fixedPhrase = property.FindPropertyRelative("fixedPhrase");
        string text = fixedPhrase.stringValue;
        float lineH = EditorGUIUtility.singleLineHeight;

        // Calcula altura real del TextArea según el contenido
        GUIStyle style = EditorStyles.textArea;
        float textHeight = style.CalcHeight(new GUIContent(text), EditorGUIUtility.currentViewWidth - 20f);
        return Mathf.Max(lineH * 2, textHeight) + lineH + SPACING; // label + textarea
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var blockType       = property.FindPropertyRelative("blockType");
        var fixedPhrase     = property.FindPropertyRelative("fixedPhrase");
        var characterSprite = property.FindPropertyRelative("characterSprite");
        var emotionalResp   = property.FindPropertyRelative("emotionalResponses");

        float lineH = EditorGUIUtility.singleLineHeight;
        float y = position.y;

        // Block Type
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineH), blockType);
        y += lineH + SPACING;

        // Character Sprite
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineH), characterSprite);
        y += lineH + SPACING;

        bool isFixed = blockType.enumValueIndex == (int)DialogueBlockType.Fixed;

        if (isFixed)
        {
            float h = GetFixedPhraseHeight(property);
            Rect labelRect = new Rect(position.x, y, position.width, lineH);
            Rect textAreaRect = new Rect(position.x, y + lineH + SPACING, position.width, h - lineH - SPACING);

            EditorGUI.LabelField(labelRect, "Fixed Phrase");
            EditorGUI.BeginChangeCheck();
            string updatedPhrase = EditorGUI.TextArea(textAreaRect, fixedPhrase.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                fixedPhrase.stringValue = updatedPhrase;
            }
        }
        else
        {
            float h = EditorGUI.GetPropertyHeight(emotionalResp, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), emotionalResp, new GUIContent("Emotional Responses"), true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var blockType   = property.FindPropertyRelative("blockType");
        var emotionalResp = property.FindPropertyRelative("emotionalResponses");

        float lineH = EditorGUIUtility.singleLineHeight;

        // BlockType + CharacterSprite
        float total = (lineH + SPACING) * 2;

        bool isFixed = blockType.enumValueIndex == (int)DialogueBlockType.Fixed;

        if (isFixed)
            total += GetFixedPhraseHeight(property);
        else
            total += EditorGUI.GetPropertyHeight(emotionalResp, true);

        return total + SPACING;
    }



    
}