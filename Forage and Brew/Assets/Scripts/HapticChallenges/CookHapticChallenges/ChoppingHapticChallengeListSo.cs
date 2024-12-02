using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ChoppingHapticChallengeList", menuName = "Haptic Challenges/ChoppingHapticChallengeListSo")]
public class ChoppingHapticChallengeListSo : CookHapticChallengeSo
{
    [field: SerializeField] public List<ChoppingHapticChallengeSo> ChoppingHapticChallenges { get; private set; }
}
