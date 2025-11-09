using DG.Tweening;
using UnityEngine;

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
        CurrentCompost.DisableInteract();
        
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
        
        compostChallengeGameObject.SetActive(false);
        
        // Sound
        if (!fromQuit)
        {
            characterAnimator.SetBool(IsStirring, false);
        }
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(_previousCameraPreset, compostCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        CurrentCompost.EnableInteract();
        
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
        if (_lastInputIndex == i) return;
        
        _lastInputIndex = i;
        _currentCompostIndex++;

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
        if (_currentObtainedSeedAnimationTime <= compostHapticChallengeGlobalValuesSo.ObtainedSeedAnimationDuration)
        {
            _currentObtainedSeedAnimationTime += Time.deltaTime;
        }
        else
        {
            _isObtainedSeedAnimationPlaying = false;
            StopCompostChallenge();
        }
    }

    private void ObtainSeed()
    {
        // CompostVfxManager.Instance.PlayObtainedSeedVfx();
        characterAnimator.SetTrigger(PotionSuccess);
        characterAnimator.SetBool(IsStirring, false);
        _isObtainedSeedAnimationPlaying = true;
        _currentObtainedSeedAnimationTime = 0f;
    }

    #endregion
}
