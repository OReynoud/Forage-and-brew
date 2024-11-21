using UnityEngine;

[CreateAssetMenu(fileName = "D_TemperatureHapticChallengeGlobalValues", menuName = "Haptic Challenges/TemperatureHapticChallengeGlobalValuesSo")]
public class TemperatureHapticChallengeGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Range(0f, 1f)] public float LowHeatPart { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float HighHeatPart { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float HeatDecreaseSpeed { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float HeatIncreaseDuration { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float HeatIncreaseQuantity { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float TemperatureMaintenanceDuration { get; private set; }
}
