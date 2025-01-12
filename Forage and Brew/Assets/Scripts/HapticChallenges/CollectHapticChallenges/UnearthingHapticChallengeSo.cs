using UnityEngine;

[CreateAssetMenu(fileName = "D_UnearthingHapticChallenge", menuName = "Haptic Challenges/UnearthingHapticChallengeSo")]
public class UnearthingHapticChallengeSo : CollectHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The delay tolerance between both input releases.")] [field: Min(0f)]
    public float InputReleaseDelayTolerance { get; private set; }
}
