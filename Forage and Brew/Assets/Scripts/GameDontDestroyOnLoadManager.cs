using System.Collections.Generic;
using UnityEngine;

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
    public List<List<PotionValuesSo>> OrderPotions { get; private set; } = new();
    
    // Letters
    public bool HasChosenLettersToday { get; set; }
    public List<LetterContentSo> AllLetters { get; set; } = new();
    public List<LetterContentSo> MailBoxLetters { get; set; } = new();
    
    
    
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
}
