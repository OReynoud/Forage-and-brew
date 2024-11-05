using System;
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
