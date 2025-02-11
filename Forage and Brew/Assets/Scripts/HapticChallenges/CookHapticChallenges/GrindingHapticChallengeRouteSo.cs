using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ScrapingHapticChallengeRoute", menuName = "Haptic Challenges/ScrapingHapticChallengeRouteSo")]
public class GrindingHapticChallengeRouteSo : ScriptableObject
{
    [field: SerializeField] [field: Tooltip("The points of the route.")]
    public List<Vector3> Points { get; private set; } = new();
    
    [field: SerializeField] [field: Tooltip("The rate at which the cursor position is saved (times per second).")]
    public List<GrindingHapticChallengeCrushInput> CrushInputs { get; private set; } = new();
    

    private void OnValidate()
    {
        for (int i = 0; i < Points.Count; i++)
        {
            if (Points[i].x > 3f)
            {
                Points[i] = new Vector3(3f, Points[i].y, Points[i].z);
            }
            else if (Points[i].x < -3f)
            {
                Points[i] = new Vector3(-3f, Points[i].y, Points[i].z);
            }
            
            if (Points[i].y > 1f)
            {
                Points[i] = new Vector3(Points[i].x, 1f, Points[i].z);
            }
            else if (Points[i].y < -1f)
            {
                Points[i] = new Vector3(Points[i].x, -1f, Points[i].z);
            }
        }
    }
}
