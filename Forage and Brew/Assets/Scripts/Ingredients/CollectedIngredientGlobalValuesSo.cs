using UnityEngine;

[CreateAssetMenu(fileName = "D_CollectedIngredientGlobalValues", menuName = "Ingredients/CollectedIngredientGlobalValuesSo")]
public class CollectedIngredientGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Min(0f)] public float GrabRadius { get; private set; } = 1f;
    [field: SerializeField] [field: Min(0f)] public float StackHeight { get; private set; } = 0.2f;
    
    [field: SerializeField] [field: Min(0f)] public float MinDropInTargetLerp { get; private set; } = 0.2f;
    [field: SerializeField] [field: Min(0f)] public float MaxDropInTargetLerp { get; private set; } = 1f;
}
