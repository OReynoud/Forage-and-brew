using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_CharacterOutfit", menuName = "Outfits/CharacterOutfitSo")]
public class CharacterOutfitSo : ScriptableObject
{
    [field: SerializeField] public string OutfitName { get; private set; }
    [field: SerializeField] public Color OutfitColor { get; private set; }
    [field: SerializeField] public Sprite OutfitSprite { get; private set; }
    [field: SerializeField] public Material OutfitMaterial { get; private set; }
    [field: SerializeField] public bool IsRainOutfit { get; private set; }
    [field: SerializeField] public int OutfitMoneyCost { get; private set; }
    [field: SerializeField] public List<IngredientCost> IngredientCosts { get; private set; } = new();
}
