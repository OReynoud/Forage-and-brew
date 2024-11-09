using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientList", menuName = "Ingredients/IngredientListSo")]
public class IngredientListSo : ScriptableObject
{
    [field: SerializeField] public IngredientValuesSo[] IngredientValues { get; private set; }
}
