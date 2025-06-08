#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Custom drawer for SmartReference<T>. Displays a boxed field with value mode selection,
/// inline or reference input, type info, and a live resolved value preview.
/// </summary>
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

        string friendlyType = GetResolvedTypeName(property);
        Debug.Log($"Type key for color lookup: '{friendlyType}'");
        Color boxColor = SmartVariablePreferences.GetColor(friendlyType);

        float boxPadding = 4f;
        Rect boxRect = new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label));
        // Draw background color (box)
        EditorGUI.DrawRect(boxRect, boxColor * new Color(1f, 1f, 1f, 0.15f));// Transparent tint
        GUI.Box(boxRect, GUIContent.none);

        // Label
        string resolvedLabel = $"{label.text} ({GetResolvedTypeName(property)})";
        Rect labelRect = new Rect(position.x + boxPadding, position.y + boxPadding, position.width, lineHeight);
        EditorGUI.LabelField(labelRect, new GUIContent(resolvedLabel));

        // Value mode dropdown
        Rect fieldRect = new Rect(labelRect.x, labelRect.y + lineHeight + spacing, labelRect.width, lineHeight);
        EditorGUI.PropertyField(fieldRect, modeProp, new GUIContent("Value Mode"));

        fieldRect.y += lineHeight + spacing;

        // Inline or reference input
        if ((ValueSourceMode)modeProp.enumValueIndex == ValueSourceMode.Inline)
        {
            EditorGUI.PropertyField(fieldRect, inlineValueProp, new GUIContent("Inline Value"));
        }
        else
        {
            EditorGUI.PropertyField(fieldRect, variableProp, new GUIContent("Reference"));

            if (variableProp.objectReferenceValue == null)
            {
                fieldRect.y += lineHeight + spacing;
                EditorGUI.HelpBox(fieldRect, "Reference is null!", MessageType.Warning);
            }
        }

        //Resolved Value Preview
        fieldRect.y += lineHeight + spacing;
        string resolvedValue = TryGetResolvedValue(property);
        EditorGUI.LabelField(fieldRect, $"🔎 Resolved Value: {resolvedValue}");

        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty modeProp = property.FindPropertyRelative("mode");
        SerializedProperty variableProp = property.FindPropertyRelative("variable");

        float height = EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
        bool isReference = (ValueSourceMode)modeProp.enumValueIndex == ValueSourceMode.Reference;
        bool isNull = variableProp.objectReferenceValue == null;

        if (isReference && isNull)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        return height + 8f; // padding
    }

    private static Dictionary<Type, PropertyInfo> cachedValueProps = new();


    private string TryGetResolvedValue(SerializedProperty property)
    {
        SerializedProperty modeProp = property.FindPropertyRelative("mode");
        SerializedProperty inlineValueProp = property.FindPropertyRelative("inlineValue");
        SerializedProperty variableProp = property.FindPropertyRelative("variable");

        // Inline Mode
        if ((ValueSourceMode)modeProp.enumValueIndex == ValueSourceMode.Inline)
        {
            switch (inlineValueProp.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return inlineValueProp.intValue.ToString();
                case SerializedPropertyType.Float:
                    return inlineValueProp.floatValue.ToString("0.###");
                case SerializedPropertyType.Boolean:
                    return inlineValueProp.boolValue.ToString();
                case SerializedPropertyType.String:
                    return inlineValueProp.stringValue;
                case SerializedPropertyType.Vector2:
                    return inlineValueProp.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return inlineValueProp.vector3Value.ToString();
                case SerializedPropertyType.ObjectReference:
                    return inlineValueProp.objectReferenceValue ? inlineValueProp.objectReferenceValue.name : "null";
                default:
                    return $"({inlineValueProp.propertyType})";
            }
        }

        // Reference Mode
        UnityEngine.Object variableObj = variableProp.objectReferenceValue;
        if (variableObj == null) return "null";

        Type variableType = variableObj.GetType();

        if (!cachedValueProps.TryGetValue(variableType, out PropertyInfo valueProp))
        {
            valueProp = variableType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            cachedValueProps[variableType] = valueProp;
        }

        if (valueProp != null)
        {
            object value = valueProp.GetValue(variableObj);
            return value?.ToString() ?? "null";
        }

        return "(Value not accessible)";
    }

    private string GetResolvedTypeName(SerializedProperty property)
    {
        SerializedProperty modeProp = property.FindPropertyRelative("mode");
        SerializedProperty inlineValueProp = property.FindPropertyRelative("inlineValue");
        SerializedProperty variableProp = property.FindPropertyRelative("variable");

        if ((ValueSourceMode)modeProp.enumValueIndex == ValueSourceMode.Inline)
        {
            return inlineValueProp.propertyType.ToString();
        }

        UnityEngine.Object variableObj = variableProp.objectReferenceValue;
        if (variableObj == null)
            return "null";

        Type variableType = variableObj.GetType();

        if (!cachedValueProps.TryGetValue(variableType, out PropertyInfo valueProp))
        {
            valueProp = variableType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            cachedValueProps[variableType] = valueProp;
        }

        string rawName = valueProp?.PropertyType.Name ?? "Unknown";

        // Translate to friendly name
        return friendlyTypeNames.TryGetValue(rawName, out string friendly)
            ? friendly
            : rawName;
    }

    /// <summary>
    /// Maps raw .NET type names to user-friendly names for display and color lookup.
    /// </summary>
    private static readonly Dictionary<string, string> friendlyTypeNames = new()
    {
        { "Single", "Float" },
        { "Int32", "Int" },
        { "Boolean", "Bool" },
        { "String", "String" },
        { "Vector2", "Vector2" },
        { "Vector3", "Vector3" },
        { "Object", "Object" }
    };
}

#endif