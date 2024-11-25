using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_CollectHapticChallengeList", menuName = "Haptic Challenges/CollectHapticChallengeListSo")]
public class CollectHapticChallengeListSo : ScriptableObject
{
    [field: SerializeField] public List<IngredientTypeHapticChallenge> HapticChallengesByIngredientType { get; private set; }
}
