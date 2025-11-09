using UnityEngine;

[CreateAssetMenu(fileName = "D_CompostHapticChallengeGlobalValues", menuName = "Haptic Challenges/CompostHapticChallengeGlobalValuesSo")]
public class CompostHapticChallengeGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Min(0)] public int InputCount { get; private set; } = 10;
    
    [field: Header("Obtained Potion Animation")]
    [field: SerializeField] public Vector2 ObtainedSeedAnimationStartPosition { get; private set; }
    [field: SerializeField] public Vector2 ObtainedSeedAnimationEndPosition { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float ObtainedSeedAnimationDuration { get; private set; } = 1f;
    [field: SerializeField] public AnimationCurve ObtainedSeedScaleAnimationCurve { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [field: SerializeField] public AnimationCurve ObtainedSeedPositionAnimationCurve { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [field: SerializeField] [field: Min(0f)] public float ObtainedSeedStayDuration { get; private set; } = 1f;
    [field: SerializeField] public AnimationCurve ObtainedSeedScaleEndAnimationCurve { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [field: SerializeField] [field: Min(0f)] public float ObtainedSeedAnimationEndDuration { get; private set; } = 1f;
}
