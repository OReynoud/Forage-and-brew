using System;
using System.Collections.Generic;
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
    [field: SerializeField] public IngredientValuesSo Ingredient { get; private set; }
    [field: SerializeField] public CookHapticChallengeSo CookedForm { get; private set; }
}

[Serializable]
public struct CauldronHapticChallengeIngredients
{
    [field: SerializeField] public List<CookedIngredientForm> CookedIngredients { get; private set; }
    [field: SerializeField] public CauldronHapticChallengeSo CauldronHapticChallengeSo { get; private set; }
}
