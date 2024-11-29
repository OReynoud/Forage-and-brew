using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDontDestroyOnLoadManager : MonoBehaviour
{
    public static GameDontDestroyOnLoadManager Instance { get; private set; }
    
    public Scene PreviousScene { get; set; }
    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Daytime;

    [field: SerializeField] public bool generateLetters { get; set; }
    
    public List<IngredientValuesSo> CollectedIngredients { get; private set; } = new();
    
    [Serializable]
    public class Letter
    {
        public LetterContentSO LetterContent;
        public int days;

        public Letter(LetterContentSO Letter, int delay)
        {
            LetterContent = Letter;
            days = delay;
        }
    }

    public List<Letter> GeneratedLetters = new List<Letter>();
    public List<Letter> ActiveLetters = new List<Letter>();
    
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
        if (generateLetters && MailBox.instance != null)
        {
            generateLetters = false;
            MailBox.instance.GenerateLetters();
        }
    }
}
