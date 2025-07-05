using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_PotionEnsemble", menuName = "Potions/PotionEnsembleSo")]
public class PotionEnsembleSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public List<PotionValuesSo> Potions { get; private set; } = new();
    [field: SerializeField] public int MoneyReward { get; private set; }
}
