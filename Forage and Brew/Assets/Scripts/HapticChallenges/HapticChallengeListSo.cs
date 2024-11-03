using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_HapticChallengeList", menuName = "Haptic Challenges/HapticChallengeListSo")]
public class HapticChallengeListSo : ScriptableObject
{
    [field: SerializeField] public List<IngredientTypeHapticChallenge> HapticChallengesByIngredientType { get; private set; }
}
