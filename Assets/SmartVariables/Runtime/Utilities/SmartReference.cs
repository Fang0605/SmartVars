using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class SmartReference<T>
{
    [SerializeField] private ValueSourceMode mode = ValueSourceMode.Inline;

    [SerializeField] private T inlineValue;

    [SerializeField] private SmartVariable<T> variable;

    public T Value
    {
        get => mode == ValueSourceMode.Inline? inlineValue : variable.Value;
        set
        {
            if(mode == ValueSourceMode.Inline)
                inlineValue = value;
            else if(variable != null)
                variable.Value = value;
        }
    }

    public bool IsUsingInlineValue => mode == ValueSourceMode.Inline;

    public ValueSourceMode Mode => mode;
    public SmartVariable<T> Variable => variable;
}
