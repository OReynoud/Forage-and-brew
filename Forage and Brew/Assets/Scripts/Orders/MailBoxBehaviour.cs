using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MailBoxBehaviour : Singleton<MailBoxBehaviour>
{
    public List<LetterContentSo> PossibleLettersPool { get; set; } = new();
    [SerializeField] private List<LetterContentSo> day1Letters;
    [SerializeField] private List<LetterContentSo> day2Letters;
    [SerializeField] private List<LetterContentSo> day3Letters;
    private readonly List<LetterContentSo> _chosenLetters = new();
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
        
        if (GameDontDestroyOnLoadManager.Instance.GenerateLetters)
        {
            ChooseLetters();
            GameDontDestroyOnLoadManager.Instance.GenerateLetters = false;
        }
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
        _chosenLetters.Clear();
        
        switch (GameDontDestroyOnLoadManager.Instance.DayPassed)
        {
            case 0:
                _chosenLetters.AddRange(day1Letters);
                break;            
            case 1:
                _chosenLetters.AddRange(day2Letters);
                break;
            case 2:
                _chosenLetters.AddRange(day3Letters);
                break;
        }

        GenerateLetters();
    }
    
    public void GenerateLetters()
    {
        GeneratedLetters.Clear();
        
        for (int i = _chosenLetters.Count - 1; i >= 0; i--)
        {
            var current = Instantiate(letterPrefab, letterPile);
            GeneratedLetters.Insert(0, current);
            current.InitLetter(_chosenLetters[i]);
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
            
            GeneratedLetters[i].AnimateLetter(true);
            
            if (i != GeneratedLetters.Count - 1) return;
        }
        
        Debug.Log("Finished Reading all letters");
        CharacterInputManager.Instance.EnableMoveInputs();
        CharacterInputManager.Instance.EnableInteractInputs();
        CharacterInputManager.Instance.DisableMailInputs();
        letterTrigger.enabled = false;
        targetPos = new Vector2(0f, -1500f);

        foreach (LetterContentSo letter in _chosenLetters)
        {
            if (letter.LetterType != LetterType.Orders) continue;
            
            OrderManager.Instance.CreateNewOrder(letter);
        }
        
    }

    private IEnumerator HandleMultipleExecutions()
    {
        _openedMailOnFrame = true;
        yield return new WaitForEndOfFrame();
        _openedMailOnFrame = false;
    }
}
