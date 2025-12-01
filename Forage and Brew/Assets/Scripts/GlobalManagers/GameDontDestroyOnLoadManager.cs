using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class GameDontDestroyOnLoadManager : MonoBehaviour
{
    // Singleton
    public static GameDontDestroyOnLoadManager Instance { get; private set; }
    
    // Global Information
    public bool IsFirstGameSession { get; set; } = true;
    
    // Scene
    [field: SerializeField] [field: ReadOnly] public Scene CurrentScene { get; set; }
    
    // Days
    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Daytime;
    [field: SerializeField]public int DayPassed { get; set; }
    
    // Collected Ingredients and Potions
    public List<IngredientValuesSo> CollectedIngredients { get; private set; } = new();
    public List<CollectedIngredientBehaviour> OutCollectedIngredients { get; private set; } = new();
    public List<CollectedSeedBehaviour> OutSeeds { get; private set; } = new();
    public List<FloorIngredient> FloorCollectedIngredients { get; private set; } = new();
    public List<CollectedPotionBehaviour> OutCookedPotions { get; private set; } = new();
    public List<FloorCookedPotion> FloorCookedPotions { get; private set; } = new();
    public List<ClientOrderPotions> OrderPotions { get; private set; } = new();
    
    // Potion Ensembles
    public Dictionary<PotionEnsembleSo, int> UnlockedPotionEnsembles { get; private set; } = new();
    
    // Unlocked Ingredients and Recipes
    [SerializeField] public List<IngredientValuesSo> UnlockedIngredients = new();
    [SerializeField] public List<PotionValuesSo> UnlockedRecipes = new();
    [SerializeField] public List<TutorialDissolveBehavior> UnlockedTutorials = new();
    public UnityEvent<IngredientValuesSo> OnNewIngredientCollected { get; private set; } = new();
    public UnityEvent<PotionValuesSo> OnNewRecipeReceived { get; private set; } = new();
    
    // Ingredients to Collect
    public bool HasChosenIngredientsToday { get; set; }
    public Dictionary<int, IngredientValuesSo> RemainingIngredientToCollectBehaviours { get; private set; } = new();
    
    // Letters
    [field: SerializeField] public bool HasChosenLettersToday { get; set; }
    [field: SerializeField] public int QuestProgressionIndex { get; set; }
    [field: SerializeField] public int FillerQuestProgression { get; set; }
    [field: Expandable] [field: SerializeField] public List<NarrativeBlockOfLettersContentSo> AllNarrativeBlocksContentSo { get; set; } = new();
    [field: SerializeField] [field: AllowNesting] public List<NarrativeBlockOfLetters> AllNarrativeBlocks { get; set; } = new();
    
    [field: Expandable] [field: SerializeField] public List<FillerBlockLettersContentSo> AllFillerBlocksContentSo { get; set; } = new();
    [field: SerializeField] [field: AllowNesting] public List<FillerBlockOfLetters> AllFillerBlocks { get; set; } = new();
    public FillerBlockOfLetters LastUsedFillerBlockOfLetters { get; set; }
    [field: SerializeField] [field: AllowNesting] public List<QuestProgressionIndexWatcher> QuestProgressionIndexWatchers { get; set; } = new();


    public List<Letter> ThanksAndErrorLetters { get; set; } = new();
    [field: SerializeField]public List<Letter> MailBoxLetters { get; set; } = new();
    public List<(Letter, LetterContentSo)> ChosenLetters { get; set; } = new();
    
    // Cauldron
    public List<TemperatureChallengeIngredients> CauldronTemperatureAndIngredients { get; private set; } = new();
    [field: SerializeField] public Temperature CauldronTemperature { get; set; } = Temperature.LowHeat;
    
    // Haptic Challenges
    public bool IsInHapticChallenge { get; set; }
    
    // Progression
    [field: SerializeField] public int WorkshopProgressionIndex { get; set; }
    public List<ChargedBiomeAreaSo> UnlockedChargedBiomeAreas { get; set; } = new();
    
    // Outfits
    [field: SerializeField] public CharacterOutfitSo CurrentOutfitSo { get; set; }
    [field: SerializeField] public List<CharacterOutfitSo> UnlockedOutfits { get; set; } = new();
    
    //Debug Options
    [Foldout("Debug")] public bool loadOrders;
    [Foldout("Debug")][ShowIf("loadOrders")] public List<LetterContentSo> OrdersToLoad;
    [Foldout("Debug")] public bool loadAllRecipes;
    [Foldout("Debug")] public bool loadAllIngredients;
    [Foldout("Debug")] public bool loadHistoric;
    [Foldout("Debug")][ShowIf("loadHistoric")] public List<LetterContentSo> HistoricToLoad;
    [Foldout("Debug")] public bool debugCommands;
    [Foldout("Debug")] public bool codexIsUnlocked;
    [Foldout("Debug")] public bool hasDonePinTutorial;
    [Foldout("Debug")] public bool unlockedOnWakeUp;
    [Foldout("Debug")] public int moneyAmountOnStart;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    
    private void Start()
    {
        //SimpleCameraBehavior.instance.InstantCamUpdate();
        InfoDisplayManager.instance.DisplayDays();
        if (MailBoxBehaviour.instance != null)
        {
            MailBoxBehaviour.instance.MailNewDayMethod();
        }
    }
}
