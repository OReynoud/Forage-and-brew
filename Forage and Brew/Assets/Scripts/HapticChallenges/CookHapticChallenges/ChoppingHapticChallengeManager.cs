using System.Collections.Generic;
using UnityEngine;

public class ChoppingHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static ChoppingHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private ChoppingHapticChallengeListSo choppingHapticChallengeListSo;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private GameObject knifeGameObject;
    
    [Header("UI")]
    [SerializeField] private GameObject choppingChallengeGameObject;
    [SerializeField] private Transform choppingInputParentTransform;
    [SerializeField] private ChoppingInputBehaviour choppingInputPrefab;
    private readonly List<ChoppingInputBehaviour> _choppingInputBehaviours = new();
    
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
    
    // Animator Hashes
    private static readonly int IsChopping = Animator.StringToHash("IsChopping");
    private static readonly int DoChop = Animator.StringToHash("DoChop");


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
        for (int i = 0; i < _currentChoppingChallenge.ChoppingInputIndices.Count; i++)
        {
            ChoppingInputBehaviour choppingInputBehaviour = Instantiate(choppingInputPrefab, choppingInputParentTransform);
            choppingInputBehaviour.SetInputSprite(_currentChoppingChallenge.ChoppingInputIndices[i]);
            _choppingInputBehaviours.Add(choppingInputBehaviour);
        }
        
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
        
        // Animation
        characterAnimator.SetBool(IsChopping, true);
        knifeGameObject.SetActive(true);
        
        StartChoppingTurn();
    }
    
    private void StopChoppingChallenge()
    {
        _isChallengeActive = false;

        foreach (ChoppingInputBehaviour choppingInputBehaviour in _choppingInputBehaviours)
        {
            Destroy(choppingInputBehaviour.gameObject);
        }
        _choppingInputBehaviours.Clear();
        choppingChallengeGameObject.SetActive(false);
        
        CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, choppingCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        // CurrentChoppingCountertopBehaviour.EnableInteract();
        characterAnimator.SetBool(IsChopping, false);
        knifeGameObject.SetActive(false);
        CurrentChoppingCountertopBehaviour.ChopIngredient(choppingHapticChallengeListSo);
    }
    
    private void UpdateChoppingChallenge()
    {
        if (!_isWaitingForNextChopping) return;
        
        _currentChoppingWaitTime += Time.deltaTime;
        
        if (_currentChoppingWaitTime >= choppingHapticChallengeListSo.TimeBeforeNextChopping)
        {
            _currentChoppingWaitTime = 0f;
            _isWaitingForNextChopping = false;
            StartChoppingTurn();
        }
    }
    
    private void StartChoppingTurn()
    {
        _choppingInputBehaviours[_currentChoppingInputIndex].SetCurrentInput();
    }
    
    public void NextChoppingTurn(int inputIndex)
    {
        if (!_isChallengeActive) return;
        
        if (_isWaitingForNextChopping) return;
        
        if (inputIndex != _currentChoppingChallenge.ChoppingInputIndices[_currentChoppingInputIndex])
        {
            _choppingInputBehaviours[_currentChoppingInputIndex].SetWrongInput();
        }
        else
        {
            _choppingInputBehaviours[_currentChoppingInputIndex].SetRightInput();
        }
        
        // Animation
        characterAnimator.SetTrigger(DoChop);
        
        // VFX
        CurrentChoppingCountertopBehaviour.CountertopVfxManager.PlayChopVfx();
        
        _currentChoppingInputIndex++;
        
        if (_currentChoppingInputIndex == _currentChoppingChallenge.ChoppingInputIndices.Count)
        {
            StopChoppingChallenge();
        }
        
        _isWaitingForNextChopping = true;
    }
}
