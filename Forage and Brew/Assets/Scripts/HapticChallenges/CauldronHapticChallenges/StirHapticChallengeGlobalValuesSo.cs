using UnityEngine;

[CreateAssetMenu(fileName = "D_StirHapticChallengeGlobalValues", menuName = "Haptic Challenges/StirHapticChallengeGlobalValuesSo")]
public class StirHapticChallengeGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Range(0.01f, 1f)] public float CheckPositionInterval { get; private set; } = 0.01f;
    [field: SerializeField] [field: Range(0f, 180f)] public float AngleToleranceForTurnEnd { get; private set; } = 5f;
    [field: SerializeField] [field: Range(0f, 180f)] public float AngleToleranceForPreviewEnd { get; private set; } = 15f;
    [field: SerializeField] [field: Min(0f)] public float PreviewPauseBetweenTurnsDuration { get; private set; } = 0.1f;
    
    [field: Header("Vibration Settings")]
    [field: SerializeField] [field: Min(0f)] public float StirTurnVibrationDuration { get; private set; } = 0.2f;
    [field: SerializeField] [field: Min(0f)] public float StirTurnVibrationPower { get; private set; } = 1f;
    
    [field: Header("Obtained Potion Animation")]
    [field: SerializeField] public Vector2 ObtainedPotionAnimationStartPosition { get; private set; }
    [field: SerializeField] public Vector2 ObtainedPotionAnimationEndPosition { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float ObtainedPotionAnimationDuration { get; private set; } = 1f;
    [field: SerializeField] public AnimationCurve ObtainedPotionScaleAnimationCurve { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [field: SerializeField] public AnimationCurve ObtainedPotionPositionAnimationCurve { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [field: SerializeField] [field: Min(0f)] public float ObtainedPotionStayDuration { get; private set; } = 1f;
    [field: SerializeField] public AnimationCurve ObtainedPotionScaleEndAnimationCurve { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [field: SerializeField] [field: Min(0f)] public float ObtainedPotionAnimationEndDuration { get; private set; } = 1f;
}
