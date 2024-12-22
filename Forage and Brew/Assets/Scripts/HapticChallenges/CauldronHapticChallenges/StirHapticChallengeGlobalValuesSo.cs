using UnityEngine;

[CreateAssetMenu(fileName = "D_StirHapticChallengeGlobalValues", menuName = "Haptic Challenges/StirHapticChallengeGlobalValuesSo")]
public class StirHapticChallengeGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Range(0.01f, 1f)] public float CheckPositionInterval { get; private set; } = 0.01f;
    [field: SerializeField] [field: Range(0f, 180f)] public float AngleToleranceForTurnEnd { get; private set; } = 5f;
    [field: SerializeField] [field: Range(0f, 180f)] public float AngleToleranceForPreviewEnd { get; private set; } = 15f;
    [field: SerializeField] [field: Min(0f)] public float PreviewPauseBetweenTurnsDuration { get; private set; } = 0.1f;
}
