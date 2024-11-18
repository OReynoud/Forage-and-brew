using UnityEngine;

[CreateAssetMenu(fileName = "D_PotionValues", menuName = "Potions/PotionValuesSo")]
public class PotionValuesSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public 
    [field: SerializeField] public GameObject MeshGameObject { get; private set; }
}
