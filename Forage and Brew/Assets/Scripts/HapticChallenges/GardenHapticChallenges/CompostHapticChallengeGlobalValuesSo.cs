using UnityEngine;

[CreateAssetMenu(fileName = "D_CompostHapticChallengeGlobalValues", menuName = "Haptic Challenges/CompostHapticChallengeGlobalValuesSo")]
public class CompostHapticChallengeGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Min(0)] public int InputCount { get; private set; } = 10;
    [field: SerializeField] [field: Min(0f)] public float ObtainedSeedAnimationDuration { get; private set; } = 2.4f;
}
