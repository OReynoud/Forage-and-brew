using UnityEngine;

[CreateAssetMenu(fileName = "D_PotionValues", menuName = "Potions/PotionValuesSo")]
public class PotionValuesSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] [field: Range(1, 5)] public int Difficulty { get; private set; } = 1;
    [field: SerializeField] public CauldronHapticChallengeIngredients[] CauldronHapticChallengeIngredients { get; private set; }
    [field: SerializeField] public StirHapticChallengeSo StirHapticChallenge { get; private set; }
    [field: SerializeField] [field: Min(0)] public int SalePrice { get; private set; }
    [field: SerializeField] public GameObject MeshGameObject { get; private set; }
}
