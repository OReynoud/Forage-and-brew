using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class MailBoxBehaviour : Singleton<MailBoxBehaviour>
{

    [SerializeField] [AllowNesting][ReadOnly]private List<Letter> chosenLetters = new();
    private readonly List<(int moneyAmount, int letterIndex)> _moneyAmountsToEarn = new();
    public List<LetterMailBoxDisplayBehaviour> GeneratedLetters { get; set; } = new();

    [SerializeField] private GameObject interactInputCanvasGameObject;

    public LetterMailBoxDisplayBehaviour letterPrefab;

    public RectTransform letterPile;

    public float letterPileLerp;

    public Vector2 targetPos;
    public Collider letterTrigger;

    private bool _openedMailOnFrame;

    [BoxGroup("LetterAnimation")] public AnimationCurve animCurve;
    [BoxGroup("LetterAnimation")] public float animSpeed;


    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        CharacterInputManager.Instance.DisableMailInputs();


        if (!GameDontDestroyOnLoadManager.Instance.HasChosenLettersToday)
        {
            ChooseLetters();
        }

        GenerateLetters();
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
            characterInteractController.CurrentNearMailBoxBehaviour = this;
            EnableInteract();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.CurrentNearMailBoxBehaviour = null;
            DisableInteract();
        }
    }

    public void ChooseLetters()
    {
        Debug.Log("Choosen Letters to generate");
        chosenLetters.Clear();

        // switch (GameDontDestroyOnLoadManager.Instance.DayPassed)
        // {
        //     case 0:
        //         _chosenLetters.AddRange(day1Letters);
        //         break;
        //     case 1:
        //         _chosenLetters.AddRange(day2Letters);
        //         break;
        //     case 2:
        //         _chosenLetters.AddRange(day3Letters);
        //         break;
        // }
        foreach (var letter in GameDontDestroyOnLoadManager.Instance.ThanksAndErrorLetters)
        {
            int index = Array.IndexOf(
                letter.RelatedNarrativeBlock.ContentSo.Content,
                letter.LetterContent);
            letter.RelatedNarrativeBlock.InactiveLetters[index] = false;
            if (letter.RelatedNarrativeBlock.CompletedLetters[letter.RelatedNarrativeBlock.SelfProgressionIndex - 1])
            {
                _moneyAmountsToEarn.Add((letter.LetterContent.OrderContent.MoneyReward, chosenLetters.Count));
                chosenLetters.Add(new Letter(letter.LetterContent.RelatedSuccessLetter, letter.RelatedNarrativeBlock));
            }
            else
            {
                _moneyAmountsToEarn.Add((letter.LetterContent.OrderContent.ErrorMoneyReward, chosenLetters.Count));
                chosenLetters.Add(new Letter(letter.LetterContent.RelatedFailureLetter, letter.RelatedNarrativeBlock));
            }
        }
        
        foreach (var t in GameDontDestroyOnLoadManager.Instance.AllNarrativeBlocks)
        {
            if (t.ContentSo.RequiredQuestProgressionIndex >
                GameDontDestroyOnLoadManager.Instance.QuestProgressionIndex)
                continue;
            
            if (t.SelfProgressionIndex >= t.CompletedLetters.Length)
                continue;
            
            if (t.CompletedLetters[t.SelfProgressionIndex] || t.InactiveLetters[t.SelfProgressionIndex])
                continue;

            chosenLetters.Add(new Letter(t.ContentSo.Content[t.SelfProgressionIndex], t));
            t.InactiveLetters[t.SelfProgressionIndex] = true;
        }
        GameDontDestroyOnLoadManager.Instance.ThanksAndErrorLetters.Clear();
        GameDontDestroyOnLoadManager.Instance.MailBoxLetters.AddRange(chosenLetters);
        //GameDontDestroyOnLoadManager.Instance.AllLetters.AddRange(_chosenLetters);
        GameDontDestroyOnLoadManager.Instance.HasChosenLettersToday = true;
    }

    public void GenerateLetters()
    {
        GeneratedLetters.Clear();

        for (int i = GameDontDestroyOnLoadManager.Instance.MailBoxLetters.Count - 1; i >= 0; i--)
        {
            var current = Instantiate(letterPrefab, letterPile);
            GeneratedLetters.Insert(0, current);
            current.InitLetter(GameDontDestroyOnLoadManager.Instance.MailBoxLetters[i].LetterContent);
        }
    }

    public void ShowLetters()
    {
        if (GeneratedLetters.Count == 0) return;

        StartCoroutine(HandleMultipleExecutions()); // Wait to be able to pass to next letter
        CharacterInputManager.Instance.DisableMoveInputs();
        CharacterInputManager.Instance.DisableInteractInputs();
        CharacterInputManager.Instance.EnableMailInputs();
        targetPos = Vector2.zero;
    }

    public void PassToNextLetter()
    {
        if (_openedMailOnFrame) return;

        for (int i = 0; i < GeneratedLetters.Count; i++)
        {
            if (GeneratedLetters[i].IsMoving) return;

            if (GeneratedLetters[i].IsPassed) continue;

            if (_moneyAmountsToEarn.Select(x => x.letterIndex).Contains(i))
            {
                MoneyManager.Instance.AddMoney(_moneyAmountsToEarn.First(x => x.letterIndex == i).moneyAmount);
            }
            GeneratedLetters[i].AnimateLetter(true);

            if (i != GeneratedLetters.Count - 1) return;
        }

        Debug.Log("Finished Reading all letters");
        CharacterInputManager.Instance.EnableMoveInputs();
        CharacterInputManager.Instance.EnableInteractInputs();
        CharacterInputManager.Instance.DisableMailInputs();
        CharacterInteractController.Instance.CurrentNearMailBoxBehaviour = null;
        DisableInteract();
        
        letterTrigger.enabled = false;
        targetPos = new Vector2(0f, -1500f);

        foreach (Letter letter in GameDontDestroyOnLoadManager.Instance.MailBoxLetters)
        {
            if (letter.LetterContent.LetterType != LetterType.Orders) continue;

            OrderManager.Instance.CreateNewOrder(letter);
        }

        GameDontDestroyOnLoadManager.Instance.MailBoxLetters.Clear();
    }

    private IEnumerator HandleMultipleExecutions()
    {
        _openedMailOnFrame = true;
        yield return new WaitForEndOfFrame();
        _openedMailOnFrame = false;
    }
}