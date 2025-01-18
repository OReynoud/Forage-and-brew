using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailBoxBehaviour : Singleton<MailBoxBehaviour>
{
    

    private readonly List<(int moneyAmount, int letterIndex)> _moneyAmountsToEarn = new();
    [field: SerializeField]public List<LetterMailBoxDisplayBehaviour> GeneratedLetters { get; set; } = new();

    [SerializeField] private GameObject interactInputCanvasGameObject;

    public LetterMailBoxDisplayBehaviour letterPrefab;

    [SerializeField] private GameObject moneyDisplayGameObject;
    [SerializeField] private RectTransform letterPile;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private CanvasGroup addMoneyCanvasGroup;
    [SerializeField] private TMP_Text addMoneyText;
    [SerializeField] private RectTransform addMoneyRectTransform;

    [SerializeField] private float letterPileLerp;
    [SerializeField] private float backgroundFadeLerp;
    [SerializeField] private float addMoneyMoveLerp = 0.05f;
    [SerializeField] private float addMoneyFadeLerp = 0.05f;

    private Vector2 _letterPileTargetPosition;
    [SerializeField] private Vector2 letterPileShownPosition = Vector2.zero;
    [SerializeField] private Vector2 letterPileHiddenPosition = new(0, -1500);
    private float _backgroundTargetFadeValue;
    [SerializeField] private float backgroundShownFadeValue = 0.8f;
    [SerializeField] private float backgroundHiddenFadeValue;

    private Vector2 _addMoneyStartPosition;
    [SerializeField] private Vector2 addMoneyLocalOffsetEndPosition = new(0, 100);
    [SerializeField] private float addMoneyStartFadeValue = 1f;
    [SerializeField] private float addMoneyEndFadeValue;

    public Collider letterBoxTrigger;

    private bool _openedMailOnFrame;

    [BoxGroup("LetterAnimation")] public AnimationCurve animCurve;
    [BoxGroup("LetterAnimation")] public float animSpeed;
    [BoxGroup("LetterAnimation")] public Animator anim;
    [BoxGroup("LetterAnimation")] public GameObject blink;


    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        CharacterInputManager.Instance.DisableMailInputs();

        moneyDisplayGameObject.SetActive(false);
        _letterPileTargetPosition = letterPileHiddenPosition;
        letterPile.anchoredPosition = letterPileHiddenPosition;
        _backgroundTargetFadeValue = backgroundHiddenFadeValue;
        backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b,
            backgroundHiddenFadeValue);
        _addMoneyStartPosition = addMoneyRectTransform.anchoredPosition;
        addMoneyRectTransform.anchoredPosition = _addMoneyStartPosition + addMoneyLocalOffsetEndPosition;
        addMoneyCanvasGroup.alpha = addMoneyEndFadeValue;

        if (!GameDontDestroyOnLoadManager.Instance.HasChosenLettersToday)
        {
            ChooseLetters();
        }

        GenerateLetters();

        if (GeneratedLetters.Count == 0)
        {
            letterBoxTrigger.enabled = false;
            anim.SetBool("IsOpen", true);
            blink.SetActive(false);
        }
    }

    private void Update()
    {
        letterPile.anchoredPosition =
            Vector2.Lerp(letterPile.anchoredPosition, _letterPileTargetPosition, letterPileLerp);
        backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b,
            Mathf.Lerp(backgroundImage.color.a, _backgroundTargetFadeValue, backgroundFadeLerp));
        addMoneyRectTransform.anchoredPosition = Vector2.Lerp(addMoneyRectTransform.anchoredPosition,
            _addMoneyStartPosition + addMoneyLocalOffsetEndPosition, addMoneyMoveLerp);
        addMoneyCanvasGroup.alpha = Mathf.Lerp(addMoneyCanvasGroup.alpha, addMoneyEndFadeValue, addMoneyFadeLerp);
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
        GameDontDestroyOnLoadManager.Instance.ChosenLetters.Clear();


        foreach (var letter in GameDontDestroyOnLoadManager.Instance.ThanksAndErrorLetters)
        {
            int index = Array.IndexOf(
                letter.RelatedNarrativeBlock.ContentSo.Content,
                letter.LetterContent);
            letter.RelatedNarrativeBlock.InactiveLetters[index] = false;
            float percentPenalty = letter.LetterContent.OrderContent.LateMoneyPenaltyPercentage;

            if (letter.RelatedNarrativeBlock.SelfProgressionIndex == 0)
            {
                GenerateFailureLetter(letter, percentPenalty);
                continue;
            }
            if (letter.RelatedNarrativeBlock.SelfProgressionIndex == letter.RelatedNarrativeBlock.CompletedLetters.Length)
            {
                GenerateSuccessLetter(letter, percentPenalty);
                continue;
            }
            if (letter.RelatedNarrativeBlock.CompletedLetters[letter.RelatedNarrativeBlock.SelfProgressionIndex])
            {
                GenerateSuccessLetter(letter, percentPenalty);
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
            if (t.NewLetterCountDown > 0)
            {
                t.NewLetterCountDown--;
                continue;
            }

            Debug.Log("Generated a letter");
            GameDontDestroyOnLoadManager.Instance.ChosenLetters.Add((new Letter(t.ContentSo.Content[t.SelfProgressionIndex], t), null));
            t.InactiveLetters[t.SelfProgressionIndex] = true;
        }

        GameDontDestroyOnLoadManager.Instance.ThanksAndErrorLetters.Clear();
        foreach (var letterTupple in GameDontDestroyOnLoadManager.Instance.ChosenLetters)
        {
            GameDontDestroyOnLoadManager.Instance.MailBoxLetters.Add(letterTupple.Item1);
        }

        //GameDontDestroyOnLoadManager.Instance.AllLetters.AddRange(_chosenLetters);
        GameDontDestroyOnLoadManager.Instance.HasChosenLettersToday = true;
    }

    private void GenerateFailureLetter(Letter letter, float percentPenalty)
    {
        int moneyToEarn;
        moneyToEarn = letter.DeliveredOnTime
            ? letter.LetterContent.OrderContent.ErrorMoneyReward
            : Mathf.RoundToInt(letter.LetterContent.OrderContent.ErrorMoneyReward * percentPenalty * 0.01f);
        _moneyAmountsToEarn.Add((moneyToEarn, GameDontDestroyOnLoadManager.Instance.ChosenLetters.Count));
        GameDontDestroyOnLoadManager.Instance.ChosenLetters.Add((new Letter(letter.LetterContent.RelatedFailureLetter, letter.RelatedNarrativeBlock),
            letter.LetterContent));
        letter.RelatedNarrativeBlock.NewLetterCountDown =
            letter.RelatedNarrativeBlock.ContentSo.TimeForLetterAfterFailure;
    }

    private void GenerateSuccessLetter(Letter letter, float percentPenalty)
    {
        int moneyToEarn;
        moneyToEarn = letter.DeliveredOnTime
            ? letter.LetterContent.OrderContent.MoneyReward
            : Mathf.RoundToInt(letter.LetterContent.OrderContent.MoneyReward * percentPenalty * 0.01f);
        _moneyAmountsToEarn.Add((moneyToEarn, GameDontDestroyOnLoadManager.Instance.ChosenLetters.Count));
        GameDontDestroyOnLoadManager.Instance.ChosenLetters.Add((new Letter(letter.LetterContent.RelatedSuccessLetter, letter.RelatedNarrativeBlock),
            letter.LetterContent));
        letter.RelatedNarrativeBlock.NewLetterCountDown =
            letter.RelatedNarrativeBlock.ContentSo.TimeForLetterAfterSuccess;
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

        
        anim.SetBool("IsOpen", true);
        blink.SetActive(false);
        StartCoroutine(HandleMultipleExecutions()); // Wait to be able to pass to next letter
        CharacterInputManager.Instance.DisableMoveInputs();
        CharacterInputManager.Instance.DisableInteractInputs();
        CharacterInputManager.Instance.EnableMailInputs();
        CharacterInputManager.Instance.DisableCodexInputs();
        moneyDisplayGameObject.SetActive(true);
        _letterPileTargetPosition = letterPileShownPosition;
        _backgroundTargetFadeValue = backgroundShownFadeValue;
        moneyText.text = MoneyManager.Instance.MoneyAmount.ToString();
        CharacterInteractController.Instance.CurrentNearMailBoxBehaviour = null;
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
                int moneyAmount = _moneyAmountsToEarn.First(x => x.letterIndex == i).moneyAmount;
                MoneyManager.Instance.AddMoney(moneyAmount);
                moneyText.text = MoneyManager.Instance.MoneyAmount.ToString();
                addMoneyText.text = moneyAmount.ToString();
                addMoneyRectTransform.anchoredPosition = _addMoneyStartPosition;
                addMoneyCanvasGroup.alpha = addMoneyStartFadeValue;
            }

            GeneratedLetters[i].AnimateLetter(true);

            if (i != GeneratedLetters.Count - 1) return;
        }

        CharacterInputManager.Instance.EnableMoveInputs();
        CharacterInputManager.Instance.EnableInteractInputs();
        CharacterInputManager.Instance.DisableMailInputs();
        CharacterInputManager.Instance.EnableCodexInputs();
        DisableInteract();

        letterBoxTrigger.enabled = false;
        moneyDisplayGameObject.SetActive(false);
        _letterPileTargetPosition = letterPileHiddenPosition;
        _backgroundTargetFadeValue = backgroundHiddenFadeValue;

        foreach (var letter in GameDontDestroyOnLoadManager.Instance.ChosenLetters)
        {
            switch (letter.Item1.LetterContent.LetterType)
            {
                case LetterType.Orders:
                    foreach (var demand in letter.Item1.LetterContent.OrderContent.RequestedPotions)
                    {
                        if (!demand.IsSpecific) continue;
                        if (IsNewRecipe(demand))
                        {
                            GameDontDestroyOnLoadManager.Instance.UnlockedRecipes.Add(demand.Potion);
                            GameDontDestroyOnLoadManager.Instance.OnNewRecipeReceived.Invoke(demand.Potion);
                        }
                    }
                    OrderManager.Instance.CreateNewOrder(letter.Item1);
                    break;
                case LetterType.Thanks:
                    CodexContentManager.instance.AddHistoricPage(letter.Item2, letter.Item1.LetterContent);
                    break;
                case LetterType.ShippingError:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        GameDontDestroyOnLoadManager.Instance.MailBoxLetters.Clear();
        AutoFlip.instance.HandleNewRecipes();
        
        GameDontDestroyOnLoadManager.Instance.ChosenLetters.Clear();
    }

    private bool IsNewRecipe(PotionDemand demand)
    {
        return !GameDontDestroyOnLoadManager.Instance.UnlockedRecipes.Contains(demand.Potion);
    }

    private IEnumerator HandleMultipleExecutions()
    {
        _openedMailOnFrame = true;
        yield return new WaitForEndOfFrame();
        _openedMailOnFrame = false;
    }
}