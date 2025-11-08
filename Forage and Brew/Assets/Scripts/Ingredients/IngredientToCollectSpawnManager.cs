using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientToCollectSpawnManager : MonoBehaviour
{
    // Singleton
    public static IngredientToCollectSpawnManager Instance { get; private set; }
    
    [SerializeField] private List<SpawnGroupCount> spawnGroupCounts;
    private readonly List<IngredientToCollectBehaviour> _activatedIngredientToCollectBehaviours = new();
    
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
        
        // Clear previous states
        _activatedIngredientToCollectBehaviours.Clear();
        
        // Pick places for the day then activate them
        foreach (SpawnGroupCount spawnGroupParentTransform in spawnGroupCounts) // Iterate through each spawn group
        {
            // Create a list to store unique random indices
            List<int> randomIndices = new();
            
            // Ensure the number of unique random indices matches the spawn count
            for (int i = 0; i < spawnGroupParentTransform.SpawnCount; i++)
            {
                // Randomly select an index from the spawn group
                int randomIndex = Random.Range(0, spawnGroupParentTransform.SpawnGroupTransform.childCount);
                
                // Ensure the random index is unique
                while (randomIndices.Contains(randomIndex))
                {
                    randomIndex = (randomIndex + 1) % spawnGroupParentTransform.SpawnGroupTransform.childCount;
                }
                
                // Add the unique random index to the list
                randomIndices.Add(randomIndex);
            }

            for (int i = 0; i < spawnGroupParentTransform.SpawnGroupTransform.childCount; i++) // Iterate through each child of the spawn group
            {
                Transform childTransform = spawnGroupParentTransform.SpawnGroupTransform.GetChild(i);

                if (childTransform.TryGetComponent(out IngredientToCollectBehaviour ingredientToCollectBehaviour))
                {
                    if (randomIndices.Contains(i)) // If the index matches a random index, activate the ingredient to collect behaviour
                    {
                        _activatedIngredientToCollectBehaviours.Add(ingredientToCollectBehaviour);
                        ingredientToCollectBehaviour.gameObject.SetActive(true);
                    }
                    else // Otherwise, deactivate it
                    {
                        ingredientToCollectBehaviour.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    
    private void ChooseIngredientsToSpawn()
    {
        // If the game has already chosen ingredients today, activate the ingredient to collect behaviours based on remaining ingredients
        if (GameDontDestroyOnLoadManager.Instance.HasChosenIngredientsToday)
        {
            // Iterate through all IngredientToCollectBehaviours in the spawn groups
            foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in spawnGroupCounts
                         .SelectMany(x => x.SpawnGroupTransform.GetComponentsInChildren<IngredientToCollectBehaviour>(true)))
            {
                // Check if the spawn index is in remaining ingredients
                if (GameDontDestroyOnLoadManager.Instance.RemainingIngredientToCollectBehaviours
                    .TryGetValue(ingredientToCollectBehaviour.SpawnIndex, out IngredientValuesSo ingredientValuesSo))
                {
                    // If it is, activate the ingredient to collect behaviour and set the ingredient values
                    ingredientToCollectBehaviour.gameObject.SetActive(true);
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
        
        // If the game has not chosen ingredients today
        List<IngredientValuesSo> ingredientValuesList = new();
        
        // Filter ingredient values based on the current biome, weather state, and lunar cycle state
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
        if (_activatedIngredientToCollectBehaviours.Select(ingredientToCollectBehaviour => ingredientToCollectBehaviour
                .SpawnIndex).Distinct().Count() != _activatedIngredientToCollectBehaviours.Count)
        {
            Debug.LogError("Spawn Indexes are not unique. You need to reassign them.");
        }
#endif

        // Iterate through all activated ingredient to collect behaviours
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in _activatedIngredientToCollectBehaviours)
        {
            List<IngredientValuesSo> localIngredientValuesList = new();
            
            // Filter ingredient values based on the spawn location of the ingredient to collect behaviour
            foreach (IngredientValuesSo ingredientValues in ingredientValuesList)
            {
                if ((ingredientValues.SpawnLocations & ingredientToCollectBehaviour.SpawnLocation) != 0)
                {
                    localIngredientValuesList.Add(ingredientValues);
                }
            }
            
            // If there are ingredient values available for the spawn location, assign a random one to the ingredient to collect behaviour
            if (localIngredientValuesList.Count > 0)
            {
                ingredientToCollectBehaviour.IngredientValuesSo = localIngredientValuesList[Random.Range(0, localIngredientValuesList.Count)];
                ingredientToCollectBehaviour.SpawnMesh();
                
                // Add the ingredient to collect behaviour to the remaining ingredient to collect behaviours
                GameDontDestroyOnLoadManager.Instance.RemainingIngredientToCollectBehaviours
                    .Add(ingredientToCollectBehaviour.SpawnIndex, ingredientToCollectBehaviour.IngredientValuesSo);
            }
            else
            {
                // If there are no ingredient values available for the spawn location, deactivate the ingredient to collect behaviour
                ingredientToCollectBehaviour.gameObject.SetActive(false);
            }
        }
        
        // Set the flag that ingredients have been chosen today
        GameDontDestroyOnLoadManager.Instance.HasChosenIngredientsToday = true;
    }
}
