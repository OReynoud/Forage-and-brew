using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientToCollectSpawnManager : MonoBehaviour
{
    // Singleton
    public static IngredientToCollectSpawnManager Instance { get; private set; }
    
    public List<IngredientToCollectBehaviour> IngredientToCollectBehaviours { get; } = new();
    
    [SerializeField] private IngredientListSo ingredientListSo;
    [SerializeField] private Biome biome;
    
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        if (GameDontDestroyOnLoadManager.Instance.HasChosenIngredientsToday)
        {
            foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in IngredientToCollectBehaviours)
            {
                if (GameDontDestroyOnLoadManager.Instance.RemainingIngredientToCollectBehaviours
                    .TryGetValue(ingredientToCollectBehaviour.SpawnIndex, out IngredientValuesSo ingredientValuesSo))
                {
                    ingredientToCollectBehaviour.IngredientValuesSo = ingredientValuesSo;
                    ingredientToCollectBehaviour.SpawnMesh();
                }
                else
                {
                    ingredientToCollectBehaviour.gameObject.SetActive(false);
                }
            }
            
            return;
        }
        
        List<IngredientValuesSo> ingredientValuesList = new();
        
        foreach (IngredientValuesSo ingredientValues in ingredientListSo.IngredientValues)
        {
            if ((ingredientValues.Biomes & biome) != 0 &&
                ingredientValues.WeatherStates.Contains(WeatherManager.Instance.CurrentWeatherStates[biome].WeatherStateSo) &&
                ingredientValues.LunarCycleStates.Contains(LunarCycleManager.Instance.CurrentLunarCycleState))
            {
                ingredientValuesList.Add(ingredientValues);
            }
        }

#if UNITY_EDITOR
        if (IngredientToCollectBehaviours.Select(ingredientToCollectBehaviour => ingredientToCollectBehaviour
                .SpawnIndex).Distinct().Count() != IngredientToCollectBehaviours.Count)
        {
            Debug.LogError("Spawn Indexes are not unique. You need to reassign them.");
        }
#endif

        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in IngredientToCollectBehaviours)
        {
            List<IngredientValuesSo> localIngredientValuesList = new();
            
            foreach (IngredientValuesSo ingredientValues in ingredientValuesList)
            {
                if ((ingredientValues.SpawnLocations & ingredientToCollectBehaviour.SpawnLocation) != 0)
                {
                    localIngredientValuesList.Add(ingredientValues);
                }
            }
            
            if (localIngredientValuesList.Count > 0)
            {
                ingredientToCollectBehaviour.IngredientValuesSo = localIngredientValuesList[Random.Range(0, localIngredientValuesList.Count)];
                ingredientToCollectBehaviour.SpawnMesh();
                
                GameDontDestroyOnLoadManager.Instance.RemainingIngredientToCollectBehaviours
                    .Add(ingredientToCollectBehaviour.SpawnIndex, ingredientToCollectBehaviour.IngredientValuesSo);
            }
            else
            {
                ingredientToCollectBehaviour.gameObject.SetActive(false);
            }
        }
        
        GameDontDestroyOnLoadManager.Instance.HasChosenIngredientsToday = true;
    }
}
