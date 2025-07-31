using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientType", menuName = "Ingredients/IngredientTypeSo")]
public class IngredientTypeSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite IconLow { get; private set; }
    [field: SerializeField] public Sprite IconHigh { get; private set; }
    [field: SerializeField] public bool IsChoppable { get; private set; }
    [field: SerializeField] public bool IsGrindable { get; private set; }
    [field: ShowIf("IsGrindable")] [field: SerializeField] public GameObject GroundMeshGameObject { get; private set; }
}
