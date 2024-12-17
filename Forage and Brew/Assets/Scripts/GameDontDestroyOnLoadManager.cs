using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameDontDestroyOnLoadManager : MonoBehaviour
{
    // Singleton
    public static GameDontDestroyOnLoadManager Instance { get; private set; }
    
    // Debug
    public bool DebugMode { get; set; }
    
    // Scene
    [field: SerializeField] public Scene PreviousScene { get; set; }
    
    // Days
    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Daytime;
    public int DayPassed { get; set; }
    
    // Ingredients and Potions
    public List<IngredientValuesSo> CollectedIngredients { get; private set; } = new();
    public List<List<PotionValuesSo>> OrderPotions { get; private set; } = new();
    
    // Letters
    [field: SerializeField] public bool GenerateLetters { get; set; }
    [field: SerializeField] public List<LetterContentSO> AllLetters { get; set; } = new();
    public List<Letter> GeneratedLetters { get; set; } = new();
    [field: ShowIf("DebugMode")] [field: SerializeField] [field: ReadOnly] public List<Letter> ActiveLetters { get; set; } = new();
    
    
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
