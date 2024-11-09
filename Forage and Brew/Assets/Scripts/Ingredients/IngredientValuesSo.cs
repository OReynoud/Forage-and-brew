using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientValues", menuName = "Ingredients/IngredientValuesSo")]
public class IngredientValuesSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public IngredientType Type { get; private set; }
    [field: SerializeField] [field: EnumFlags] public Biome Biomes { get; private set; }
    [field: SerializeField] [field: EnumFlags] public SpawnLocation SpawnLocations { get; private set; }
    [field: SerializeField] public WeatherStateSo[] WeatherStates { get; private set; }
    [field: SerializeField] public LunarCycleStateSo[] LunarCycleStates { get; private set; }
    [field: SerializeField] public GameObject MeshGameObject { get; private set; }
}
