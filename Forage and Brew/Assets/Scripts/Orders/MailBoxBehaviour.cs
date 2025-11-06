using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    [SerializeField] private Collider letterBoxTrigger;
    [SerializeField] private Animator letterBoxAnimator;

    private bool _openedMailOnFrame;

    [BoxGroup("LetterAnimation")] public AnimationCurve animCurve;
    [BoxGroup("LetterAnimation")] public float animSpeed;
    [BoxGroup("LetterAnimation")] public GameObject blink;
    [BoxGroup("LetterAnimation")] public AudioSource audioSource;
    
    // Animator Hashes
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");
    private static readonly int IsEmpty = Animator.StringToHash("IsEmpty");
    private static readonly int ReadingLetters = Animator.StringToHash("Letters");
    private static readonly int StartRead = Animator.StringToHash("DoReadLetters");


    private void Start()
    {        
        interactInputCanvasGameObject.SetActive(false);
        CharacterInputManager.Instance.DisableMailInputs();

        moneyDisplayGameObject.SetActive(false);
        GenerateLetters();
        if (GeneratedLetters.Count == 0)
        {
            letterBoxTrigger.enabled = false;
            letterBoxAnimator.SetBool(IsEmpty, true);
            blink.SetActive(false);
        }
        _letterPileTargetPosition = letterPileHiddenPosition;
        letterPile.anchoredPosition = letterPileHiddenPosition;
    }

    public void MailNewDayMethod()
    {
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
            letterBoxAnimator.SetBool(IsEmpty, true);
            blink.SetActive(false);
        }
        else
        {
            letterBoxTrigger.enabled = true;
            letterBoxAnimator.SetBool(IsEmpty, false);
            blink.SetActive(true);
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

    public int ordersCount = 0;
    private bool fillerChosen = false;
    private List<FillerBlockOfLetters> tempValidFillerBlocks = new();
    private List<LetterContentSo> tempValidFillerLetters = new();
    public void ChooseLetters()
    {
        ordersCount = 0;
        GameDontDestroyOnLoadManager.Instance.ChosenLetters.Clear();

        foreach (var letter in GameDontDestroyOnLoadManager.Instance.ThanksAndErrorLetters)
        {
            if (letter.RelatedFillerBlock != null)
            {
                GenerateSuccessLetter(letter);
                continue;
            }
            int index = letter.RelatedNarrativeBlock.ContentSo.Content.IndexOf(letter.LetterContent);
            letter.RelatedNarrativeBlock.InactiveLetters[index] = false;
            
            if (letter.RelatedNarrativeBlock.SelfProgressionIndex == letter.RelatedNarrativeBlock.CompletedLetters.Length)
            {
                GenerateSuccessLetter(letter);
                continue;
            }
            if (letter.RelatedNarrativeBlock.CompletedLetters[index])
            {  
                GenerateSuccessLetter(letter);
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

            //Debug.Log("Generated a letter");
            GameDontDestroyOnLoadManager.Instance.ChosenLetters.Add((new Letter(t.ContentSo.Content[t.SelfProgressionIndex], t), null));
            t.InactiveLetters[t.SelfProgressionIndex] = true;
            if (t.ContentSo.Content[t.SelfProgressionIndex].LetterType == LetterType.Orders)
            {
                ordersCount++;
            }
        }

        fillerChosen = false;
        if (OrderManager.Instance.CurrentOrders.FindIndex(x => x == null) + ordersCount < GameDontDestroyOnLoadManager.Instance.QuestProgressionIndexWatchers[GameDontDestroyOnLoadManager.Instance.FillerQuestProgression].MaximumOrdersAmount)
        {
            foreach (var FillerBlocks in GameDontDestroyOnLoadManager.Instance.AllFillerBlocks)
            {
                if (FillerBlocks == GameDontDestroyOnLoadManager.Instance.LastUsedFillerBlockOfLetters || 
                    FillerBlocks.ContentSo.RequiredQuestProgressionIndex > GameDontDestroyOnLoadManager.Instance.QuestProgressionIndex)
                    continue;
                
                if (!FillerBlocks.HasUsedFirstLetter)
                {
                    GameDontDestroyOnLoadManager.Instance.ChosenLetters.Add((new Letter(FillerBlocks.ContentSo.FirstFiller,FillerBlocks), null));
                    FillerBlocks.HasUsedFirstLetter = true;
                    fillerChosen = true;
                    break;
                }
            }

            if (!fillerChosen)
            {
                
                foreach (var FillerBlocks in GameDontDestroyOnLoadManager.Instance.AllFillerBlocks)
                {
                    if (FillerBlocks == GameDontDestroyOnLoadManager.Instance.LastUsedFillerBlockOfLetters || 
                        FillerBlocks.ContentSo.RequiredQuestProgressionIndex > GameDontDestroyOnLoadManager.Instance.QuestProgressionIndex)
                        continue;
                    tempValidFillerBlocks.Add(FillerBlocks);
                }

                int i = Random.Range(0, tempValidFillerBlocks.Count);
                GameDontDestroyOnLoadManager.Instance.LastUsedFillerBlockOfLetters = tempValidFillerBlocks[i];
                foreach (var FillerLetter in tempValidFillerBlocks[i].ContentSo.Content)
                {
                    if (FillerLetter == tempValidFillerBlocks[i].LastUsedLetter || 
                        FillerLetter.QuestProgressionRequired > GameDontDestroyOnLoadManager.Instance.QuestProgressionIndex)
                        continue;
                    tempValidFillerLetters.Add(FillerLetter);
                }
                int y = Random.Range(0, tempValidFillerLetters.Count);
                tempValidFillerBlocks[i].LastUsedLetter = tempValidFillerLetters[y];
                GameDontDestroyOnLoadManager.Instance.ChosenLetters.Add((new Letter(tempValidFillerLetters[y],tempValidFillerBlocks[i]), null));
                fillerChosen = true;
            }
        }

        GameDontDestroyOnLoadManager.Instance.ThanksAndErrorLetters.Clear();
        tempValidFillerBlocks.Clear();
        tempValidFillerLetters.Clear();
        foreach (var letterTupple in GameDontDestroyOnLoadManager.Instance.ChosenLetters)
        {
            GameDontDestroyOnLoadManager.Instance.MailBoxLetters.Add(letterTupple.Item1);
        }

        //GameDontDestroyOnLoadManager.Instance.AllLetters.AddRange(_chosenLetters);
        GameDontDestroyOnLoadManager.Instance.HasChosenLettersToday = true;
    }
    
    private void GenerateSuccessLetter(Letter letter)
    {
        Debug.Log("Generated success letter");
        int moneyToEarn = letter.LetterContent.OrderContent.MoneyReward;
        _moneyAmountsToEarn.Add((moneyToEarn, GameDontDestroyOnLoadManager.Instance.ChosenLetters.Count));
        if (letter.RelatedNarrativeBlock != null)
        {
            GameDontDestroyOnLoadManager.Instance.ChosenLetters.Add((new Letter(letter.LetterContent.RelatedSuccessLetter, letter.RelatedNarrativeBlock),
                letter.LetterContent));
            letter.RelatedNarrativeBlock.NewLetterCountDown =
                letter.LetterContent.TimeForLetterAfterSuccess;
        }
        else
        {
            GameDontDestroyOnLoadManager.Instance.ChosenLetters.Add((new Letter(letter.LetterContent.RelatedSuccessLetter, letter.RelatedFillerBlock),
                letter.LetterContent));
        }

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
        //Debug.Log("OpenMailbox");
        CharacterAnimManager.instance.animator.SetTrigger(StartRead);
        CharacterAnimManager.instance.animator.SetBool(ReadingLetters, true);
        
        audioSource.Play();
        letterBoxAnimator.SetBool(IsOpen, true);
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
    }

    public void PassToNextLetter()
    {
        if (_openedMailOnFrame) return;

        //Debug.Log("Pass to next letter");
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

        Debug.Log("Read every letter");
        letterBoxAnimator.SetBool(IsOpen, false);
        letterBoxAnimator.SetBool(IsEmpty, true);
        CharacterInputManager.Instance.DisableMailInputs();
        DisableInteract();

        letterBoxTrigger.enabled = false;
        moneyDisplayGameObject.SetActive(false);
        _letterPileTargetPosition = letterPileHiddenPosition;
        _backgroundTargetFadeValue = backgroundHiddenFadeValue;
        int newOrdersCounter = 0;

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
                    newOrdersCounter++;
                    break;
                case LetterType.Thanks:
                    if (!CodexContentManager.instance.historicPages.Find(x => x.OriginLetter))
                        CodexContentManager.instance.AddHistoricPage(letter.Item2, letter.Item1.LetterContent);
                    break;
                case LetterType.Gift:
                    CodexContentManager.instance.AddHistoricPage(letter.Item1.LetterContent, null);
                    MoneyManager.Instance.AddMoney(letter.Item1.LetterContent.MoneyAmount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        CharacterInteractController.Instance.CurrentNearMailBoxBehaviour = null;
        GameDontDestroyOnLoadManager.Instance.MailBoxLetters.Clear();
        
        
        if (GameDontDestroyOnLoadManager.Instance.DayPassed == 0)
        {
            TutorialManager.instance.NotifyFromRecipeReceived("AfterLetter1");
        }
        if (GameDontDestroyOnLoadManager.Instance.DayPassed == 1)
        {
            TutorialManager.instance.NotifyFromRecipeReceived("SpendMoney");
        }
        if (GameDontDestroyOnLoadManager.Instance.DayPassed == 6)
        {
            TutorialManager.instance.NotifyFromRecipeReceived("GardenTuto");
        }
        
        foreach (var recipe in CodexContentManager.instance.recipes)
        {
            if (!recipe.isDissolved)
            {
                CodexContentManager.instance.pageIndexesToCheck.Insert(0, recipe.PageNumber);
                if (Array.Exists(recipe.storedPotion.TemperatureChallengeIngredients, x => x.Temperature != Temperature.None))
                {
                    Debug.Log("Add Bellows Tutorial");
                    TutorialManager.instance.NotifyFromRecipeReceived("Bellows");
                }
            }
        }

        newOrdersCounter = Mathf.FloorToInt(newOrdersCounter * 0.5f);
        for (int i = 0; i < newOrdersCounter; i++)
        {
            CodexContentManager.instance.tutorialDissolvesToCheck.Insert(0, "");
        }
        
        var temp = CodexContentManager.instance.pageIndexesToCheck.Count -
                   CodexContentManager.instance.tutorialDissolvesToCheck.Count;
        Debug.Log(temp);
        if (temp >= 1)
        {
            for (int i = 1; i < temp; i++)
            {
                CodexContentManager.instance.tutorialDissolvesToCheck.Add("");
            }
        }
        AutoFlip.instance.HandleNewRecipes();
        audioSource.Stop();
        audioSource.Play();
        
        CharacterAnimManager.instance.animator.SetBool(ReadingLetters, false);
        
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