using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientType", menuName = "Ingredients/IngredientTypeSo")]
public class IngredientTypeSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}
