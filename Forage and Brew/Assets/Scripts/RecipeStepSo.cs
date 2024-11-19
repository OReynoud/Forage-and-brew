using UnityEngine;

[CreateAssetMenu(fileName = "RecipeStepSo", menuName = "Scriptable Objects/RecipeStepSo")]
public class RecipeStepSo : ScriptableObject
{
    [field: SerializeField] public IngredientValuesSo Ingredient { get; private set; }
}
