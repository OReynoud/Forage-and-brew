using UnityEngine;

[CreateAssetMenu(fileName = "D_HarvestHapticChallenge", menuName = "Haptic Challenges/HarvestHapticChallengeSo")]
public class HarvestHapticChallengeSo : CollectHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The delay tolerance between input press and release.")] [field: Min(0f)]
    public float InputReleaseDelayTolerance { get; private set; }
    
    
    [field: Header("Vibration Settings")]
    
    [field: SerializeField] [field: Tooltip("The duration of the vibration when the input can be released.")] [field: Min(0f)]
    public float InputReleaseVibrationDuration { get; private set; } = 0.5f;
    
    [field: SerializeField] [field: Tooltip("The power of the vibration when the input can be released.")] [field: Min(0f)]
    public float InputReleaseVibrationPower { get; private set; } = 0.5f;
}
