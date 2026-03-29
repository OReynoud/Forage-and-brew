using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ChoppingHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static ChoppingHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private ChoppingHapticChallengeListSo choppingHapticChallengeListSo;
    [SerializeField] private Rigidbody characterRigidbody;
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
    private Tweener _cameraTransitionTweener;
    
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
        List<ChoppingHapticChallengeSo> choppingHapticChallenges = choppingHapticChallengeListSo
            .ChoppingHapticChallenges.Where(choppingHapticChallenge =>
                choppingHapticChallenge.ChoppingInputIndices.Count ==
                CurrentChoppingCountertopBehaviour.CollectedIngredients[0].IngredientValuesSo.CutActionCount).ToList();
        _currentChoppingChallenge = choppingHapticChallenges[Random.Range(0, choppingHapticChallenges.Count)];
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
        CurrentChoppingCountertopBehaviour.IsCharacterOnCountertop = true;
        CurrentChoppingCountertopBehaviour.SetCutIngredients();
        
        // Inputs
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnableChoppingHapticChallengeInputs();
        
        // Camera
        _cameraTransitionTweener?.Kill();
        _previousCameraPreset = SimpleCameraBehavior.instance.TargetCamSettings;
        ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = true;
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(choppingChallengeCameraPreset, choppingCameraTransitionTime);

        // Character
        transform.position = CurrentChoppingCountertopBehaviour.transform.position + CurrentChoppingCountertopBehaviour.transform.rotation * characterChoppingPosition;
        transform.rotation = CurrentChoppingCountertopBehaviour.transform.rotation * Quaternion.Euler(characterChoppingRotation);
        characterRigidbody.isKinematic = true;
        
        // Animation
        characterAnimator.SetBool(IsChopping, true);
        knifeGameObject.SetActive(true);
        characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Carry"),0);
        
        // Put in HapticChallenge
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = true;
        
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
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(_previousCameraPreset, choppingCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        // CurrentChoppingCountertopBehaviour.EnableInteract();
        characterRigidbody.isKinematic = false;
        characterAnimator.SetBool(IsChopping, false);
        knifeGameObject.SetActive(false);
        CurrentChoppingCountertopBehaviour.IsCharacterOnCountertop = false;
        
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = false;
        
        _cameraTransitionTweener = DOTween.To(() => transform.position, x => transform.position = x, transform.position,
            choppingCameraTransitionTime).OnComplete(
            () =>
            {
                ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = false;
            });
        
        CurrentChoppingCountertopBehaviour.ChopIngredient(choppingHapticChallengeListSo);
        
        //Animation
        characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Carry"),1);
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
        
            // Rumble
            RumbleManager.Instance.PlayMultipleRumbles(choppingHapticChallengeListSo.WrongInputVibrationDurations,
                choppingHapticChallengeListSo.WrongInputVibrationPower, choppingHapticChallengeListSo.WrongInputVibrationIntervals);
        }
        else
        {
            _choppingInputBehaviours[_currentChoppingInputIndex].SetRightInput();
        
            // Animation
            characterAnimator.SetTrigger(DoChop);
        
            // VFX
            CurrentChoppingCountertopBehaviour.CountertopVfxManager.PlayChopVfx();
            
            // Sound
            CurrentChoppingCountertopBehaviour.PlayChoppingSound();
        
            // Rumble
            RumbleManager.Instance.PlayRumble(choppingHapticChallengeListSo.CorrectInputVibrationDuration,
                choppingHapticChallengeListSo.CorrectInputVibrationPower);
        
            _currentChoppingInputIndex++;
        
            if (_currentChoppingInputIndex == _currentChoppingChallenge.ChoppingInputIndices.Count)
            {
                StopChoppingChallenge();
            }
            else
            {
                CurrentChoppingCountertopBehaviour.SetCutIngredientPositionAndRotation(_currentChoppingInputIndex);
                _isWaitingForNextChopping = true;
            }
        }
    }
}
