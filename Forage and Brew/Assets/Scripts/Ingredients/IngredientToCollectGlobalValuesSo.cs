using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientToCollectGlobalValues", menuName = "Ingredients/IngredientToCollectGlobalValuesSo")]
public class IngredientToCollectGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Min(0f)] public float CollectRadius { get; private set; } = 2f;
}
