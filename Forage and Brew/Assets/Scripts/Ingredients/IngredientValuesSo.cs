using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientValues", menuName = "Ingredients/IngredientValuesSo")]
public class IngredientValuesSo : ScriptableObject
{
    [field: SerializeField] public IngredientType Type { get; private set; }
    [field: SerializeField] public GameObject MeshGameObject { get; private set; }
}
