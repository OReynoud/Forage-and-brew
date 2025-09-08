using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_CharacterOutfitList", menuName = "Outfits/CharacterOutfitListSo")]
public class CharacterOutfitListSo : ScriptableObject
{
    [field: SerializeField] public List<CharacterOutfitSo> CasualOutfits { get; private set; } = new();
    [field: SerializeField] public List<CharacterOutfitSo> RainOutfits { get; private set; } = new();


    private void OnValidate()
    {
        if (CasualOutfits.Count != RainOutfits.Count)
        {
            Debug.LogError($"CasualOutfits count ({CasualOutfits.Count}) does not match RainOutfits count" +
                           $" ({RainOutfits.Count}) in {name}. Please ensure both lists have the same number of" +
                           $" outfits.");
        }
    }
}
