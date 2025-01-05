using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientValues", menuName = "Ingredients/IngredientValuesSo")]
public class IngredientValuesSo : StackableValuesSo
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] [field: ResizableTextArea]public string Description { get; private set; }
    [field: SerializeField]  public Sprite icon { get; private set; }
    [field: SerializeField] public IngredientTypeSo Type { get; private set; }
    [field: SerializeField] [field: EnumFlags] public Biome Biomes { get; private set; }
    [field: SerializeField] [field: EnumFlags] public SpawnLocation SpawnLocations { get; private set; }
    [field: SerializeField] public WeatherStateSo[] WeatherStates { get; private set; }
    [field: SerializeField] public LunarCycleStateSo[] LunarCycleStates { get; private set; }
    [field: SerializeField] public GameObject MeshGameObject { get; private set; }
}
