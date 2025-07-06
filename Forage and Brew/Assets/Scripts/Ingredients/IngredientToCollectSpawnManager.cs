using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientToCollectSpawnManager : MonoBehaviour
{
    // Singleton
    public static IngredientToCollectSpawnManager Instance { get; private set; }
    
    [SerializeField] private List<SpawnGroupCount> spawnGroupCounts;
    private readonly List<IngredientToCollectBehaviour> _activatedIngredientToCollectBehaviours = new();
    private readonly List<IngredientToCollectBehaviour> _deactivatedIngredientToCollectBehaviours = new();
    [SerializeField] private int weedSpawnMinCount = 3;
    [SerializeField] private int weedSpawnMaxCount = 7;
    private List<int> _yesterdayRemainingWeeds = new();
    
    [SerializeField] private IngredientListSo ingredientListSo;
    [SerializeField] private Biome biome;
    
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        ChooseSpawnPlaces();
        SpawnWeeds();
        ChooseIngredientsToSpawn();
    }


    private void ChooseSpawnPlaces()
    {
        if (GameDontDestroyOnLoadManager.Instance.HasChosenIngredientsToday) return;
        
        // Clear previous states
        _activatedIngredientToCollectBehaviours.Clear();
        _deactivatedIngredientToCollectBehaviours.Clear();
        
        if (GameDontDestroyOnLoadManager.Instance.DayPassed == 0)
        {
            // For the first day, pick places for the day then activate them
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
                            _deactivatedIngredientToCollectBehaviours.Add(ingredientToCollectBehaviour);
                            ingredientToCollectBehaviour.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        else
        {
            // For subsequent days, activate places based on tomorrow's spawn index
            // Iterate through all IngredientToCollectBehaviours in the spawn groups
            foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in spawnGroupCounts
                         .SelectMany(x => x.SpawnGroupTransform.GetComponentsInChildren<IngredientToCollectBehaviour>(true)))
            {
                // Check if the spawn index is in tomorrow's places to spawn
                if (GameDontDestroyOnLoadManager.Instance.TomorrowPlacesToSpawn.Contains(ingredientToCollectBehaviour.SpawnIndex))
                {
                    // If it is, activate the ingredient to collect behaviour
                    _activatedIngredientToCollectBehaviours.Add(ingredientToCollectBehaviour);
                    ingredientToCollectBehaviour.gameObject.SetActive(true);
                }
                else
                {
                    // If it is not, deactivate the ingredient to collect behaviour
                    _deactivatedIngredientToCollectBehaviours.Add(ingredientToCollectBehaviour);
                    ingredientToCollectBehaviour.gameObject.SetActive(false);
                }
            }
        }
        
        // Clear the tomorrow places to spawn for the next day
        GameDontDestroyOnLoadManager.Instance.TomorrowPlacesToSpawn.Clear();
        
        // Randomly select spawn places for tomorrow
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
                    if (randomIndices.Contains(i)) // If the index matches a random index, add it to tomorrow's places to spawn
                    {
                        GameDontDestroyOnLoadManager.Instance.TomorrowPlacesToSpawn
                            .Add(ingredientToCollectBehaviour.SpawnIndex);
                    }
                }
            }
        }
    }

    private void SpawnWeeds()
    {
        // If the game has already chosen ingredients today, activate the weeds based on remaining weeds
        if (GameDontDestroyOnLoadManager.Instance.HasChosenIngredientsToday)
        {
            // Iterate through all IngredientToCollectBehaviours in the spawn groups
            foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in spawnGroupCounts
                      .SelectMany(x => x.SpawnGroupTransform.GetComponentsInChildren<IngredientToCollectBehaviour>(true)))
            {
                // Check if the spawn index is in remaining weeds
                if (GameDontDestroyOnLoadManager.Instance.RemainingWeeds.Contains(ingredientToCollectBehaviour.SpawnIndex))
                {
                    // If it is, activate the ingredient to collect behaviour and enable weed
                    ingredientToCollectBehaviour.gameObject.SetActive(true);
                    ingredientToCollectBehaviour.EnableWeed();
                }
                else
                {
                    // If it is not, deactivate the ingredient to collect behaviour
                    ingredientToCollectBehaviour.gameObject.SetActive(false);
                }
            }
            
            return;
        }
        
        // If the game has not chosen ingredients today
        // Store the previous day's remaining weeds and clear the current remaining weeds
        _yesterdayRemainingWeeds.Clear();
        _yesterdayRemainingWeeds = GameDontDestroyOnLoadManager.Instance.RemainingWeeds.ToList();
        GameDontDestroyOnLoadManager.Instance.RemainingWeeds.Clear();

        // Remove deactivated ingredient to collect behaviours that are not in tomorrow's places to spawn
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in _deactivatedIngredientToCollectBehaviours.ToList())
        {
            if (!GameDontDestroyOnLoadManager.Instance.TomorrowPlacesToSpawn.Contains(ingredientToCollectBehaviour.SpawnIndex))
            {
                _deactivatedIngredientToCollectBehaviours.Remove(ingredientToCollectBehaviour);
            }
        }
        
        // Randomly select the number of weeds to spawn
        int weedCount = Random.Range(weedSpawnMinCount, weedSpawnMaxCount + 1);
        // Ensure the weed count does not exceed available deactivated ingredients
        weedCount = Mathf.Min(weedCount, _deactivatedIngredientToCollectBehaviours.Count);
        
        // Randomly activate the weeds from the deactivated ingredient to collect behaviours
        for (int i = 0; i < weedCount; i++)
        {
            // Randomly select an index from the deactivated ingredient to collect behaviours
            int randomIndex = Random.Range(0, _deactivatedIngredientToCollectBehaviours.Count);
            IngredientToCollectBehaviour ingredientToCollectBehaviour = _deactivatedIngredientToCollectBehaviours[randomIndex];
            
            // Activate the ingredient to collect behaviour, enable weed, and add it to remaining weeds
            ingredientToCollectBehaviour.gameObject.SetActive(true);
            ingredientToCollectBehaviour.EnableWeed();
            GameDontDestroyOnLoadManager.Instance.RemainingWeeds.Add(ingredientToCollectBehaviour.SpawnIndex);
            
            // Ensure the weed ingredient to collect behaviour is removed from the deactivated list
            _deactivatedIngredientToCollectBehaviours.Remove(ingredientToCollectBehaviour);
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
                    // If it is not, deactivate the ingredient to collect behaviour (if it is not a weed)
                    if (!ingredientToCollectBehaviour.IsWeed)
                    {
                        ingredientToCollectBehaviour.gameObject.SetActive(false);
                    }
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
            // If the ingredient to collect behaviour was a weed yesterday, skip it
            if (_yesterdayRemainingWeeds.Contains(ingredientToCollectBehaviour.SpawnIndex))
            {
                ingredientToCollectBehaviour.gameObject.SetActive(false);
                continue;
            }
            
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
