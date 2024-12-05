using UnityEngine;

[CreateAssetMenu(fileName = "D_TemperatureHapticChallengeGlobalValues", menuName = "Haptic Challenges/TemperatureHapticChallengeGlobalValuesSo")]
public class TemperatureHapticChallengeGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Range(0f, 1f)] public float LowHeatMinValue { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float LowHeatMaxValue { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float MediumHeatMinValue { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float MediumHeatMaxValue { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float HighHeatMinValue { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float HighHeatMaxValue { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float HeatDecreaseSpeed { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float HeatIncreaseDuration { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float HeatIncreaseQuantity { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float TemperatureMaintenanceDuration { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float InputDetectionTolerance { get; private set; } = 0.15f;
}
