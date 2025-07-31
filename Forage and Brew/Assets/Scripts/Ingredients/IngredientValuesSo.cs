using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "D_IngredientValues", menuName = "Ingredients/IngredientValuesSo")]
public class IngredientValuesSo : StackableValuesSo
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] [field: ResizableTextArea]public string Description { get; private set; }
    [field: SerializeField]  public Sprite iconLow { get; private set; }
    [field: SerializeField]  public Sprite iconHigh { get; private set; }
    [field: SerializeField] public IngredientTypeSo Type { get; private set; }
    [field: SerializeField] [field: EnumFlags] public Biome Biomes { get; private set; }
    [field: SerializeField] [field: EnumFlags] public SpawnLocation SpawnLocations { get; private set; }
    [field: SerializeField] public WeatherStateSo[] WeatherStates { get; private set; }
    [field: SerializeField] public LunarCycleStateSo[] LunarCycleStates { get; private set; }
    [field: SerializeField] public GameObject MeshGameObject { get; private set; }
    [field: ShowIf("IsChoppable")] [field: SerializeField] public GameObject CutMeshGameObject { get; private set; }
    [field: ShowIf("IsChoppable")] [field: SerializeField] public List<Vector3> CutMeshPositions { get; private set; }
    [field: ShowIf("IsChoppable")] [field: SerializeField] public List<Vector3> CutMeshRotations { get; private set; }
    [field: ShowIf("IsChoppable")] [field: SerializeField] public List<Vector3> CutMeshEndPositions { get; private set; }
    [field: ShowIf("IsChoppable")] [field: SerializeField] public List<Vector3> CutMeshEndRotations { get; private set; }

    private bool IsChoppable => Type?.IsChoppable ?? false;
    

    public void SetData(string newName, string newDescription, List<WeatherStateSo> newWeatherStates,
        List<LunarCycleStateSo> newLunarCycleStates, Biome newBiomes, SpawnLocation newSpawnLocations)
    {
        Name = newName;
        Description = newDescription;
        WeatherStates = newWeatherStates.ToArray();
        LunarCycleStates = newLunarCycleStates.ToArray();
        Biomes = newBiomes;
        SpawnLocations = newSpawnLocations;
    }
    
    
    private void OnValidate()
    {
        if (!IsChoppable) return;
        
        while (CutMeshPositions.Count < 4)
        {
            CutMeshPositions.Add(Vector3.zero);
        }

        while (CutMeshPositions.Count > 4)
        {
            CutMeshPositions.RemoveAt(CutMeshPositions.Count - 1);
        }
        
        while (CutMeshRotations.Count < 4)
        {
            CutMeshRotations.Add(Vector3.zero);
        }
        
        while (CutMeshRotations.Count > 4)
        {
            CutMeshRotations.RemoveAt(CutMeshRotations.Count - 1);
        }
        
        while (CutMeshEndPositions.Count < 5)
        {
            CutMeshEndPositions.Add(Vector3.zero);
        }
        
        while (CutMeshEndPositions.Count > 5)
        {
            CutMeshEndPositions.RemoveAt(CutMeshEndPositions.Count - 1);
        }
        
        while (CutMeshEndRotations.Count < 5)
        {
            CutMeshEndRotations.Add(Vector3.zero);
        }
        
        while (CutMeshEndRotations.Count > 5)
        {
            CutMeshEndRotations.RemoveAt(CutMeshEndRotations.Count - 1);
        }
    }
}
