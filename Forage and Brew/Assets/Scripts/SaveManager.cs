using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    // Singleton
    public static SaveManager Instance { get; private set; }
    
    [Header("Debug")]
    [SerializeField] private bool isSaveEnabled;
    
    [Header("Required Data")]
    [SerializeField] private SceneListSo sceneListSo;
    
    [Header("Dependencies")]
    [SerializeField] private GameDontDestroyOnLoadManager gameDontDestroyOnLoadManager;
    [SerializeField] private WeatherManager weatherManager;
    [SerializeField] private LunarCycleManager lunarCycleManager;
    
    private string _directoryPath;
    private string _filePath;
    private readonly Encoding _encoding = Encoding.UTF8;
    
    [SerializeField, HideInInspector] private SavedData data = new();
    
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
        
        if (!isSaveEnabled) return;
        
        SetPaths();
        LoadGame();
    }
    
    private void OnApplicationQuit()
    {
        if (!isSaveEnabled) return;
        
        SaveGame();
    }
    
    
    private void SetPaths()
    {
        _directoryPath = Path.Combine(Application.persistentDataPath, "Saves");
        _filePath = Path.Combine(_directoryPath, "Save.json");
        
        if (!File.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }
    }
    
    public void SaveGame()
    {
        // Scene
        data.PreviousScene = gameDontDestroyOnLoadManager.PreviousScene;
        
        // Days
        data.CurrentTimeOfDay = gameDontDestroyOnLoadManager.CurrentTimeOfDay;
        data.DayPassed = gameDontDestroyOnLoadManager.DayPassed;
        
        // Ingredients
        data.CollectedIngredients = gameDontDestroyOnLoadManager.CollectedIngredients;
        data.FloorCollectedIngredients = new List<FloorIngredient>();
        data.FloorCollectedIngredients.AddRange(gameDontDestroyOnLoadManager.OutCollectedIngredients
            .Select(collectedIngredient => new FloorIngredient(collectedIngredient.IngredientValuesSo,
                collectedIngredient.transform.position, collectedIngredient.transform.rotation)));
        data.FloorCollectedIngredients.AddRange(gameDontDestroyOnLoadManager.FloorCollectedIngredients);
        
        // Potions
        data.FloorCookedPotions = new List<FloorCookedPotion>();
        data.FloorCookedPotions.AddRange(gameDontDestroyOnLoadManager.OutCookedPotions
            .Select(collectedPotion => new FloorCookedPotion(collectedPotion.PotionValuesSo,
                collectedPotion.transform.position, collectedPotion.transform.rotation)));
        data.FloorCookedPotions.AddRange(gameDontDestroyOnLoadManager.FloorCookedPotions);
        data.OrderPotions = gameDontDestroyOnLoadManager.OrderPotions;
        
        // Unlocked Ingredients and Recipes
        data.UnlockedIngredients = gameDontDestroyOnLoadManager.UnlockedIngredients;
        data.UnlockedRecipes = gameDontDestroyOnLoadManager.UnlockedRecipes;
        
        // Letters
        data.HasChosenLettersToday = gameDontDestroyOnLoadManager.HasChosenLettersToday;
        data.QuestProgressionIndex = gameDontDestroyOnLoadManager.QuestProgressionIndex;
        data.AllNarrativeBlocks = gameDontDestroyOnLoadManager.AllNarrativeBlocks;
        data.ThanksAndErrorLetters = gameDontDestroyOnLoadManager.ThanksAndErrorLetters;
        data.MailBoxLetters = gameDontDestroyOnLoadManager.MailBoxLetters;
        
        // Cauldron
        data.CauldronTemperatureAndIngredients = gameDontDestroyOnLoadManager.CauldronTemperatureAndIngredients;
        data.CauldronTemperature = gameDontDestroyOnLoadManager.CauldronTemperature;
        
        // Cycles
        data.CurrentWeatherStatesKeys = weatherManager.CurrentWeatherStates.Keys.ToList();
        data.CurrentWeatherStatesValues = weatherManager.CurrentWeatherStates.Values.ToList();
        data.CurrentLunarCycleStateIndex = lunarCycleManager.CurrentLunarCycleStateIndex;
        
        // Save Data
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(_filePath, jsonData, _encoding);
    }

    public void LoadGame()
    {
        // Check if there is no save file
        if (!File.Exists(_filePath))
        {
            foreach (NarrativeBlockOfLettersContentSo contentSo in gameDontDestroyOnLoadManager.AllNarrativeBlocksContentSo)
            {
                gameDontDestroyOnLoadManager.AllNarrativeBlocks.Add(new NarrativeBlockOfLetters(contentSo));
            }
            
            return;
        }
        
        // Global Information
        gameDontDestroyOnLoadManager.IsFirstGameSession = false;
        
        // Load Data
        string jsonData = File.ReadAllText(_filePath, _encoding);
        data = JsonUtility.FromJson<SavedData>(jsonData);
        
        // Scene
        gameDontDestroyOnLoadManager.PreviousScene = data.PreviousScene;
        foreach (SceneName sceneName in sceneListSo.SceneNames)
        {
            if (sceneName.Scene == data.PreviousScene)
            {
                SceneManager.LoadScene(sceneName.Name);
                SceneTransitionManager.instance.HandleLoadNewScene();
                break;
            }
        }
        
        // Days
        gameDontDestroyOnLoadManager.CurrentTimeOfDay = data.CurrentTimeOfDay;
        gameDontDestroyOnLoadManager.DayPassed = data.DayPassed;
        
        // Ingredients
        gameDontDestroyOnLoadManager.CollectedIngredients.AddRange(data.CollectedIngredients);
        gameDontDestroyOnLoadManager.FloorCollectedIngredients.AddRange(data.FloorCollectedIngredients);
        
        // Potions
        gameDontDestroyOnLoadManager.FloorCookedPotions.AddRange(data.FloorCookedPotions);
        gameDontDestroyOnLoadManager.OrderPotions.AddRange(data.OrderPotions);
        
        // Unlocked Ingredients and Recipes
        gameDontDestroyOnLoadManager.UnlockedIngredients.AddRange(data.UnlockedIngredients);
        gameDontDestroyOnLoadManager.UnlockedRecipes.AddRange(data.UnlockedRecipes);
        
        // Letters
        gameDontDestroyOnLoadManager.HasChosenLettersToday = data.HasChosenLettersToday;
        gameDontDestroyOnLoadManager.QuestProgressionIndex = data.QuestProgressionIndex;
        gameDontDestroyOnLoadManager.AllNarrativeBlocks.AddRange(data.AllNarrativeBlocks);
        gameDontDestroyOnLoadManager.ThanksAndErrorLetters.AddRange(data.ThanksAndErrorLetters);
        gameDontDestroyOnLoadManager.MailBoxLetters.AddRange(data.MailBoxLetters);
        
        // Cauldron
        gameDontDestroyOnLoadManager.CauldronTemperatureAndIngredients.AddRange(data.CauldronTemperatureAndIngredients);
        gameDontDestroyOnLoadManager.CauldronTemperature = data.CauldronTemperature;
        
        // Cycles
        foreach (Biome key in data.CurrentWeatherStatesKeys)
        {
            weatherManager.CurrentWeatherStates.Add(key, data.CurrentWeatherStatesValues[data.CurrentWeatherStatesKeys.IndexOf(key)]);
        }
        
        lunarCycleManager.CurrentLunarCycleStateIndex = data.CurrentLunarCycleStateIndex;
    }
    
    
    [Serializable]
    public class SavedData
    {
        [field: SerializeField] public Scene PreviousScene { get; set; }
        
        [field: SerializeField] public TimeOfDay CurrentTimeOfDay { get; set; }
        [field: SerializeField] public int DayPassed { get; set; }
        
        [field: SerializeField] public List<IngredientValuesSo> CollectedIngredients { get; set; }
        [field: SerializeField] public List<FloorIngredient> FloorCollectedIngredients { get; set; }
        
        [field: SerializeField] public List<FloorCookedPotion> FloorCookedPotions { get; set; }
        [field: SerializeField] public List<ClientOrderPotions> OrderPotions { get; set; }
        
        [field: SerializeField] public List<IngredientValuesSo> UnlockedIngredients { get; set; }
        [field: SerializeField] public List<PotionValuesSo> UnlockedRecipes { get; set; }
        
        [field: SerializeField] public bool HasChosenLettersToday { get; set; }
        [field: SerializeField] public int QuestProgressionIndex { get; set; }
        [field: SerializeField] public List<NarrativeBlockOfLetters> AllNarrativeBlocks { get; set; }
        [field: SerializeField] public List<Letter> ThanksAndErrorLetters { get; set; }
        [field: SerializeField] public List<Letter> MailBoxLetters { get; set; }
        
        [field: SerializeField] public List<TemperatureChallengeIngredients> CauldronTemperatureAndIngredients { get; set; }
        [field: SerializeField] public Temperature CauldronTemperature { get; set; }
        
        [field: SerializeField] public List<Biome> CurrentWeatherStatesKeys { get; set; }
        [field: SerializeField] public List<WeatherSuccessiveDays> CurrentWeatherStatesValues { get; set; }
        [field: SerializeField] public int CurrentLunarCycleStateIndex { get; set; }
    }
}
