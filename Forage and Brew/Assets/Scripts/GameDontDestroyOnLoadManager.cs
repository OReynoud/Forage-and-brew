using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class GameDontDestroyOnLoadManager : MonoBehaviour
{
    // Singleton
    public static GameDontDestroyOnLoadManager Instance { get; private set; }
    
    // Debug
    [SerializeField] private bool debugMode;
    
    // Scene
    [field: SerializeField] public Scene PreviousScene { get; set; }
    
    // Days
    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Daytime;
    public int DayPassed { get; set; }
    
    // Ingredients and Potions
    public List<IngredientValuesSo> CollectedIngredients { get; private set; } = new();
    public List<CollectedIngredientBehaviour> OutCollectedIngredients { get; private set; } = new();
    public List<(IngredientValuesSo ingredient, Vector3 position, Quaternion rotation)> FloorCollectedIngredients { get; private set; } = new();
    public List<CollectedPotionBehaviour> OutCookedPotions { get; private set; } = new();
    public List<(PotionValuesSo potion, Vector3 position, Quaternion rotation)> FloorCookedPotions { get; private set; } = new();
    public List<ClientOrderPotions> OrderPotions { get; private set; } = new();
    
    // Unlocked Ingredients and Recipes
    public List<IngredientValuesSo> UnlockedIngredients { get; private set; } = new();
    public List<PotionValuesSo> UnlockedRecipes { get; private set; } = new();
    public UnityEvent<IngredientValuesSo> OnNewIngredientCollected { get; private set; } = new();
    
    // Letters
    public bool HasChosenLettersToday { get; set; }
    [field: SerializeField] public int QuestProgressionIndex { get; set; }
    
    [field: Expandable][field: SerializeField] public List<NarrativeBlockOfLettersContentSo> AllNarrativeBlocksContentSo { get; set; } = new();
    public List<NarrativeBlockOfLetters> AllNarrativeBlocks { get; set; } = new();
    
    public List<Letter> ThanksAndErrorLetters { get; set; } = new();
    public List<Letter> MailBoxLetters { get; set; } = new();
    
    // Cauldron
    public List<TemperatureChallengeIngredients> CauldronTemperatureAndIngredients { get; private set; } = new();
    [field: SerializeField] public Temperature CauldronTemperature { get; set; } = Temperature.LowHeat;
    
    
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
        foreach (var ContentSo in AllNarrativeBlocksContentSo)
        {
            AllNarrativeBlocks.Add(new NarrativeBlockOfLetters(ContentSo));
        }
    }
    
    private void Start()
    {

        InfoDisplayManager.instance.DisplayDays();
    }
}
