using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private SmartReference<string> playerName;
    [SerializeField] private SmartReference<float> playerHealth;
    [SerializeField] private SmartReference<int> playerLives;
    [SerializeField] private SmartReference<Vector3> playerPosition;
}
