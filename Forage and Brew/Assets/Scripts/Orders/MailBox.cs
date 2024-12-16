using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class MailBox : Singleton<MailBox>
{

    public GameObject papers;

    public WeatherStateSo[] allPossibleWeatherStates; // 0 = Cloudy, 1 = Rainy, 2 = Sunny


    public List<LetterContentSO> PossibleLettersPool;
    
    [SerializeField] private GameObject interactInputCanvasGameObject;

    public LetterContainer letterPrefab;

    public RectTransform letterPile;

    public float letterPileLerp;

    public Vector2 targetPos;
    public Collider letterTrigger;


    [BoxGroup("LetterAnimation")] public AnimationCurve animCurve;
    [BoxGroup("LetterAnimation")] public Vector2 aimedPos;
    [BoxGroup("LetterAnimation")] public float animSpeed;


    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        CharacterInputManager.Instance.DisableMailInputs();
    }

    private void Update()
    {
        letterPile.anchoredPosition = Vector2.Lerp(letterPile.anchoredPosition, targetPos, letterPileLerp);
    }

    private void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    private void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.CurrentNearMailBox = this;
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.CurrentNearMailBox = null;
            DisableInteract();
        }
    }
    public void GenerateLetters()
    {
        switch (GameDontDestroyOnLoadManager.Instance.dayPassed)
        {
            case 0:
                GameDontDestroyOnLoadManager.Instance.GeneratedLetters.Add(new GameDontDestroyOnLoadManager.Letter(PossibleLettersPool[0],PossibleLettersPool[0].TimeToFulfill));
                break;            
            case 1:
                GameDontDestroyOnLoadManager.Instance.GeneratedLetters.Add(new GameDontDestroyOnLoadManager.Letter(PossibleLettersPool[1],PossibleLettersPool[1].TimeToFulfill));
                GameDontDestroyOnLoadManager.Instance.GeneratedLetters.Add(new GameDontDestroyOnLoadManager.Letter(PossibleLettersPool[2],PossibleLettersPool[2].TimeToFulfill));
                break;
            case 2:
                GameDontDestroyOnLoadManager.Instance.GeneratedLetters.Add(new GameDontDestroyOnLoadManager.Letter(PossibleLettersPool[3],PossibleLettersPool[3].TimeToFulfill));
                GameDontDestroyOnLoadManager.Instance.GeneratedLetters.Add(new GameDontDestroyOnLoadManager.Letter(PossibleLettersPool[4],PossibleLettersPool[4].TimeToFulfill));
                break;
        }

        foreach (var letter in GameDontDestroyOnLoadManager.Instance.GeneratedLetters)
        {
            
            var current = Instantiate(letterPrefab,letterPile);
            current.InitLetter(
                letter.LetterContent.ClientName,
                letter.LetterContent.Description,
                letter.LetterContent.RequestedPotions,
                letter.LetterContent.MoneyReward,
                letter.LetterContent.TimeToFulfill,
                letter.LetterContent.LetterType);
            letter.associatedLetter = current;
            current.transform.SetSiblingIndex(1);
        }
    }

    public async void ShowLetters()
    {
        if (GameDontDestroyOnLoadManager.Instance.GeneratedLetters.Count == 0)
            return;
        
        CharacterInputManager.Instance.DisableMoveInputs();
        CharacterInputManager.Instance.DisableInteractInputs();
        await Task.Delay(100);
        CharacterInputManager.Instance.EnableMailInputs();
        targetPos = Vector2.zero;
    }

    public void PassToNextLetter()
    {
        for (int i = 0; i < GameDontDestroyOnLoadManager.Instance.GeneratedLetters.Count; i++)
        {
            if (GameDontDestroyOnLoadManager.Instance.GeneratedLetters[i].associatedLetter.isMoved)
                continue;
            GameDontDestroyOnLoadManager.Instance.GeneratedLetters[i].associatedLetter.AnimateLetter(true);
            if (i != GameDontDestroyOnLoadManager.Instance.GeneratedLetters.Count - 1)
                return;
        }
        Debug.Log("Finished Reading all letters");
        CharacterInputManager.Instance.EnableMoveInputs();
        CharacterInputManager.Instance.EnableInteractInputs();
        CharacterInputManager.Instance.DisableMailInputs();
        letterTrigger.enabled = false;
        targetPos = new Vector2(0,-1500);


        foreach (var letter in GameDontDestroyOnLoadManager.Instance.GeneratedLetters)
        {
            if (letter.LetterContent.LetterType == LetterType.Orders)
            {
                CodexContentManager.instance.ReceiveNewOrder(                
                    letter.LetterContent.ClientName,
                    letter.LetterContent.Description,
                    letter.LetterContent.RequestedPotions,
                    letter.LetterContent.MoneyReward,
                    letter.LetterContent.TimeToFulfill);
                GameDontDestroyOnLoadManager.Instance.ActiveLetters.Add(letter);
                GameDontDestroyOnLoadManager.Instance.OrderPotions.Add(new List<PotionValuesSo>());
                for (int i = 0; i < letter.LetterContent.RequestedPotions.Length; i++)
                {
                    GameDontDestroyOnLoadManager.Instance.OrderPotions[^1].Add(null);
                }
            }
        }
        
    }
}
