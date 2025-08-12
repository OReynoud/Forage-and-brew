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

public interface ISeedAddable
{
    void AddSeed(CollectedSeedBehavior collectedSeedBehaviour);
    bool UseEndPoint { get; set; }
    Transform EndPoint { get; set; }
    float heightShove { get; set; }
}



public interface ICinematicInteraction
{
    void StartInteraction();
}


