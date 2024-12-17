using UnityEngine;

[CreateAssetMenu(fileName = "D_HarvestHapticChallenge", menuName = "Haptic Challenges/HarvestHapticChallengeSo")]
public class HarvestHapticChallengeSo : CollectHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The delay tolerance between input press and release.")] [field: Min(0f)]
    public float InputReleaseDelayTolerance { get; private set; }
}
