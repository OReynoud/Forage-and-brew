using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ScrapingHapticChallengeRoute", menuName = "Haptic Challenges/ScrapingHapticChallengeRouteSo")]
public class ScrapingHapticChallengeRouteSo : ScriptableObject
{
    [field: SerializeField] [field: Tooltip("The points of the route.")]
    public List<Vector3> Points { get; private set; } = new();
}
