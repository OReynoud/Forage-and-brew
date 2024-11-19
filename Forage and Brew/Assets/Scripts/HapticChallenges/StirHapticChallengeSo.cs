using UnityEngine;

[CreateAssetMenu(fileName = "D_StirHapticChallenge", menuName = "Haptic Challenges/StirHapticChallengeSo")]
public class StirHapticChallengeSo : ScriptableObject
{
    [field: SerializeField] [field: Min(0)] public float[] StirDurations { get; private set; }
}
