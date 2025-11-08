using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientToCollectGlobalValues", menuName = "Ingredients/IngredientToCollectGlobalValuesSo")]
public class IngredientToCollectGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Min(0f)] public float CollectRadius { get; private set; } = 2f;
    [field: SerializeField] [field: Min(0f)] public float AfkTriggerTime { get; private set; } = 5f;
    
    [field: Header("Obtaining Feedback")]
    [field: SerializeField] public float ObtainingFeedbackDuration { get; private set; } = 2f;
    [field: SerializeField] public float ObtainingFeedbackDistance { get; private set; } = 256f;
    [field: SerializeField] public AnimationCurve ObtainingFeedbackMoveCurve { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [field: SerializeField] public AnimationCurve ObtainingFeedbackFadeCurve { get; private set; } = AnimationCurve.Linear(0f, 1f, 1f, 0f);
}
