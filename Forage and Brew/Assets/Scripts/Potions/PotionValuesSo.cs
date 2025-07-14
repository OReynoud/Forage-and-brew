using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "D_PotionValues", menuName = "Potions/PotionValuesSo")]
public class PotionValuesSo : StackableValuesSo
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] [field: ResizableTextArea]public string Description { get; private set; }
    
    [field: SerializeField] [field: EnumFlags] public List<PotionTagSo> Tags { get; private set; } = new();
    
    [field: SerializeField] public PotionDifficultySo PotionDifficulty { get; private set; }
    
    [field: SerializeField] public TemperatureChallengeIngredients[] TemperatureChallengeIngredients { get; private set; }
    [field: SerializeField] public StirHapticChallengeSo StirHapticChallenge { get; private set; }
    
    [field: SerializeField] public Color SpriteLiquidColor { get; private set; } = Color.white;
    [field: SerializeField] [field: ColorUsage(true, true)] public Color MeshLiquidMainColor { get; private set; } = Color.white;
    [field: SerializeField] [field: ColorUsage(true, true)] public Color MeshLiquidTopColor { get; private set; } = Color.white;
    [field: SerializeField] [field: ColorUsage(true, true)] public Color MeshLiquidFoamColor { get; private set; } = Color.white;
    [field: SerializeField] public Color MeshLiquidRimColor { get; private set; } = Color.white;

    
    public void SetData(string newName, string newDescription, PotionDifficultySo newPotionDifficulty,
        TemperatureChallengeIngredients[] newTemperatureChallengeIngredients)
    {
        Name = newName;
        Description = newDescription;
        PotionDifficulty = newPotionDifficulty;
        TemperatureChallengeIngredients = newTemperatureChallengeIngredients;
    }
    
    public void SetDefaultData(StirHapticChallengeSo newStirHapticChallenge, Color newSpriteLiquidColor,
        Color newMeshLiquidColor, Color newMeshLiquidTopColor, Color newMeshLiquidFoamColor,
        Color newMeshLiquidRimColor)
    {
        StirHapticChallenge = newStirHapticChallenge;
        SpriteLiquidColor = newSpriteLiquidColor;
        MeshLiquidMainColor = newMeshLiquidColor;
        MeshLiquidTopColor = newMeshLiquidTopColor;
        MeshLiquidFoamColor = newMeshLiquidFoamColor;
        MeshLiquidRimColor = newMeshLiquidRimColor;
    }
}
