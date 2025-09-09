using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "D_SeedValues", menuName = "Seeds/SeedValuesSo")]
public class SeedValuesSo : StackableValuesSo
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] [field: ResizableTextArea] public string Description { get; private set; }
    [field: SerializeField] public Sprite SeedIcon { get; private set; }
    [field: SerializeField] public IngredientTypeSo RequiredIngredientType { get; private set; }
    
    [field: SerializeField] public int DaysToMature { get; private set; }
    [field: SerializeField] public int RequiredIngredientTypeAmount { get; private set; }
    [field: SerializeField] public GameObject MeshGameObject { get; private set; }
}
