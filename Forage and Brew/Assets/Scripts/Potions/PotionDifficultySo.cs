using UnityEngine;

[CreateAssetMenu(fileName = "D_PotionDifficulty", menuName = "Potions/PotionDifficultySo")]
public class PotionDifficultySo : ScriptableObject
{
    [field: SerializeField] [field: Range(1, 5)] public int Difficulty { get; private set; } = 1;
    [field: SerializeField] public GameObject MeshGameObject { get; private set; }
}
