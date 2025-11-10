using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompostHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static CompostHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private CompostHapticChallengeGlobalValuesSo compostHapticChallengeGlobalValuesSo;
    [SerializeField] private CollectedSeedBehaviour collectedSeedPrefab;
    
    [Header("UI")]
    [SerializeField] private GameObject compostChallengeGameObject;
    [SerializeField] private GameObject leftInputGameObject;
    [SerializeField] private GameObject rightInputGameObject;
    [SerializeField] private RectTransform obtainedSeedRectTransform;
    [SerializeField] private TMP_Text obtainedSeedNameText;
    [SerializeField] private Image obtainedSeedVegetableImage;
    
    [Header("Camera")]
    [SerializeField] private float compostCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
    
    [Header("Character")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Vector3 characterCompostPosition;
    [SerializeField] private Vector3 characterCompostRotation;
    
    private bool _isInCompostChallenge;
    private int _currentCompostIndex;
    private int _lastInputIndex = -1;
    private bool _isObtainedSeedAnimationPlaying;
    private float _currentObtainedSeedAnimationTime;
    public GardenCompostBehaviour CurrentCompost { get; set; }
    
    // Animator Hashes
    private static readonly int IsStirring = Animator.StringToHash("IsStirring");
    private static readonly int PotionSuccess = Animator.StringToHash("PotionSuccess");


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        compostChallengeGameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isObtainedSeedAnimationPlaying)
        {
            UpdateObtainedSeedAnimation();
        }
    }
    
    
    public void StartCompostChallenge()
    {
        // Check if player is near a cauldron
        if (!CurrentCompost) return;
        
        // Camera
        ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = true;
        _previousCameraPreset = SimpleCameraBehavior.instance.TargetCamSettings;

        // Character
        transform.position = CurrentCompost.transform.position + characterCompostPosition;
        transform.rotation = Quaternion.Euler(characterCompostRotation);
        characterAnimator.SetBool(IsStirring, true);
        
        // Inputs
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnableCompostHapticChallengeInputs();
        CharacterInputManager.Instance.EnableQuitHapticChallengeInputs();
        
        // Compost Box
        CurrentCompost.DisableInteraction();
        
        // Challenge
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = true;
        _isInCompostChallenge = true;
        _lastInputIndex = -1;
        _currentCompostIndex = 0;
        
        // UI
        compostChallengeGameObject.SetActive(true);
        leftInputGameObject.SetActive(true);
        rightInputGameObject.SetActive(true);
    }

    public void StopCompostChallenge(bool fromQuit = false)
    {
        if (!_isInCompostChallenge) return;
        
        if (_isObtainedSeedAnimationPlaying) return;

        if (!fromQuit)
        {
            CurrentCompost.CompleteCompostHapticChallenge();
        }
        
        // Sound
        if (!fromQuit)
        {
            characterAnimator.SetBool(IsStirring, false);
        }
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(_previousCameraPreset, compostCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        compostChallengeGameObject.SetActive(false);
        
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = false;
        _isInCompostChallenge = false;
        
        DOTween.To(() => transform.position, x => transform.position = x, transform.position,
            compostCameraTransitionTime).OnComplete(
            () =>
            {
                ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = false;
            });
    }
    

    public void CheckInputCompostChallenge(int i)
    {
        if (!_isInCompostChallenge) return;
        
        if (_currentCompostIndex >= compostHapticChallengeGlobalValuesSo.InputCount) return;
        
        if (_lastInputIndex == i) return;
        
        _lastInputIndex = i;
        _currentCompostIndex++;
        
        CurrentCompost.BreakDownHumus();

        switch (i)
        {
            case 1:
                leftInputGameObject.SetActive(false);
                rightInputGameObject.SetActive(true);
                break;
            case 2:
                leftInputGameObject.SetActive(true);
                rightInputGameObject.SetActive(false);
                break;
        }
        
        if (_currentCompostIndex >= compostHapticChallengeGlobalValuesSo.InputCount)
        {
            ObtainSeed();
        }
    }


    #region Obtained Seed

    private void UpdateObtainedSeedAnimation()
    {
        _currentObtainedSeedAnimationTime += Time.deltaTime;
        
        if (_currentObtainedSeedAnimationTime <= compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationDuration)
        {
            obtainedSeedRectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one,
                compostHapticChallengeGlobalValuesSo.ObtainedSeedScaleAnimationCurve.Evaluate(
                    _currentObtainedSeedAnimationTime /
                    compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationDuration));
            
            obtainedSeedRectTransform.anchoredPosition = Vector3.Lerp(
                compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationStartPosition,
                compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationEndPosition,
                compostHapticChallengeGlobalValuesSo.ObtainedSeedPositionAnimationCurve.Evaluate(
                    _currentObtainedSeedAnimationTime /
                    compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationDuration));
        }
        else if (_currentObtainedSeedAnimationTime <= compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationDuration +
                 compostHapticChallengeGlobalValuesSo.ObtainedSeedStayDuration)
        {
            obtainedSeedRectTransform.localScale = Vector3.one;
            obtainedSeedRectTransform.anchoredPosition =
                compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationEndPosition;
        }
        else
        {
            obtainedSeedRectTransform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.zero,
                compostHapticChallengeGlobalValuesSo.ObtainedSeedScaleEndAnimationCurve.Evaluate(
                    (_currentObtainedSeedAnimationTime - compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationDuration
                                                         - compostHapticChallengeGlobalValuesSo.ObtainedSeedStayDuration) /
                    compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationEndDuration));
        }
            
        if (_currentObtainedSeedAnimationTime >= compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationDuration +
            compostHapticChallengeGlobalValuesSo.ObtainedSeedStayDuration +
            compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationEndDuration)
        {
            _isObtainedSeedAnimationPlaying = false;
            StopCompostChallenge();
        }
    }

    private void ObtainSeed()
    {
        CurrentCompost.PlayObtainedSeedVfx();
        obtainedSeedVegetableImage.sprite = CurrentCompost.currentSeed.IngredientToGrowSo.iconHigh;
        obtainedSeedNameText.text = CurrentCompost.currentSeed.IngredientToGrowSo.Name + " Seed";
        obtainedSeedRectTransform.localScale = Vector3.zero;
        obtainedSeedRectTransform.anchoredPosition =
            compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationStartPosition;
        obtainedSeedRectTransform.gameObject.SetActive(true);
        leftInputGameObject.SetActive(false);
        rightInputGameObject.SetActive(false);
        characterAnimator.SetTrigger(PotionSuccess);
        characterAnimator.SetBool(IsStirring, false);
        _isObtainedSeedAnimationPlaying = true;
        _currentObtainedSeedAnimationTime = 0f;
    }

    #endregion
}
