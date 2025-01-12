using UnityEngine;

public interface IIngredientAddable
{
    void AddIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour);
}

public interface IPotionAddable
{
    void AddPotion(CollectedPotionBehaviour collectedPotionBehaviour);
}

public interface IStackable
{
    void EnableGrab();
    void DisableGrab();
    void GrabMethod(bool grab);
    void DropInTarget(Transform target, Vector3 offset = default);
    Transform GetTransform();
    StackableValuesSo GetStackableValuesSo();
    float GetStackHeight();
}
