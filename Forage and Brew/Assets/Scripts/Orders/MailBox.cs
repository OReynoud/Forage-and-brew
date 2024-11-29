using System;
using System.Collections.Generic;
using UnityEngine;

public class MailBox : Singleton<MailBox>
{

    public GameObject papers;

    public WeatherStateSo[] allPossibleWeatherStates; // 0 = Cloudy, 1 = Rainy, 2 = Sunny

    public LetterContentSO[] allLettersList;

    public List<LetterContentSO> PossibleLettersPool;
    
    [SerializeField] private GameObject interactInputCanvasGameObject;

    public LetterContainer letterPrefab;

    public RectTransform letterPile;

    public float letterPileLerp;

    public Vector2 targetPos;


    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
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
        GameDontDestroyOnLoadManager.Instance.GeneratedLetters.Add(new GameDontDestroyOnLoadManager.Letter(PossibleLettersPool[0],PossibleLettersPool[0].TimeToFulfill));
        foreach (var letter in GameDontDestroyOnLoadManager.Instance.GeneratedLetters)
        {
            var current = Instantiate(letterPrefab,letterPile);
            current.InitLetter(
                letter.LetterContent.ClientName,
                letter.LetterContent.Description,
                letter.LetterContent.RequestedPotions,
                letter.LetterContent.MoneyReward,
                letter.LetterContent.TimeToFulfill);
        }
    }

    public void ShowLetters()
    {
        CharacterInputManager.Instance.DisableMoveInputs();
        targetPos = Vector2.zero;
    }
}
