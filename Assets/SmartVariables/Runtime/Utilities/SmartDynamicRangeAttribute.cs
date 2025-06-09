using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SmartDynamicRangeAttribute : PropertyAttribute
{
    public string MinField { get; }
    public string MaxField { get; }

    public SmartDynamicRangeAttribute(string minFieldName, string maxFieldName)
    {
        MinField = minFieldName;
        MaxField = maxFieldName;
    }
}