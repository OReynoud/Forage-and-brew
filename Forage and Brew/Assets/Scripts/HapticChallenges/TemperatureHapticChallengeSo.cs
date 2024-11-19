using UnityEngine;

[CreateAssetMenu(fileName = "D_TemperatureHapticChallenge", menuName = "Haptic Challenges/TemperatureHapticChallengeSo")]
public class TemperatureHapticChallengeSo : CauldronHapticChallengeSo
{
    [field: SerializeField] public float Temperature { get; private set; }
}
