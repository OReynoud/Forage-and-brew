using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoppingHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static ChoppingHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private ChoppingHapticChallengeListSo choppingHapticChallengeListSo;
    
    [Header("UI")]
    [SerializeField] private GameObject choppingChallengeGameObject;
    [SerializeField] private RectTransform gaugeRectTransform;
    [SerializeField] private Image gaugeImage;
    [SerializeField] private RectTransform currentInputRectTransform;
    [SerializeField] private Image currentInputImage;
    [SerializeField] private RectTransform nextInputRectTransform;
    [SerializeField] private Image nextInputImage;
    [SerializeField] private List<Sprite> choppingInputSprites;
    
    [Header("Camera")]
    [SerializeField] private CameraPreset choppingChallengeCameraPreset;
    [SerializeField] private float choppingCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
    
    [Header("Character")]
    [SerializeField] private Vector3 characterChoppingPosition;
    [SerializeField] private Vector3 characterChoppingRotation;
    
    public ChoppingCountertopBehaviour CurrentChoppingCountertopBehaviour { get; set; }
    private bool _isChallengeActive;
    private ChoppingHapticChallengeSo _currentChoppingChallenge;
    private int _currentChoppingInputIndex;
    private bool _isWaitingForNextChopping;
    private float _currentChoppingWaitTime;
    
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        choppingChallengeGameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isChallengeActive) return;
        
        UpdateChoppingChallenge();
    }
    
    
    public void StartChoppingChallenge()
    {
        if (!CurrentChoppingCountertopBehaviour) return;
        
        // Challenge variables
        _isChallengeActive = true;
        _currentChoppingChallenge = choppingHapticChallengeListSo.ChoppingHapticChallenges[Random.Range(0,
            choppingHapticChallengeListSo.ChoppingHapticChallenges.Count)];
        _currentChoppingInputIndex = 0;
        _isWaitingForNextChopping = false;
        _currentChoppingWaitTime = 0f;
        
        // UI
        choppingChallengeGameObject.SetActive(true);
        gaugeImage.fillAmount = 0f;
        
        // Countertop
        CurrentChoppingCountertopBehaviour.DisableInteract();
        
        // Inputs
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnableChoppingHapticChallengeInputs();
        
        // Camera
        _previousCameraPreset = CameraController.instance.TargetCamSettings;
        CameraController.instance.ApplyScriptableCamSettings(choppingChallengeCameraPreset, choppingCameraTransitionTime);

        // Character
        transform.position = CurrentChoppingCountertopBehaviour.transform.position + characterChoppingPosition;
        transform.rotation = CurrentChoppingCountertopBehaviour.transform.rotation * Quaternion.Euler(characterChoppingRotation);
        
        StartChoppingTurn();
    }
    
    private void StopChoppingChallenge()
    {
        _isChallengeActive = false;
        choppingChallengeGameObject.SetActive(false);
        
        CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, choppingCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        CurrentChoppingCountertopBehaviour.EnableInteract();
    }
    
    private void UpdateChoppingChallenge()
    {
        if (!_isWaitingForNextChopping) return;
        
        _currentChoppingWaitTime += Time.deltaTime;
        
        if (_currentChoppingWaitTime >= _currentChoppingChallenge.TimeBeforeNextChopping)
        {
            _currentChoppingWaitTime = 0f;
            _isWaitingForNextChopping = false;
            StartChoppingTurn();
        }
    }
    
    private void StartChoppingTurn()
    {
        currentInputImage.enabled = true;
        currentInputImage.sprite = choppingInputSprites[_currentChoppingChallenge.ChoppingInputIndices[_currentChoppingInputIndex] - 1];
        currentInputRectTransform.anchoredPosition = new Vector2(_currentChoppingInputIndex *
            gaugeRectTransform.sizeDelta.x / _currentChoppingChallenge.ChoppingInputIndices.Count +
            gaugeRectTransform.sizeDelta.x / _currentChoppingChallenge.ChoppingInputIndices.Count * 0.5f,
            currentInputRectTransform.anchoredPosition.y);

        if (_currentChoppingInputIndex + 1 < _currentChoppingChallenge.ChoppingInputIndices.Count)
        {
            nextInputImage.sprite = choppingInputSprites[_currentChoppingChallenge.ChoppingInputIndices[_currentChoppingInputIndex + 1] - 1];
            nextInputRectTransform.anchoredPosition = new Vector2((_currentChoppingInputIndex + 1) *
                gaugeRectTransform.sizeDelta.x / _currentChoppingChallenge.ChoppingInputIndices.Count +
                gaugeRectTransform.sizeDelta.x / _currentChoppingChallenge.ChoppingInputIndices.Count * 0.5f,
                nextInputRectTransform.anchoredPosition.y);
        }
    }
    
    public void NextChoppingTurn(int inputIndex)
    {
        if (!_isChallengeActive) return;
        
        if (_isWaitingForNextChopping) return;
        
        if (inputIndex != _currentChoppingChallenge.ChoppingInputIndices[_currentChoppingInputIndex]) return;
        
        _currentChoppingInputIndex++;
        
        gaugeImage.fillAmount = (float) _currentChoppingInputIndex / _currentChoppingChallenge.ChoppingInputIndices.Count;
        currentInputImage.enabled = false;
        
        if (_currentChoppingInputIndex == _currentChoppingChallenge.ChoppingInputIndices.Count)
        {
            StopChoppingChallenge();
        }
        
        _isWaitingForNextChopping = true;
    }
}
