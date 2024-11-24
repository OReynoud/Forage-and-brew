using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public struct IngredientTypeHapticChallenge
{
    [field: SerializeField] public IngredientType IngredientType { get; private set; }
    [field: SerializeField] public CollectHapticChallengeSo CollectHapticChallengeSo { get; private set; }
}

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
public struct CookedIngredientForm
{
    public CookedIngredientForm(IngredientValuesSo ingredient, CookHapticChallengeSo cookedForm)
    {
        IsAType = false;
        Ingredient = ingredient;
        IngredientType = default;
        CookedForm = cookedForm;
    }
    
    public CookedIngredientForm(IngredientType ingredientType, CookHapticChallengeSo cookedForm)
    {
        IsAType = true;
        Ingredient = default;
        IngredientType = ingredientType;
        CookedForm = cookedForm;
    }
    
    [field: SerializeField] public bool IsAType { get; private set; }
    [field: AllowNesting] [field: HideIf("IsAType")] [field: SerializeField] public IngredientValuesSo Ingredient { get; private set; }
    [field: AllowNesting] [field: ShowIf("IsAType")] [field: SerializeField] public IngredientType IngredientType { get; private set; }
    [field: SerializeField] public CookHapticChallengeSo CookedForm { get; private set; }
}

[Serializable]
public struct TemperatureChallengeIngredients
{
    public TemperatureChallengeIngredients(List<CookedIngredientForm> cookedIngredients, Temperature temperature)
    {
        CookedIngredients = cookedIngredients;
        Temperature = temperature;
    }
    
    [field: SerializeField] public List<CookedIngredientForm> CookedIngredients { get; private set; }
    [field: SerializeField] public Temperature Temperature { get; private set; }
}

[Serializable]
public struct HapticChallengeMovementDirectionRectTransform
{
    [field: SerializeField] public HapticChallengeMovementDirection HapticChallengeMovementDirection { get; private set; }
    [field: SerializeField] public RectTransform RectTransform { get; private set; }
}

[Serializable]
public struct HapticChallengeMovementDirectionProbability
{
    [field: SerializeField] public HapticChallengeMovementDirection HapticChallengeMovementDirection { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float Probability { get; private set; }
}

[Serializable]
public struct HapticChallengeGaugeParts
{
    [field: SerializeField] [field: Range(0f, 1f)] public float CorrectGaugeMinValue { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float CorrectGaugeMaxValue { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float PerfectGaugeMinValue { get; private set; }
    [field: SerializeField] [field: Range(0f, 1f)] public float PerfectGaugeMaxValue { get; private set; }
}
