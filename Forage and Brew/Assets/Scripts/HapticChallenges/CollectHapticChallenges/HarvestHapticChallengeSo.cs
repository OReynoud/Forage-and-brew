using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_HarvestHapticChallenge", menuName = "Haptic Challenges/HarvestHapticChallengeSo")]
public class HarvestHapticChallengeSo : CollectHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The list of sequences of gauge parts.")]
    public List<HarvestHapticChallengeGaugePartSequenceSo> GaugeParts { get; private set; } = new();
    
    [field: SerializeField] [field: Tooltip("The speed of the arrow (in gauge part).")]
    [field: Range(0f, 1f)] public float ArrowSpeed { get; private set; } = 0.4f;
}
