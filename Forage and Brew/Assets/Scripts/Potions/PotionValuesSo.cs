using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "D_PotionValues", menuName = "Potions/PotionValuesSo")]
public class PotionValuesSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] [field: EnumFlags] public PotionTag tags { get; private set; } 
    [field: SerializeField] [field: EnumFlags] [field: ReadOnly] public PotionTag effectiveTags { get; private set; }
    
    [field: SerializeField] [field: Range(1, 5)] public int Difficulty { get; private set; } = 1;
    [field: SerializeField] public TemperatureChallengeIngredients[] TemperatureChallengeIngredients { get; private set; }
    [field: SerializeField] public StirHapticChallengeSo StirHapticChallenge { get; private set; }
    [field: SerializeField] [field: Min(0)] public int SalePrice { get; private set; }
    [field: SerializeField] public GameObject MeshGameObject { get; private set; }


    private void OnValidate()
    {
        effectiveTags = PotionTag.None;
        if ((tags & PotionTag.FullEffects) == PotionTag.FullEffects)
        {
            effectiveTags |= PotionTag.ShallowCuts | PotionTag.Colds | PotionTag.Illness | PotionTag.Beverage | PotionTag.EnergyDrink;
            return;
        }
        if ((tags & PotionTag.FullHeal) == PotionTag.FullHeal)
        {
            effectiveTags |= PotionTag.ShallowCuts | PotionTag.Colds | PotionTag.Illness;
        }
        else
        {
            if ((tags & PotionTag.Colds) == PotionTag.Colds)
            {
                effectiveTags |= PotionTag.Colds;
            }
            if ((tags & PotionTag.Illness) == PotionTag.Illness)
            {
                effectiveTags |= PotionTag.Illness;
            }
            if ((tags & PotionTag.ShallowCuts) == PotionTag.ShallowCuts)
            {
                effectiveTags |= PotionTag.ShallowCuts;
            }
        }
        if ((tags & PotionTag.Breakfast) == PotionTag.Breakfast)
        {
            effectiveTags |= PotionTag.Beverage | PotionTag.EnergyDrink;
        }
        else
        {
            if ((tags & PotionTag.Beverage) == PotionTag.Beverage)
            {
                effectiveTags |= PotionTag.Beverage;
            }
            if ((tags & PotionTag.EnergyDrink) == PotionTag.EnergyDrink)
            {
                effectiveTags |= PotionTag.EnergyDrink;
            }
        }
    }
}
