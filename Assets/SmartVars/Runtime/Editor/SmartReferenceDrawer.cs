﻿#if UNITY_EDITOR
using SmartVars.Attributes;
using SmartVars.Preferences;
using SmartVars.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SmartVars.Edtior
{
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
            Color boxColor = SmartVariablePreferences.GetColor(friendlyType);

            float boxPadding = 4f;
            Rect boxRect = new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label));
            // Draw background color (box)
            EditorGUI.DrawRect(boxRect, boxColor * new Color(1f, 1f, 1f, 0.15f));// Transparent tint
            GUI.Box(boxRect, GUIContent.none);

            // Label
            string resolvedLabel = $"{label.text} ({GetResolvedTypeName(property)})";
            Rect labelRect = new Rect(position.x + boxPadding, position.y + boxPadding, position.width, lineHeight);
            // Draw attributes like [Tooltip], [Header], etc.
            DrawPropertyAttributes(position, property, label);

            // Value mode dropdown
            Rect fieldRect = new Rect(labelRect.x, labelRect.y + lineHeight + spacing, labelRect.width, lineHeight);
            EditorGUI.PropertyField(fieldRect, modeProp, new GUIContent("Value Mode"));

            fieldRect.y += lineHeight + spacing;

            // Inline or reference input
            if ((ValueSourceMode)modeProp.enumValueIndex == ValueSourceMode.Inline)
            {
                var memberInfo = fieldInfo;
                SmartRangeAttribute range = memberInfo.GetCustomAttribute<SmartRangeAttribute>();
                SmartDynamicRangeAttribute smartRange = fieldInfo.GetCustomAttribute<SmartDynamicRangeAttribute>();

                float min = 0, max = 1;
                bool useRange = false;


                if (range != null)
                {
                    min = range.Min;
                    max = range.Max;
                    useRange = true;
                }
                else if (smartRange != null)
                {
                    // Resolve min/max from sibling fields in the target object
                    var target = property.serializedObject.targetObject;
                    var targetType = target.GetType();

                    FieldInfo minField = targetType.GetField(smartRange.MinField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo maxField = targetType.GetField(smartRange.MaxField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (minField != null && maxField != null)
                    {
                        min = Convert.ToSingle(minField.GetValue(target));
                        max = Convert.ToSingle(maxField.GetValue(target));
                        useRange = true;
                    }
                }

                // Draw slider if applicable
                if (useRange && inlineValueProp.propertyType == SerializedPropertyType.Float)
                {
                    inlineValueProp.floatValue = EditorGUI.Slider(fieldRect, "Inline Value", inlineValueProp.floatValue, min, max);
                }
                else if (useRange && inlineValueProp.propertyType == SerializedPropertyType.Integer)
                {
                    inlineValueProp.intValue = EditorGUI.IntSlider(fieldRect, "Inline Value", inlineValueProp.intValue, (int)min, (int)max);
                }
                else
                {
                    EditorGUI.PropertyField(fieldRect, inlineValueProp, new GUIContent("Inline Value"));
                }
            }
            else
            {
                EditorGUI.PropertyField(fieldRect, variableProp, new GUIContent("Reference"));
                fieldRect.y += lineHeight + spacing;

                if (variableProp.objectReferenceValue == null)
                {
                    EditorGUI.HelpBox(fieldRect, "Reference is null!", MessageType.Warning);
                }
                else
                {
                    // Get the serialized object of the referenced SmartVariable
                    SerializedObject smartVarSO = new SerializedObject(variableProp.objectReferenceValue);
                    SerializedProperty valueProp = smartVarSO.FindProperty("value");
                    SerializedProperty defaultValueProp = smartVarSO.FindProperty("defaultValue");

                    if (valueProp != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.PropertyField(fieldRect, valueProp, new GUIContent("Reference Value"));
                        if (EditorGUI.EndChangeCheck())
                        {
                            smartVarSO.ApplyModifiedProperties();
                        }

                        fieldRect.y += lineHeight + spacing;

                        if (defaultValueProp != null)
                        {
                            GUI.enabled = false;
                            EditorGUI.PropertyField(fieldRect, defaultValueProp, new GUIContent("Default Value"));
                            GUI.enabled = true;
                        }
                    }
                    else
                    {
                        EditorGUI.HelpBox(fieldRect, "Value property not found.", MessageType.Warning);
                    }
                }
            }

            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty modeProp = property.FindPropertyRelative("mode");
            SerializedProperty variableProp = property.FindPropertyRelative("variable");

            float baseHeight = EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
            bool isReference = (ValueSourceMode)modeProp.enumValueIndex == ValueSourceMode.Reference;
            bool isNull = variableProp.objectReferenceValue == null;

            if (isReference)
            {
                if (isNull)
                {
                    baseHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
                else
                {
                    // Add extra line for default value
                    baseHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return baseHeight + 8f; // padding
        }

        private void DrawPropertyAttributes(Rect position, SerializedProperty property, GUIContent label)
        {
            FieldInfo field = GetFieldInfoFromProperty(property);

            if (field == null)
            {
                EditorGUI.LabelField(position, label);
                return;
            }

            var attributes = field.GetCustomAttributes<PropertyAttribute>(true);

            float yOffset = 0f;
            foreach (var attr in attributes)
            {
                if (attr is HeaderAttribute header)
                {
                    //var headerRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
                    //EditorGUI.LabelField(headerRect, header.header, EditorStyles.boldLabel);
                    //yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
                else if (attr is SpaceAttribute space)
                {
                    yOffset += space.height;
                }
                else if (attr is TooltipAttribute tooltip)
                {
                    label.tooltip = tooltip.tooltip;
                }
                // Add handling for other attributes as needed
            }

            var labelRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);
        }

        private FieldInfo GetFieldInfoFromProperty(SerializedProperty property)
        {
            var type = property.serializedObject.targetObject.GetType();
            var field = type.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return field;
        }

        private static Dictionary<Type, PropertyInfo> cachedValueProps = new();

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


}
#endif