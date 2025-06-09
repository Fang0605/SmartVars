using UnityEngine;
namespace SmartVars.Variables
{
    [CreateAssetMenu(menuName = "SmartVariables/Enum Variable")]
    public class EnumVariable<TEnum> : SmartVariable<TEnum> where TEnum : System.Enum
    {

    }
}