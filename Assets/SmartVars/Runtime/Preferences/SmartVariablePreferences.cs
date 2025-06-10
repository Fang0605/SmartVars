using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartVars.Preferences
{
    public class SmartVariablePreferences : MonoBehaviour
    {
        private const string Prefix = "SmartVariable_Color_";

        private static readonly Dictionary<string, Color> defaultColors = new()
    {
        { "Float", new Color(0.4f, 0.6f, 1f) },
        { "Integer", new Color(1f, 0.65f, 0.2f) },
        { "Bool", new Color(0.4f, 1f, 0.4f) },
        { "String", new Color(0.8f, 0.6f, 1f) },
        { "Vector2", new Color(0.6f, 1f, 1f) },
        { "Vector3", new Color(0.6f, 1f, 1f) },
        { "null", new Color(0.5f, 0.5f, 0.5f) },
        { "Unknown", new Color(0.7f, 0.7f, 0.7f) }
    };

        public static Color GetColor(string type)
        {
            string key = Prefix + type;
            if (EditorPrefs.HasKey(key))
            {
                ColorUtility.TryParseHtmlString(EditorPrefs.GetString(key), out var c);
                return c;
            }

            return defaultColors.TryGetValue(type, out var color) ? color : defaultColors["Unknown"];
        }

        public static void SetColor(string type, Color color)
        {
            EditorPrefs.SetString(Prefix + type, $"#{ColorUtility.ToHtmlStringRGBA(color)}");
        }

        [SettingsProvider]
        public static SettingsProvider CreatePreferencesGUI()
        {
            return new SettingsProvider("Preferences/Smart Variables", SettingsScope.User)
            {
                label = "Smart Variables",
                guiHandler = (searchContext) =>
                {
                    GUILayout.Label("Type Colors", EditorStyles.boldLabel);

                    foreach (var entry in defaultColors)
                    {
                        Color current = GetColor(entry.Key);
                        Color newColor = EditorGUILayout.ColorField(ObjectNames.NicifyVariableName(entry.Key), current);

                        if (newColor != current)
                            SetColor(entry.Key, newColor);
                    }
                }
            };
        }
    }

}