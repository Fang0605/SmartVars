using UnityEngine;

public abstract class SmartVariableBinder<T> : MonoBehaviour
{
    [SerializeField] protected SmartReference<T> source;

    protected virtual void OnEnable()
    {
        if (source != null)
            source.OnInlineValueChanged += OnValueChanged;

        UpdateUI(source != null ? source.Value : default);
    }

    protected virtual void OnDisable()
    {
        if (source != null)
            source.OnInlineValueChanged -= OnValueChanged;
    }

    protected void OnValueChanged(T newValue)
    {
        UpdateUI(newValue);
    }

    protected abstract void UpdateUI(T newValue);
}
