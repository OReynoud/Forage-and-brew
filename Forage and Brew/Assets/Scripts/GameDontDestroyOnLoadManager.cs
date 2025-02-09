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
    [field: SerializeField] public Scene CurrentScene { get; set; }
    
    // Days
    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Daytime;
    public int DayPassed { get; set; }
    
    // Collected Ingredients and Potions
    public List<IngredientValuesSo> CollectedIngredients { get; private set; } = new();
    public List<CollectedIngredientBehaviour> OutCollectedIngredients { get; private set; } = new();
    public List<FloorIngredient> FloorCollectedIngredients { get; private set; } = new();
    public List<CollectedPotionBehaviour> OutCookedPotions { get; private set; } = new();
    public List<FloorCookedPotion> FloorCookedPotions { get; private set; } = new();
    public List<ClientOrderPotions> OrderPotions { get; private set; } = new();
    
    // Unlocked Ingredients and Recipes
    public List<IngredientValuesSo> UnlockedIngredients { get; private set; } = new();
    [SerializeField] public List<PotionValuesSo> UnlockedRecipes = new();
    public UnityEvent<IngredientValuesSo> OnNewIngredientCollected { get; private set; } = new();
    public UnityEvent<PotionValuesSo> OnNewRecipeReceived { get; private set; } = new();
    
    // Ingredients to Collect
    public bool HasChosenIngredientsToday { get; set; }
    public Dictionary<int, IngredientValuesSo> RemainingIngredientToCollectBehaviours { get; private set; } = new();
    
    // Letters
    [field: SerializeField] public bool HasChosenLettersToday { get; set; }
    [field: SerializeField] public int QuestProgressionIndex { get; set; }
    
    [field: Expandable] [field: SerializeField] public List<NarrativeBlockOfLettersContentSo> AllNarrativeBlocksContentSo { get; set; } = new();
    [field: SerializeField] [field: AllowNesting] public List<NarrativeBlockOfLetters> AllNarrativeBlocks { get; set; } = new();
    
    public List<Letter> ThanksAndErrorLetters { get; set; } = new();
    [field: SerializeField]public List<Letter> MailBoxLetters { get; set; } = new();
    public List<(Letter, LetterContentSo)> ChosenLetters { get; set; } = new();
    
    // Cauldron
    public List<TemperatureChallengeIngredients> CauldronTemperatureAndIngredients { get; private set; } = new();
    [field: SerializeField] public Temperature CauldronTemperature { get; set; } = Temperature.LowHeat;
    
    // Haptic Challenges
    public bool IsInHapticChallenge { get; set; }
    
    
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
        SimpleCameraBehavior.instance.InstantCamUpdate();
        InfoDisplayManager.instance.DisplayDays();
    }
}
