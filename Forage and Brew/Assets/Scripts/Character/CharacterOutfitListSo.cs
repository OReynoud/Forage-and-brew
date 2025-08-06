using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_CharacterOutfitList", menuName = "Outfits/CharacterOutfitListSo")]
public class CharacterOutfitListSo : ScriptableObject
{
    [field: SerializeField] public List<CharacterOutfitSo> Outfits { get; private set; } = new();
}
