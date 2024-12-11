using UnityEngine;

[CreateAssetMenu(fileName = "D_CollectedPotionGlobalValues", menuName = "Potions/CollectedPotionGlobalValuesSo")]
public class CollectedPotionGlobalValuesSo : ScriptableObject
{
    [field: SerializeField] [field: Min(0f)] public float GrabRadius { get; private set; } = 1f;
    [field: SerializeField] [field: Min(0f)] public float StackHeight { get; private set; } = 0.4f;
    
    [field: SerializeField] [field: Min(0f)] public float MinDropInTargetLerp { get; private set; } = 0.8f;
    [field: SerializeField] [field: Min(0f)] public float MaxDropInTargetLerp { get; private set; } = 3f;
}
