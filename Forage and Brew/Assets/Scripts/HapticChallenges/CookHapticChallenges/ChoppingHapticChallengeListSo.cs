using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ChoppingHapticChallengeList", menuName = "Haptic Challenges/ChoppingHapticChallengeListSo")]
public class ChoppingHapticChallengeListSo : CookHapticChallengeSo
{
    [field: SerializeField] public List<ChoppingHapticChallengeSo> ChoppingHapticChallenges { get; private set; }
    
    [field: SerializeField] [field: Tooltip("The time before the next chopping (in seconds).")]
    public float TimeBeforeNextChopping { get; private set; } = 1f;
    
    [field: Header("Vibration Settings")]
    
    [field: SerializeField] [field: Tooltip("The duration of the vibration when the input is correct.")] [field: Min(0f)]
    public float CorrectInputVibrationDuration { get; private set; } = 0.5f;
    
    [field: SerializeField] [field: Tooltip("The power of the vibration when the input is correct.")] [field: Min(0f)]
    public float CorrectInputVibrationPower { get; private set; } = 2f;
    
    [field: SerializeField] [field: Tooltip("The durations of the vibration when the input is wrong.")] [field: Min(0f)]
    public List<float> WrongInputVibrationDurations { get; private set; } = new();
    
    [field: SerializeField] [field: Tooltip("The power of the vibration when the input is wrong.")] [field: Min(0f)]
    public float WrongInputVibrationPower { get; private set; } = 0.5f;
    
    [field: SerializeField] [field: Tooltip("The intervals between the vibrations when the input is wrong.")] [field: Min(0f)]
    public List<float> WrongInputVibrationIntervals { get; private set; } = new();
    
    
    private void OnValidate()
    {
        if (WrongInputVibrationDurations.Count != WrongInputVibrationIntervals.Count + 1)
        {
            Debug.LogError("The number of wrong input vibration durations should be equal to the number of wrong" +
                           " input vibration intervals + 1.");
        }
    }
}
