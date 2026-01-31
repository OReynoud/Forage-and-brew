using UnityEngine;

[CreateAssetMenu(fileName = "D_FrogInteractionValues", menuName = "Animals/FrogInteractionValuesSo")]
public class FrogInteractionValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Tooltip("The duration of the vibration when petting.")] [field: Min(0f)]
    public float PetVibrationDuration { get; private set; } = 1.2f;
    
    [field: SerializeField] [field: Tooltip("The target power of the vibration when petting.")] [field: Min(0f)]
    public float PetVibrationTargetPower { get; private set; } = 0.3f;
    
    [field: SerializeField] [field: Tooltip("The curve that the vibration power will follow when petting.")] [field: Min(0f)]
    public AnimationCurve PetVibrationPowerCurve { get; private set; } = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
}
