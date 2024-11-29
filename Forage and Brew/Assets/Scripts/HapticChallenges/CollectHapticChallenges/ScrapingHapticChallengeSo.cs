using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ScrapingHapticChallenge", menuName = "Haptic Challenges/ScrapingHapticChallengeSo")]
public class ScrapingHapticChallengeSo : CollectHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The list of routes.")]
    public List<ScrapingHapticChallengeRouteSo> Routes { get; private set; } = new();
    
    [field: SerializeField] [field: Tooltip("The rate at which the drawn positions are saved (times per second).")]
    public int DrawnPositionsSaveRate { get; private set; } = 2;
    
    [field: SerializeField] [field: Tooltip("The speed of the cursor (pixels per second).")]
    public float CursorSpeed { get; private set; } = 50f;
    
    [field: SerializeField] [field: Tooltip("The distance tolerance between the cursor and the end of the route to consider it as completed.")]
    public float EndPointDistanceTolerance { get; private set; } = 10f;
}
