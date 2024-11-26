using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_UnearthingHapticChallenge", menuName = "Haptic Challenges/UnearthingHapticChallengeSo")]
public class UnearthingHapticChallengeSo : CollectHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The probability to have a movement direction.")]
    public List<HapticChallengeMovementDirectionProbability> MovementDirectionProbabilities { get; private set; }
    
    [field: SerializeField] [field: Tooltip("The duration of the movement direction (in seconds).")]
    public float MovementDirectionDuration { get; private set; }
    
    [field: SerializeField] [field: Tooltip("The tolerance of the cardinal movement directions (for a vector unit 1).")]
    public float CardinalMovementDirectionTolerance { get; private set; }
    
    [field: SerializeField] [field: Tooltip("The tolerance of the diagonal movement directions (for a vector unit 1).")]
    public float DiagonalMovementDirectionTolerance { get; private set; }
    
    [field: SerializeField] [field: Range(0f, 1f)] [field: Tooltip("The part of the gauge increase when the movement direction is correct.")]
    public float GaugeIncreasePart { get; private set; }
}
