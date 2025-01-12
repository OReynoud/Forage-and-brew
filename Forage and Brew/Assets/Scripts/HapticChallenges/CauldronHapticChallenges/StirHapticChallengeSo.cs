using UnityEngine;

[CreateAssetMenu(fileName = "D_StirHapticChallenge", menuName = "Haptic Challenges/StirHapticChallengeSo")]
public class StirHapticChallengeSo : ScriptableObject
{
    [field: SerializeField] public StirCameraAndDuration[] StirCamerasAndDurations { get; private set; }
}
