using System;
using UnityEngine;

[Serializable]
public struct IngredientTypeHapticChallenge
{
    [field: SerializeField] public IngredientType IngredientType { get; private set; }
    [field: SerializeField] public HapticChallengeSo HapticChallengeSo { get; private set; }
}
