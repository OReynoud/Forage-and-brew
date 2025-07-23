using UnityEngine;

public interface IIngredientAddable
{
    void AddIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour);
    
    bool UseEndPoint { get; set; }
    Transform EndPoint { get; set; }
    float heightShove { get; set; }
}

public interface IPotionAddable
{
    void AddPotion(CollectedPotionBehaviour collectedPotionBehaviour);
    bool UseEndPoint { get; set; }
    Transform EndPoint { get; set; }
    float heightShove { get; set; }
}

public interface IStackable
{
    void EnableGrab();
    void DisableGrab();
    void GrabMethod(bool grab);
    void DropInTarget(Transform target, bool useEndPoint, Vector3 offset = default);
    Transform GetTransform();
    StackableValuesSo GetStackableValuesSo();
    float GetStackHeight();
    AnimationCurve ShoveRotationCurve { get; set; }
}

public interface ICinematicInteraction
{
    void StartInteraction();
}


