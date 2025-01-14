using UnityEngine;

[CreateAssetMenu(fileName = "D_PotionDifficulty", menuName = "Potions/PotionDifficultySo")]
public class PotionDifficultySo : ScriptableObject
{
    [field: SerializeField] [field: Range(0, 5)] public int Difficulty { get; private set; } = 1;
    [field: SerializeField] public Sprite PotionSprite { get; private set; }
    [field: SerializeField] public Sprite LiquidSprite { get; private set; }
    [field: SerializeField] public PotionLiquidColorManager MeshGameObjectLiquidColorManager { get; private set; }
}
