using System;
using System.Collections.Generic;
using UnityEngine;

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
            if(!EqualityComparer<T>.Default.Equals(this.value, value))
            {
                this.value = value;
                OnValueChanged?.Invoke(this.value);
            }
        }
    }

    public void SetValue(T newValue) => Value = newValue;
    public T GetValue() => Value;

    public void ResetValue() => Value = defaultValue;
}
