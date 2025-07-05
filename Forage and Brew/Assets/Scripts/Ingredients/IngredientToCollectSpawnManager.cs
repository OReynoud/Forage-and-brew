using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientToCollectSpawnManager : MonoBehaviour
{
    // Singleton
    public static IngredientToCollectSpawnManager Instance { get; private set; }
    
    [SerializeField] private List<Transform> spawnGroupParentTransforms;
    [SerializeField] private int spawnProbabilityBase = 3;
    private readonly List<IngredientToCollectBehaviour> _ingredientToCollectBehaviours = new();
    
    [SerializeField] private IngredientListSo ingredientListSo;
    [SerializeField] private Biome biome;
    
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        ChooseSpawnPlaces();
        ChooseIngredientsToSpawn();
    }

    
    private void ChooseSpawnPlaces()
    {
        if (GameDontDestroyOnLoadManager.Instance.HasChosenIngredientsToday) return;
        
        _ingredientToCollectBehaviours.Clear();
        
        foreach (Transform spawnGroupParentTransform in spawnGroupParentTransforms)
        {
            int randomIndex = Random.Range(0, spawnProbabilityBase);

            for (int i = 0; i < spawnGroupParentTransform.childCount; i++)
            {
                Transform childTransform = spawnGroupParentTransform.GetChild(i);
                
                if (childTransform.TryGetComponent(out IngredientToCollectBehaviour ingredientToCollectBehaviour))
                {
                    if (i == randomIndex)
                    {
                        _ingredientToCollectBehaviours.Add(ingredientToCollectBehaviour);
                        ingredientToCollectBehaviour.gameObject.SetActive(true);
                    }
                    else
                    {
                        ingredientToCollectBehaviour.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    
    private void ChooseIngredientsToSpawn()
    {
        if (GameDontDestroyOnLoadManager.Instance.HasChosenIngredientsToday)
        {
            foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in _ingredientToCollectBehaviours)
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
                ingredientValues.WeatherStates.Contains(WeatherManager.Instance.CurrentWeatherState.WeatherStateSo) &&
                ingredientValues.LunarCycleStates.Contains(LunarCycleManager.Instance.CurrentLunarCycleState))
            {
                ingredientValuesList.Add(ingredientValues);
            }
        }

#if UNITY_EDITOR
        if (_ingredientToCollectBehaviours.Select(ingredientToCollectBehaviour => ingredientToCollectBehaviour
                .SpawnIndex).Distinct().Count() != _ingredientToCollectBehaviours.Count)
        {
            Debug.LogError("Spawn Indexes are not unique. You need to reassign them.");
        }
#endif

        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in _ingredientToCollectBehaviours)
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
