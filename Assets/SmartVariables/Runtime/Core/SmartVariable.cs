using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartVars
{
    public abstract class SmartVariable<T> : ScriptableObject
    {
        [SerializeField] protected T value;

        [SerializeField] private T defaultValue;

        public Action<T> OnValueChanged;

        public virtual T Value
        {
            get => value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(this.value, value))
                {
                    this.value = value;
                    OnValueChanged?.Invoke(this.value);
                }
            }
        }

        public void SetValue(T newValue) => Value = newValue;
        public T GetValue() => Value;

        public void ResetValue() => Value = defaultValue;

#if UNITY_EDITOR
        // Automatically reset to defaultValue when exiting Play Mode
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                value = defaultValue;
                EditorUtility.SetDirty(this); // update in inspector
            }
        }
#endif
    }

}