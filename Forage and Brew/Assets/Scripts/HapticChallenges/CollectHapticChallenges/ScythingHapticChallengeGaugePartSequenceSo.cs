using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ScythingHapticChallengeGaugePartSequence", menuName = "Haptic Challenges/ScythingHapticChallengeGaugePartSequenceSo")]
public class ScythingHapticChallengeGaugePartSequenceSo : ScriptableObject
{
    [field: SerializeField] [field: Tooltip("The sequence of gauge parts.")]
    public List<HapticChallengeGaugeParts> GaugeParts { get; private set; } = new();
}
