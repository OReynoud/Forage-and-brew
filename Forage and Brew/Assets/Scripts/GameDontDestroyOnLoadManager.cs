using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDontDestroyOnLoadManager : MonoBehaviour
{
    public static GameDontDestroyOnLoadManager Instance { get; private set; }
    

    [field: SerializeField] public Scene PreviousScene { get; set; }
    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Daytime;
    
    public int dayPassed { get; set; }

    
    [field: SerializeField] public bool generateLetters { get; set; }
    
    public List<IngredientValuesSo> CollectedIngredients { get; private set; } = new();
    
    [Serializable]
    public class Letter
    {
        public LetterContainer associatedLetter;
        public LetterContentSO LetterContent;
        public int days;

        public Letter(LetterContentSO Letter, int delay )
        {
            LetterContent = Letter;
            days = delay;
        }
    }
    public List<LetterContentSO> AllLetters = new List<LetterContentSO>();

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


    private void Update()
    {
        if (!generateLetters || MailBox.instance == null) return;
        Debug.Log("GeneratedLetters");
        switch (dayPassed)
        {
            case 0:
                break;            
            case 1:
                break;
            case 2:
                break;
        }
        generateLetters = false;
        MailBox.instance.GenerateLetters();
    }
}
