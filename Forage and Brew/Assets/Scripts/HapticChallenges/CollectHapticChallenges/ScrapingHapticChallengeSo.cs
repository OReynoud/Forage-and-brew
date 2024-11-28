using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ScrapingHapticChallenge", menuName = "Haptic Challenges/ScrapingHapticChallengeSo")]
public class ScrapingHapticChallengeSo : CollectHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The list of routes.")]
    public List<ScrapingHapticChallengeRouteSo> Routes { get; private set; } = new();
}
