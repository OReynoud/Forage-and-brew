using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_HarvestHapticChallengeGaugePartSequence", menuName = "Haptic Challenges/HarvestHapticChallengeGaugePartSequenceSo")]
public class HarvestHapticChallengeGaugePartSequenceSo : ScriptableObject
{
    [field: SerializeField] [field: Tooltip("The sequence of gauge parts.")]
    public List<HapticChallengeGaugeParts> GaugeParts { get; private set; } = new();
}
