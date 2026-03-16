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
    [SerializeField] public bool isSaveEnabled;
    
    [Header("Required Data")]
    [SerializeField] private SceneListSo sceneListSo;
    
    [Header("Dependencies")]
    [SerializeField] private GameDontDestroyOnLoadManager gameDontDestroyOnLoadManager;
    [SerializeField] private CodexContentManager codexManager;
    [SerializeField] private WeatherManager weatherManager;
    [SerializeField] private LunarCycleManager lunarCycleManager;
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private OrderManager orderManager;
    [SerializeField] private CharacterInputManager inputManager;
    
    private static string DirectoryPath => Path.Combine(Application.persistentDataPath, "Saves");
    public static string FilePath => Path.Combine(DirectoryPath, "Save.json");
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
        // Debug.Log("Bro saved");
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
                collectedIngredient.CookedForm, collectedIngredient.transform.position,
                collectedIngredient.transform.rotation)));
        data.FloorCollectedIngredients.AddRange(gameDontDestroyOnLoadManager.FloorCollectedIngredients);
        
        // Cooked Potions
        data.FloorCookedPotions = new List<FloorCookedPotion>();
        data.FloorCookedPotions.AddRange(gameDontDestroyOnLoadManager.OutCookedPotions
            .Select(collectedPotion => new FloorCookedPotion(collectedPotion.PotionValuesSo,
                collectedPotion.transform.position, collectedPotion.transform.rotation)));
        data.FloorCookedPotions.AddRange(gameDontDestroyOnLoadManager.FloorCookedPotions);
        data.OrderPotions = new List<ClientOrderPotions>();
        data.OrderPotions.AddRange(PotionCrateManager.Instance.PotionCrates
            .Select(potionCrate => new ClientOrderPotions(potionCrate.OrderContentSo, potionCrate.ClientSo,
                potionCrate.ContainedPotions.Select(potion => new FloorCookedPotion(potion.PotionValuesSo,
                    potion.transform.position, potion.transform.rotation)).ToList())));
        data.OrderPotions.AddRange(gameDontDestroyOnLoadManager.OrderPotions);
        
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
        data.FillerQuestProgression = gameDontDestroyOnLoadManager.FillerQuestProgression;
        data.AllNarrativeBlocks = gameDontDestroyOnLoadManager.AllNarrativeBlocks;
        data.AllFillerBlocks = gameDontDestroyOnLoadManager.AllFillerBlocks;
        data.LastUsedFillerBlock = gameDontDestroyOnLoadManager.LastUsedFillerBlockOfLetters;
        data.QuestProgressionIndexWatchers = gameDontDestroyOnLoadManager.QuestProgressionIndexWatchers;
        data.ThanksAndErrorLetters = gameDontDestroyOnLoadManager.ThanksAndErrorLetters;
        data.MailBoxLetters = gameDontDestroyOnLoadManager.MailBoxLetters;
        data.ChosenLetters.Clear();
        foreach (var tuple in gameDontDestroyOnLoadManager.ChosenLetters)
        {
            data.ChosenLetters.Add(new ChosenLetter()
            {
                VisualLetterComponent = tuple.Item1,
                DataLetterComponent = tuple.Item2
            });
        }
        // Debug.Log(data.ChosenLetters.Count);
            
        
        // Cauldron
        data.CauldronTemperatureAndIngredients = gameDontDestroyOnLoadManager.CauldronTemperatureAndIngredients;
        data.CauldronTemperature = gameDontDestroyOnLoadManager.CauldronTemperature;
        
        // Cycles
        data.CurrentWeatherState = weatherManager.CurrentWeatherState;
        data.CurrentLunarCycleStateIndex = lunarCycleManager.CurrentLunarCycleStateIndex;
        
        // Money
        data.MoneyAmount = moneyManager.MoneyAmount;
        
        // Orders
        data.OrdersContentList = new ();
        data.OrdersRelatedNBList = new ();
        data.OrdersRelatedLetterList = new();
        foreach (var order in orderManager.CurrentOrders)
        {
            if (order == null)
            {
                data.OrdersContentList.Add(null);
                data.OrdersRelatedNBList.Add(null);
                data.OrdersRelatedLetterList.Add(null);
                continue;
            }
            data.OrdersContentList.Add(order.OrderContent);
            data.OrdersRelatedNBList.Add(order.RelatedNarrativeBlock);
            data.OrdersRelatedLetterList.Add(order.RelatedLetter);
        }
        
        //Tutorial progression
        data.CodexIsUnlocked = gameDontDestroyOnLoadManager.codexIsUnlocked;
        data.HasDonePinTutorial = gameDontDestroyOnLoadManager.hasDonePinTutorial;
        
        data.UnlockedTutorials = gameDontDestroyOnLoadManager.UnlockedTutorials;
        
        //Garden
        data.PlotsData = gameDontDestroyOnLoadManager.plotsData;
        
        //Mirror
        data.CurrentOutfit = gameDontDestroyOnLoadManager.CurrentOutfitSo;
        data.UnlockedOutifts = gameDontDestroyOnLoadManager.UnlockedOutfits;
        
        //Progression
        data.WorkshopProgressionIndex = gameDontDestroyOnLoadManager.WorkshopProgressionIndex;
        data.UnlockedChargedBiomeAreas = gameDontDestroyOnLoadManager.UnlockedChargedBiomeAreas;
        
        // Save Data
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, jsonData, _encoding);
    }
    public void LoadGame()
    {
        
        // Check if there is no save file
        if (!File.Exists(FilePath) || !isSaveEnabled)
        {
            foreach (NarrativeBlockOfLettersContentSo contentSo in gameDontDestroyOnLoadManager.AllNarrativeBlocksContentSo)
            {
                gameDontDestroyOnLoadManager.AllNarrativeBlocks.Add(new NarrativeBlockOfLetters(contentSo));
            }
            foreach (FillerBlockLettersContentSo contentSo in gameDontDestroyOnLoadManager.AllFillerBlocksContentSo)
            {
                gameDontDestroyOnLoadManager.AllFillerBlocks.Add(new FillerBlockOfLetters(contentSo, false, null));
            }
            
            inputManager.SetupInputs();
            return;
        }
        // Debug.Log("Bro loaded");
        // Global Information
        gameDontDestroyOnLoadManager.IsFirstGameSession = false;
        
        // Load Data
        string jsonData = File.ReadAllText(FilePath, _encoding);
        data = JsonUtility.FromJson<SavedData>(jsonData);


        // cam.ApplyScriptableCamSettings(cam.TargetCamSettings, 0);
        // cam.InstantCamUpdate(cam.TargetCamSettings);

        // Scene
        gameDontDestroyOnLoadManager.CurrentScene = data.PreviousScene;
        // foreach (SceneName sceneName in sceneListSo.SceneNames)
        // {
        //     if (sceneName.Scene == data.PreviousScene)
        //     {
        //         SceneManager.LoadScene(sceneName.Name);
        //         SceneTransitionManager.instance.HandleLoadNewScene(sceneName.Scene);
        //         break;
        //     }
        // }
        
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
        gameDontDestroyOnLoadManager.FillerQuestProgression = data.FillerQuestProgression;
        gameDontDestroyOnLoadManager.AllNarrativeBlocks.AddRange(data.AllNarrativeBlocks);
        gameDontDestroyOnLoadManager.AllFillerBlocks.AddRange(data.AllFillerBlocks);
        gameDontDestroyOnLoadManager.LastUsedFillerBlockOfLetters = data.LastUsedFillerBlock;
        gameDontDestroyOnLoadManager.QuestProgressionIndexWatchers.AddRange(data.QuestProgressionIndexWatchers);
        gameDontDestroyOnLoadManager.ThanksAndErrorLetters.AddRange(data.ThanksAndErrorLetters);
        gameDontDestroyOnLoadManager.MailBoxLetters.AddRange(data.MailBoxLetters);
        foreach (var chosenLetter in data.ChosenLetters)
        {
            gameDontDestroyOnLoadManager.ChosenLetters.Add((chosenLetter.VisualLetterComponent,chosenLetter.DataLetterComponent));
        }
        
        // Cauldron
        gameDontDestroyOnLoadManager.CauldronTemperatureAndIngredients.AddRange(data.CauldronTemperatureAndIngredients);
        gameDontDestroyOnLoadManager.CauldronTemperature = data.CauldronTemperature;
        
        // Cycles
        weatherManager.CurrentWeatherState = data.CurrentWeatherState;
        lunarCycleManager.CurrentLunarCycleStateIndex = data.CurrentLunarCycleStateIndex;
        
        // Money
        gameDontDestroyOnLoadManager.moneyAmountOnStart = data.MoneyAmount;
        
        // Orders
        for (int i = 0; i < data.OrdersContentList.Count; i++)
        {
            orderManager.CurrentOrders.Add(new Order(data.OrdersContentList[i], data.OrdersRelatedNBList[i], data.OrdersRelatedLetterList[i]));
        }
        
        // Tutorial progression
        gameDontDestroyOnLoadManager.codexIsUnlocked = data.CodexIsUnlocked;
        if (data.CodexIsUnlocked)
        {
            codexManager.codexIsUnlocked = true;
            codexManager.tutorialDissolvesToCheck.Clear();
        }
        gameDontDestroyOnLoadManager.hasDonePinTutorial = data.HasDonePinTutorial;
        
        gameDontDestroyOnLoadManager.UnlockedTutorials = data.UnlockedTutorials;
        
        //Garden
        gameDontDestroyOnLoadManager.plotsData = data.PlotsData;
        
        //Mirror
        gameDontDestroyOnLoadManager.CurrentOutfitSo = data.CurrentOutfit;
        gameDontDestroyOnLoadManager.UnlockedOutfits.AddRange(data.UnlockedOutifts);
        
        //Progression
        gameDontDestroyOnLoadManager.WorkshopProgressionIndex = data.WorkshopProgressionIndex;
        gameDontDestroyOnLoadManager.UnlockedChargedBiomeAreas.AddRange(data.UnlockedChargedBiomeAreas);
        
        foreach (SceneName sceneName in sceneListSo.SceneNames)
        {
            if (sceneName.Scene == data.PreviousScene)
            {
                SceneManager.LoadScene(sceneName.Name);
                //SceneTransitionManager.instance.HandleLoadNewScene(sceneName.Scene);
                break;
            }
        }

        CodexPickUpBehaviour.doTutorialPages = false;
        inputManager.SetupInputs();
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
        [field: SerializeField] public List<string> UnlockedTutorials { get; set; }
        
        [field: SerializeField] public bool HasChosenIngredientsToday { get; set; }
        [field: SerializeField] public List<int> RemainingIngredientToCollectBehavioursKeys { get; set; }
        [field: SerializeField] public List<IngredientValuesSo> RemainingIngredientToCollectBehavioursValues { get; set; }
        
        [field: SerializeField] public bool HasChosenLettersToday { get; set; }
        [field: SerializeField] public int QuestProgressionIndex { get; set; }
        [field: SerializeField] public int FillerQuestProgression { get; set; }
        [field: SerializeField] public List<NarrativeBlockOfLetters> AllNarrativeBlocks { get; set; }
        [field: SerializeField] public List<FillerBlockOfLetters> AllFillerBlocks { get; set; }
        [field: SerializeField] public FillerBlockOfLetters LastUsedFillerBlock { get; set; }
        [field: SerializeField] public List<QuestProgressionIndexWatcher> QuestProgressionIndexWatchers { get; set; }
        [field: SerializeField] public List<Letter> ThanksAndErrorLetters { get; set; }
        [field: SerializeField] public List<Letter> MailBoxLetters { get; set; }
        [field: SerializeField] public List<ChosenLetter> ChosenLetters { get; set; }
        
        [field: SerializeField] public List<TemperatureChallengeIngredients> CauldronTemperatureAndIngredients { get; set; }
        [field: SerializeField] public Temperature CauldronTemperature { get; set; }
        
        [field: SerializeField] public WeatherSuccessiveDays CurrentWeatherState { get; set; }
        [field: SerializeField] public int CurrentLunarCycleStateIndex { get; set; }
        
        [field: SerializeField] public int MoneyAmount { get; set; }
        
        //----------Orders Content----------
        [field: SerializeField] public List<OrderContentSo> OrdersContentList { get; set; }
        [field: SerializeField] public List<NarrativeBlockOfLetters> OrdersRelatedNBList { get; set; }
        [field: SerializeField] public List<LetterContentSo> OrdersRelatedLetterList { get; set; }
        
        
        //----------Orders Content----------
        
        [field: SerializeField] public bool CodexIsUnlocked { get; set; }
        [field: SerializeField] public bool HasDonePinTutorial { get; set; }
        [field: SerializeField] public List<GardenPlotData> PlotsData { get; set; }
        [field: SerializeField] public CharacterOutfitSo CurrentOutfit { get; set; }
        [field: SerializeField] public List<CharacterOutfitSo> UnlockedOutifts { get; set; }
        [field: SerializeField] public int WorkshopProgressionIndex { get; set; }
        [field: SerializeField] public List<ChargedBiomeAreaSo> UnlockedChargedBiomeAreas { get; set; }
    }
}
