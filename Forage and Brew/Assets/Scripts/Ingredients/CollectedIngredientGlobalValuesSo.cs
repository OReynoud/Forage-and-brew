using UnityEngine;

[CreateAssetMenu(fileName = "D_CollectedIngredientGlobalValues", menuName = "Ingredients/CollectedIngredientGlobalValuesSo")]
public class CollectedIngredientGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Min(0f)] public float GrabRadius { get; private set; } = 1f;
}
