using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ScythingHapticChallenge", menuName = "Haptic Challenges/ScythingHapticChallengeSo")]
public class ScythingHapticChallengeSo : CollectHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The list of sequences of gauge parts.")]
    public List<ScythingHapticChallengeGaugePartSequenceSo> GaugeParts { get; private set; } = new();
    
    [field: SerializeField] [field: Tooltip("The speed of the arrow (in gauge part).")]
    [field: Range(0f, 1f)] public float ArrowSpeed { get; private set; } = 0.4f;
    
    [field: SerializeField] [field: Tooltip("The tolerance of the detection of the joystick input.")]
    [field: Range(0f, 1f)] public float InputDetectionTolerance { get; private set; } = 0.1f;
}
