using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct IngredientTypeHapticChallenge
{
    [field: SerializeField] public IngredientType IngredientType { get; private set; }
    [field: SerializeField] public HapticChallengeSo HapticChallengeSo { get; private set; }
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
