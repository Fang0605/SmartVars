using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SmartVariable<T> : ScriptableObject
{
    [SerializeField] protected T value;

    [SerializeField] private T defaultValue;

    public UnityEvent<T> OnValueChanged = new UnityEvent<T>();

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
