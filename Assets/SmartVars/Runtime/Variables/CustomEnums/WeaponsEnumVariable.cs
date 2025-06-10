using UnityEngine;

namespace SmartVars.Variables
{
    [CreateAssetMenu(menuName = "SmartVariables/Weapons Variable")]
    public class WeaponsEnumVariable : SmartVariable<Weapons> { }
}

public enum Weapons
{
    Fist,
    Sword,
    Gun
}