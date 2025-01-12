using UnityEngine;

[CreateAssetMenu(fileName = "D_PotionList", menuName = "Potions/PotionListSo")]
public class PotionListSo : ScriptableObject
{
    [field: SerializeField] public PotionValuesSo[] Potions { get; private set; }
    [field: SerializeField] public PotionValuesSo DefaultPotion { get; private set; }
}
