using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private SmartReference<float> playerHealth;

    [SerializeField] private SmartReference<string> playerName; 
}
