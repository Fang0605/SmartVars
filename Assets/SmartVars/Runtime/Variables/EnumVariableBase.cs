using UnityEngine;


namespace SmartVars.Variables
{
    public abstract class EnumVariableBase : SmartVariable<System.Enum>
    {
        public abstract System.Enum EnumValue { get; set; }
    }
}
