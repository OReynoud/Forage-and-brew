using UnityEngine;

[CreateAssetMenu(fileName = "D_TemperatureHapticChallengeGlobalValues", menuName = "Haptic Challenges/TemperatureHapticChallengeGlobalValuesSo")]
public class CompostHapticChallengeGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Range(0f, 1f)] public float LowHeatMinValue { get; private set; }
}
