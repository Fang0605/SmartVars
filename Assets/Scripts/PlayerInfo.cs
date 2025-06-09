using UnityEngine;
using SmartVars.Attributes;
using SmartVars.Utilities;

public class PlayerInfo : MonoBehaviour
{

    [SerializeField] private SmartReference<string> playerName;

    [SerializeField] private SmartReference<float> playerHealth;

    private float minDamage = 0, maxDamage = 10;
    [SmartDynamicRange(nameof(minDamage), nameof(maxDamage))]
    [SerializeField] private SmartReference<float> playerDamage;

    [SerializeField] private SmartReference<int> playerLives;

    [SerializeField] private SmartReference<Vector3> playerPosition;

    [SerializeField] private SmartReference<Weapons> weapons;

}

