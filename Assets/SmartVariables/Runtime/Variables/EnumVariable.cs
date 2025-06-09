using UnityEngine;
namespace SmartVars.Variables
{
    public class EnumVariable<TEnum> : EnumVariableBase where TEnum : System.Enum
    {
        [SerializeField] private TEnum enumValue;

        public TEnum TypedValue
        {
            get => enumValue;
            set => enumValue = value;
        }

        public override System.Enum EnumValue
        {
            get => enumValue;
            set => enumValue = (TEnum)value;
        }

        public override System.Enum Value
        {
            get => enumValue;
            set => enumValue = (TEnum)value;
        }
    }
}