#if UNITY_EDITOR
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SmartReference<>), true)]
public class SmartReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty modeProp = property.FindPropertyRelative("mode");
        SerializedProperty inlineValueProp = property.FindPropertyRelative("inlineValue");
        SerializedProperty variableProp = property.FindPropertyRelative("variable");

        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        // Label
        Rect labelRect = new Rect(position.x, position.y, position.width, lineHeight);
        EditorGUI.LabelField(labelRect, label);

        // Mode Dropdown
        Rect modeRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
        EditorGUI.PropertyField(modeRect, modeProp, new GUIContent("Value Mode"));

        // Value Field
        Rect valueRect = new Rect(position.x, modeRect.y + lineHeight + spacing, position.width, lineHeight);

        if ((ValueSourceMode)modeProp.enumValueIndex == ValueSourceMode.Inline)
        {
            EditorGUI.PropertyField(valueRect, inlineValueProp, new GUIContent("Inline Value"));
        }
        else
        {
            EditorGUI.PropertyField(valueRect, variableProp, new GUIContent("Reference"));

            // Show warning if variable is null
            if (variableProp.objectReferenceValue == null)
            {
                Rect warningRect = new Rect(position.x, valueRect.y + lineHeight + spacing, position.width, lineHeight);
                EditorGUI.HelpBox(warningRect, "Reference is null!", MessageType.Warning);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty modeProp = property.FindPropertyRelative("mode");
        SerializedProperty variableProp = property.FindPropertyRelative("variable");

        float height = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;

        bool isReference = (ValueSourceMode)modeProp.enumValueIndex == ValueSourceMode.Reference;
        bool isNull = variableProp.objectReferenceValue == null;

        if (isReference && isNull)
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        return height;
    }
}

#endif