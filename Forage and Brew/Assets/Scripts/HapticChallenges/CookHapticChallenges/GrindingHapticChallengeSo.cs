using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_GrindingHapticChallenge", menuName = "Haptic Challenges/GrindingHapticChallengeSo")]
public class GrindingHapticChallengeSo : CookHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The list of routes.")]
    public List<GrindingHapticChallengeRouteSo> Routes { get; private set; } = new();
    
    [field: SerializeField] [field: Tooltip("The rate at which the drawn positions are saved (times per second).")]
    [field: Min(1)] public int DrawnPositionsSaveRate { get; private set; } = 2;
    
    [field: SerializeField] [field: Tooltip("The speed of the cursor (pixels per second).")]
    [field: Min(0f)] public float CursorSpeed { get; private set; } = 50f;
    
    [field: SerializeField] [field: Tooltip("The distance tolerance between the cursor and the end of the route to consider it as completed.")]
    [field: Min(0f)] public float EndPointDistanceTolerance { get; private set; } = 10f;
    
    [field: SerializeField] [field: Tooltip("The distance tolerance between the cursor and the crush input to consider it as completed.")]
    [field: Min(0f)] public float CrushInputDistanceTolerance { get; private set; } = 15f;
    
    [field: SerializeField] [field: Tooltip("The minimum speed of the grinding animation.")]
    [field: Min(0f)] public float GrindingAnimationMinSpeed { get; private set; } = 0.1f;
}
