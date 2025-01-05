using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientTypeList", menuName = "Ingredients/IngredientTypeListSo")]
public class IngredientTypeListSo : ScriptableObject
{
    [field: SerializeField] public IngredientTypeSo[] IngredientTypes { get; private set; }
}
