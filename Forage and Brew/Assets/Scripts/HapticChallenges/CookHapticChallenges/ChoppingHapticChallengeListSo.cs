using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ChoppingHapticChallengeList", menuName = "Haptic Challenges/ChoppingHapticChallengeListSo")]
public class ChoppingHapticChallengeListSo : CookHapticChallengeSo
{
    [field: SerializeField] public List<ChoppingHapticChallengeSo> ChoppingHapticChallenges { get; private set; }
    
    [field: SerializeField] [field: Tooltip("The time before the next chopping (in seconds).")]
    public float TimeBeforeNextChopping { get; private set; } = 1f;
}
