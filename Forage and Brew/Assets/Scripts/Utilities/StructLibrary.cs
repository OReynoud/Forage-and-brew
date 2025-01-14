using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


[Serializable]
public struct SceneName
{
    [field: SerializeField] public Scene Scene { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
}

[Serializable]
public struct WeatherStateEndProbabilityBySuccessiveDayNumber
{
    [field: SerializeField] [field: Min(1)] public int SuccessiveDayNumber { get; private set; }
    [field: SerializeField] public List<WeatherStateEndProbability> WeatherStateEndProbabilities { get; private set; }
}

[Serializable]
public struct WeatherStateEndProbability
{
    [field: SerializeField] [field: Tooltip("The weather state for the next day.")]
    public WeatherStateSo WeatherStateSo { get; private set; }
    
    [field: SerializeField] [field: Tooltip("The probability to have this weather state for the next day (in per mil).")]
    [field: Range(0f, 1000f)] public float EndProbability { get; set; }
}

[Serializable]
public struct CookedIngredientForm : IEquatable<CookedIngredientForm>
{
    public CookedIngredientForm(IngredientValuesSo ingredient, CookHapticChallengeSo cookedForm)
    {
        IsAType = false;
        Ingredient = ingredient;
        IngredientType = null;
        CookedForm = cookedForm;
    }
    
    public CookedIngredientForm(IngredientTypeSo ingredientType, CookHapticChallengeSo cookedForm)
    {
        IsAType = true;
        Ingredient = null;
        IngredientType = ingredientType;
        CookedForm = cookedForm;
    }
    
    [field: SerializeField] public bool IsAType { get; private set; }
    [field: AllowNesting] [field: HideIf("IsAType")] [field: SerializeField] public IngredientValuesSo Ingredient { get; private set; }
    [field: AllowNesting] [field: ShowIf("IsAType")] [field: SerializeField] public IngredientTypeSo IngredientType { get; private set; }
    [field: SerializeField] public CookHapticChallengeSo CookedForm { get; private set; }

    public bool Equals(CookedIngredientForm other)
    {
        return IsAType == other.IsAType && Equals(Ingredient, other.Ingredient) && IngredientType == other.IngredientType && Equals(CookedForm, other.CookedForm);
    }

    public override bool Equals(object obj)
    {
        return obj is CookedIngredientForm other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsAType, Ingredient, IngredientType, CookedForm);
    }
}

[Serializable]
public struct TemperatureChallengeIngredients
{
    public TemperatureChallengeIngredients(List<CookedIngredientForm> cookedIngredients, Temperature temperature)
    {
        CookedIngredients = cookedIngredients;
        Temperature = temperature;
    }
    
    [field: SerializeField] public List<CookedIngredientForm> CookedIngredients { get; set; }
    [field: SerializeField] public Temperature Temperature { get; set; }
}

[Serializable]
public struct StirCameraAndDuration
{
    [field: SerializeField] public CameraPreset Camera { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float Duration { get; private set; }
}

[Serializable]
public struct GrindingHapticChallengeCrushInput
{
    [field: SerializeField] [field: Range(1, 2)] public int Input { get; private set; }
    [field: SerializeField] public Vector2 Position { get; private set; }
    
    public GrindingHapticChallengeCrushInput(int input, Vector2 position)
    {
        Input = input;
        Position = position;
    }
}

[Serializable]
public struct FloorIngredient : IEquatable<FloorIngredient>
{
    public FloorIngredient(IngredientValuesSo ingredient, Vector3 position, Quaternion rotation)
    {
        Ingredient = ingredient;
        Position = position;
        Rotation = rotation;
    }
    
    [field: SerializeField] public IngredientValuesSo Ingredient { get; set; }
    [field: SerializeField] public Vector3 Position { get; set; }
    [field: SerializeField] public Quaternion Rotation { get; set; }

    public bool Equals(FloorIngredient other)
    {
        return Equals(Ingredient, other.Ingredient) && Position.Equals(other.Position) && Rotation.Equals(other.Rotation);
    }

    public override bool Equals(object obj)
    {
        return obj is FloorIngredient other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Ingredient, Position, Rotation);
    }
}

[Serializable]
public struct FloorCookedPotion : IEquatable<FloorCookedPotion>
{
    public FloorCookedPotion(PotionValuesSo potion, Vector3 position, Quaternion rotation)
    {
        Potion = potion;
        Position = position;
        Rotation = rotation;
    }
    
    [field: SerializeField] public PotionValuesSo Potion { get; set; }
    [field: SerializeField] public Vector3 Position { get; set; }
    [field: SerializeField] public Quaternion Rotation { get; set; }

    public bool Equals(FloorCookedPotion other)
    {
        return Equals(Potion, other.Potion) && Position.Equals(other.Position) && Rotation.Equals(other.Rotation);
    }

    public override bool Equals(object obj)
    {
        return obj is FloorCookedPotion other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Potion, Position, Rotation);
    }
}
