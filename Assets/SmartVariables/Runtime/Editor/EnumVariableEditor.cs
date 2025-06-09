#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using SmartVars.Variables;

namespace SmartVars.Editor
{
    [CustomEditor(typeof(EnumVariableBase), true)]
    public class EnumVariableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var enumVar = (EnumVariableBase)target;
            SerializedObject so = serializedObject;
            so.Update();

            // Draw script field (disabled)
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ScriptableObject)target), typeof(ScriptableObject), false);
            GUI.enabled = true;

            // Display enum as popup
            Enum currentEnum = enumVar.EnumValue;
            Type enumType = currentEnum.GetType();
            string[] enumNames = Enum.GetNames(enumType);
            int selectedIndex = Array.IndexOf(Enum.GetValues(enumType), currentEnum);

            int newIndex = EditorGUILayout.Popup("Value", selectedIndex, enumNames);
            if (newIndex != selectedIndex)
            {
                enumVar.EnumValue = (Enum)Enum.GetValues(enumType).GetValue(newIndex);
                EditorUtility.SetDirty(target);
            }

            so.ApplyModifiedProperties();
        }
    }
}
#endif