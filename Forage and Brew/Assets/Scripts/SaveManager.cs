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
    [SerializeField] private CodexContentManager codexManager;
    [SerializeField] private WeatherManager weatherManager;
    [SerializeField] private LunarCycleManager lunarCycleManager;
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private OrderManager orderManager;
    
    private static string DirectoryPath => Path.Combine(Application.persistentDataPath, "Saves");
    private static string FilePath => Path.Combine(DirectoryPath, "Save.json");
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
        
        SetDirectory();
        LoadGame();
    }
    
    private void OnApplicationQuit()
    {
        if (!isSaveEnabled) return;
        
        SaveGame();
    }
    
    
    private void SetDirectory()
    {
        if (!File.Exists(DirectoryPath))
        {
            Directory.CreateDirectory(DirectoryPath);
        }
    }
    
    public void SaveGame()
    {
        // Scene
        data.PreviousScene = gameDontDestroyOnLoadManager.CurrentScene;
        
        // Days
        data.CurrentTimeOfDay = gameDontDestroyOnLoadManager.CurrentTimeOfDay;
        data.DayPassed = gameDontDestroyOnLoadManager.DayPassed;
        
        // Collected Ingredients
        data.CollectedIngredients = gameDontDestroyOnLoadManager.CollectedIngredients;
        data.FloorCollectedIngredients = new List<FloorIngredient>();
        data.FloorCollectedIngredients.AddRange(gameDontDestroyOnLoadManager.OutCollectedIngredients
            .Select(collectedIngredient => new FloorIngredient(collectedIngredient.IngredientValuesSo,
                collectedIngredient.transform.position, collectedIngredient.transform.rotation)));
        data.FloorCollectedIngredients.AddRange(gameDontDestroyOnLoadManager.FloorCollectedIngredients);
        
        // Cooked Potions
        data.FloorCookedPotions = new List<FloorCookedPotion>();
        data.FloorCookedPotions.AddRange(gameDontDestroyOnLoadManager.OutCookedPotions
            .Select(collectedPotion => new FloorCookedPotion(collectedPotion.PotionValuesSo,
                collectedPotion.transform.position, collectedPotion.transform.rotation)));
        data.FloorCookedPotions.AddRange(gameDontDestroyOnLoadManager.FloorCookedPotions);
        data.OrderPotions = gameDontDestroyOnLoadManager.OrderPotions;
        
        // Unlocked Ingredients and Recipes
        data.UnlockedIngredients = gameDontDestroyOnLoadManager.UnlockedIngredients;
        data.UnlockedRecipes = gameDontDestroyOnLoadManager.UnlockedRecipes;
        
        // Ingredients to Collect
        data.HasChosenIngredientsToday = gameDontDestroyOnLoadManager.HasChosenIngredientsToday;
        data.RemainingIngredientToCollectBehavioursKeys = gameDontDestroyOnLoadManager
            .RemainingIngredientToCollectBehaviours.Keys.ToList();
        data.RemainingIngredientToCollectBehavioursValues = gameDontDestroyOnLoadManager
            .RemainingIngredientToCollectBehaviours.Values.ToList();
        
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
        
        // Money
        data.MoneyAmount = moneyManager.MoneyAmount;
        
        // Orders
        data.CurrentOrders = orderManager.CurrentOrders;
        
        // Save Data
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, jsonData, _encoding);
    }

    public void LoadGame()
    {
        // Check if there is no save file
        if (!File.Exists(FilePath))
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
        string jsonData = File.ReadAllText(FilePath, _encoding);
        data = JsonUtility.FromJson<SavedData>(jsonData);
        
        // Scene
        gameDontDestroyOnLoadManager.CurrentScene = data.PreviousScene;
        foreach (SceneName sceneName in sceneListSo.SceneNames)
        {
            if (sceneName.Scene == data.PreviousScene)
            {
                SceneManager.LoadScene(sceneName.Name);
                SceneTransitionManager.instance.HandleLoadNewScene(sceneName.Scene);
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
        
        // Ingredients to Collect
        gameDontDestroyOnLoadManager.HasChosenIngredientsToday = data.HasChosenIngredientsToday;
        foreach (int key in data.RemainingIngredientToCollectBehavioursKeys)
        {
            gameDontDestroyOnLoadManager.RemainingIngredientToCollectBehaviours.Add(key,
                data.RemainingIngredientToCollectBehavioursValues
                    [data.RemainingIngredientToCollectBehavioursKeys.IndexOf(key)]);
        }
        
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
            weatherManager.CurrentWeatherStates.Add(key,
                data.CurrentWeatherStatesValues[data.CurrentWeatherStatesKeys.IndexOf(key)]);
        }
        
        lunarCycleManager.CurrentLunarCycleStateIndex = data.CurrentLunarCycleStateIndex;
        
        // Money
        moneyManager.MoneyAmount = data.MoneyAmount;
        
        // Orders
        orderManager.CurrentOrders.AddRange(data.CurrentOrders);
    }
    
    public static void DeleteSave(bool isOnMainMenu)
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
        
        if (isOnMainMenu) return;
        
        SceneManager.LoadScene("SC_MainMenu");

        if (Instance)
        {
            DestroyImmediate(Instance.gameObject);
        }
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
        
        [field: SerializeField] public bool HasChosenIngredientsToday { get; set; }
        [field: SerializeField] public List<int> RemainingIngredientToCollectBehavioursKeys { get; set; }
        [field: SerializeField] public List<IngredientValuesSo> RemainingIngredientToCollectBehavioursValues { get; set; }
        
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
        
        [field: SerializeField] public int MoneyAmount { get; set; }
        
        [field: SerializeField] public List<Order> CurrentOrders { get; set; }
    
    }
}
