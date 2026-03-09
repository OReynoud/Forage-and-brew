using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static CollectHapticChallengeManager Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private UnearthingHapticChallengeSo unearthingHapticChallengeSo;
    [SerializeField] private ScrapingHapticChallengeSo scrapingHapticChallengeSo;
    [SerializeField] private HarvestHapticChallengeSo harvestHapticChallengeSo;
    [SerializeField] private Animator characterAnimator;

    [Header("Ingredient Types")]
    [SerializeField] private IngredientTypeSo scythingIngredientType;
    [SerializeField] private IngredientTypeSo unearthingIngredientType;
    [SerializeField] private IngredientTypeSo scrapingIngredientType;
    [SerializeField] private IngredientTypeSo harvestIngredientType;
    [SerializeField] private IngredientTypeSo extractIngredientType;
    
    [Header("Visuals")]
    [SerializeField] private float characterScythingDistance = 1f;
    [SerializeField] private float characterUnearthingDistance = 1f;
    [SerializeField] private float characterScrapingDistance = 1f;
    [SerializeField] private float characterHarvestDistance = 1f;
    [SerializeField] private float characterExtractDistance = 1f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource collectAudioSource;
    
    [HideInInspector]public UnityEvent<IngredientValuesSo> UpdateCounters = new UnityEvent<IngredientValuesSo>();
    
    // Global variables
    private bool _callCodexOnAnimationEnd;
    public List<IngredientToCollectBehaviour> CurrentIngredientToCollectBehaviours { get; } = new();
    private IngredientToCollectBehaviour _currentIngredientToCollectBehaviour;
    private GardenPlotBehavior _currentGardenPlotBehaviour;
    public Vector2 JoystickInputValue { get; set; }
    private Vector2 _lastJoystickInputValue;
    
    // Unearthing
    private float _currentUnearthingTime;
    private int _unearthingInputIndexAlreadyPressed;
    private bool _areBothUnearthingInputsPressed;
    private bool _canValidateUnearthing;
    private int _unearthingInputIndexAlreadyReleased;
    
    // Scraping
    private bool _isScrapingHapticChallengeActive;
    private Vector2 _firstScrapingJoystickPosition;
    
    // Harvest
    private float _currentHarvestTime;
    private bool _canValidateHarvest;
    
    // Extract
    private bool _canValidateExtract;
    
    // Animator Hashes
    private static readonly int DoBuildUpHarvest = Animator.StringToHash("DoBuildUpHarvest");
    private static readonly int DoCancelHarvest = Animator.StringToHash("DoCancelHarvest");
    private static readonly int DoHarvest = Animator.StringToHash("DoHarvest");
    private static readonly int DoScrape = Animator.StringToHash("DoScrape");
    private static readonly int DoScythe = Animator.StringToHash("DoScythe");
    private static readonly int DoBuildUpUnearth = Animator.StringToHash("DoBuildUpUnearth");
    private static readonly int DoCancelUnearth = Animator.StringToHash("DoCancelUnearth");
    private static readonly int DoUnearth = Animator.StringToHash("DoUnearth");


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameDontDestroyOnLoadManager.Instance.OnNewIngredientCollected.AddListener(CodexCall);
    }

    private void Update()
    {
        if (CurrentIngredientToCollectBehaviours.Count == 0) return;
        
        UpdateUnearthing();
        
        UpdateScraping();
        
        UpdateHarvest();
    }


    #region Scything

    public void CheckScythingInput()
    {
        SortIngredientsByDistance();
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (!ingredientToCollectBehaviour.IngredientValuesSo) continue;

            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != scythingIngredientType) continue;
            
            _currentIngredientToCollectBehaviour = ingredientToCollectBehaviour;
        
            characterAnimator.SetTrigger(DoScythe);
            _currentIngredientToCollectBehaviour.IngredientToCollectVfxManagerBehaviour.PlayScythingVfx();
            CharacterInputManager.Instance.DisableMoveInputs();

            FaceIngredient(characterScythingDistance, _currentIngredientToCollectBehaviour.transform);

            CharacterMovementController.Instance.FinishWalkToLocation.AddListener(CollectScythingIngredient);
            
            return;
        }
    }
    
    private void CollectScythingIngredient()
    {
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(CollectScythingIngredient);
        
        CollectIngredient();
    }

    #endregion


    #region Unearthing
    
    private void UpdateUnearthing()
    {
        if (_currentUnearthingTime <= 0f) return;
        
        _currentUnearthingTime -= Time.deltaTime;
        
        if (_currentUnearthingTime <= 0f)
        {
            _canValidateUnearthing = false;
            _areBothUnearthingInputsPressed = false;
            _unearthingInputIndexAlreadyReleased = 0;
        
            _currentIngredientToCollectBehaviour.PressUnearthing();
            
            characterAnimator.SetTrigger(DoCancelUnearth);
            CharacterInputManager.Instance.EnableMoveInputs();
        }
    }
    
    public void CheckUnearthingInputPressed(int inputIndex)
    {
        if (_unearthingInputIndexAlreadyPressed == 0)
        {
            SortIngredientsByDistance();
        
            foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
            {
                if (ingredientToCollectBehaviour.IngredientValuesSo.Type != unearthingIngredientType) continue;
            
                _currentIngredientToCollectBehaviour = ingredientToCollectBehaviour;
                _unearthingInputIndexAlreadyPressed = inputIndex;
                
                break;
            }
            
            return;
        }
        
        if (inputIndex != _unearthingInputIndexAlreadyPressed)
        {
            _areBothUnearthingInputsPressed = true;
            _currentIngredientToCollectBehaviour.ReleaseUnearthing();

            float heightDifference = _currentIngredientToCollectBehaviour.transform.position.y - transform.position.y;
            heightDifference = Mathf.Abs(heightDifference);
            int index = characterAnimator.GetLayerIndex("Unearth_Spine");
            characterAnimator.SetLayerWeight(index,1 - heightDifference);
            characterAnimator.SetTrigger(DoBuildUpUnearth);
            CharacterInputManager.Instance.DisableMoveInputs();
            CharacterInputManager.Instance.DisableCodexInputs();
            
            FaceIngredient(characterUnearthingDistance, _currentIngredientToCollectBehaviour.transform);
        }
    }
    
    public void CheckUnearthingInputReleased(int inputIndex)
    {
        if (!_areBothUnearthingInputsPressed)
        {
            _unearthingInputIndexAlreadyPressed = 0;
            return;
        }
        
        CharacterInputManager.Instance.EnableCodexInputs();

        if (!_canValidateUnearthing)
        {
            _unearthingInputIndexAlreadyReleased = inputIndex;
            _unearthingInputIndexAlreadyPressed = inputIndex % 2 + 1;
            _canValidateUnearthing = true;
            _currentUnearthingTime = unearthingHapticChallengeSo.InputReleaseDelayTolerance;
        }
        else
        {
            if (inputIndex != _unearthingInputIndexAlreadyReleased)
            {
                characterAnimator.SetTrigger(DoUnearth);
                _currentIngredientToCollectBehaviour.IngredientToCollectVfxManagerBehaviour.PlayUnearthingVfx();
                
                CollectIngredient();
                _currentUnearthingTime = 0f;
                _unearthingInputIndexAlreadyPressed = 0;
                _unearthingInputIndexAlreadyReleased = 0;
                _areBothUnearthingInputsPressed = false;
                _canValidateUnearthing = false;
                return;
            }
        }
    }

    #endregion
    
    
    #region Scraping
    
    private void UpdateScraping()
    {
        if (JoystickInputValue.magnitude < 1f - scrapingHapticChallengeSo.JoystickMagnitudeTolerance &&
            _firstScrapingJoystickPosition == Vector2.zero) return;
        
        if (JoystickInputValue.magnitude >= 1f - scrapingHapticChallengeSo.JoystickMagnitudeTolerance &&
            _firstScrapingJoystickPosition != Vector2.zero && Vector2.Angle(_firstScrapingJoystickPosition,
                JoystickInputValue) < scrapingHapticChallengeSo.AngleToTravel) return;

        if (!_isScrapingHapticChallengeActive)
        {
            if (!_currentIngredientToCollectBehaviour)
            {
                SortIngredientsByDistance();
        
                foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
                {
                    if (!ingredientToCollectBehaviour.IngredientValuesSo) continue;

                    if (ingredientToCollectBehaviour.IngredientValuesSo.Type != scrapingIngredientType) continue;
            
                    _currentIngredientToCollectBehaviour = ingredientToCollectBehaviour;
                    _firstScrapingJoystickPosition = JoystickInputValue;
                
                    _isScrapingHapticChallengeActive = true;
                
                    break;
                }
            }
            
            return;
        }

        if (JoystickInputValue.magnitude < 1f - scrapingHapticChallengeSo.JoystickMagnitudeTolerance)
        {
            _firstScrapingJoystickPosition = Vector2.zero;
            _currentIngredientToCollectBehaviour = null;
            _isScrapingHapticChallengeActive = false;
            return;
        }
        
        _firstScrapingJoystickPosition = Vector2.zero;
        
        characterAnimator.SetTrigger(DoScrape);
        _currentIngredientToCollectBehaviour.IngredientToCollectVfxManagerBehaviour.PlayScrapingVfx();
        CharacterInputManager.Instance.DisableMoveInputs();

        FaceIngredient(characterScrapingDistance, _currentIngredientToCollectBehaviour.transform);
        
        _isScrapingHapticChallengeActive = false;
        
        CollectIngredient();
    }

    #endregion
    
    
    #region Harvest
    
    private void UpdateHarvest()
    {
        if (_currentHarvestTime <= 0f) return;
        
        _currentHarvestTime -= Time.deltaTime;
        _currentIngredientToCollectBehaviour.SetHarvestValue(1f - _currentHarvestTime / harvestHapticChallengeSo.InputReleaseDelayTolerance);
        
        if (_currentHarvestTime <= 0f)
        {
            _canValidateHarvest = true;
            
            RumbleManager.Instance.PlayRumble(harvestHapticChallengeSo.InputReleaseVibrationDuration,
                harvestHapticChallengeSo.InputReleaseVibrationPower);
            
            _currentIngredientToCollectBehaviour.ReleaseHarvest();
        }
    }
    
    public void CheckHarvestInputPressed()
    {
        SortIngredientsByDistance();
        
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (!ingredientToCollectBehaviour.IngredientValuesSo) continue;

            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != harvestIngredientType) continue;
            
            _currentIngredientToCollectBehaviour = ingredientToCollectBehaviour;
            _currentHarvestTime = harvestHapticChallengeSo.InputReleaseDelayTolerance;
            
            characterAnimator.SetTrigger(DoBuildUpHarvest);
            CharacterInputManager.Instance.DisableMoveInputs();
            CharacterInputManager.Instance.DisableCodexInputs();
            
            FaceIngredient(characterHarvestDistance, _currentIngredientToCollectBehaviour.transform);
            
            break;
        }
    }
    
    public void CheckHarvestInputReleased()
    {
        if (_currentHarvestTime == 0f && !_canValidateHarvest) return;
        
        _currentHarvestTime = 0f;
        _currentIngredientToCollectBehaviour.SetHarvestValue(0f);
        
        CharacterInputManager.Instance.EnableCodexInputs();
        
        if (!_canValidateHarvest)
        {
            _currentIngredientToCollectBehaviour = null;
            characterAnimator.SetTrigger(DoCancelHarvest);
            CharacterInputManager.Instance.EnableMoveInputs();
            return;
        }
        
        _canValidateHarvest = false;
        
        characterAnimator.SetTrigger(DoHarvest);
        _currentIngredientToCollectBehaviour.IngredientToCollectVfxManagerBehaviour.PlayHarvestVfx();
        
        CollectIngredient();
    }
    
    #endregion
    
    
    #region Extract
    
    public void CheckExtractHoldInputPressed()
    {
        SortIngredientsByDistance();
        
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (!ingredientToCollectBehaviour.IngredientValuesSo) continue;

            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != extractIngredientType) continue;
            
            _currentIngredientToCollectBehaviour = ingredientToCollectBehaviour;
            _currentIngredientToCollectBehaviour.PressExtract();
            
            _canValidateExtract = true;
            
            characterAnimator.SetTrigger(DoBuildUpHarvest);
            CharacterInputManager.Instance.DisableMoveInputs();
            CharacterInputManager.Instance.DisableCodexInputs();
            
            FaceIngredient(characterExtractDistance, _currentIngredientToCollectBehaviour.transform);
            
            break;
        }
    }
    
    public void CheckExtractHoldInputReleased()
    {
        if (!_canValidateExtract) return;
        
        _currentIngredientToCollectBehaviour.HoldExtract();
        
        _currentIngredientToCollectBehaviour = null;
        characterAnimator.SetTrigger(DoCancelHarvest);
        
        CharacterInputManager.Instance.EnableCodexInputs();
        CharacterInputManager.Instance.EnableMoveInputs();
        
        _canValidateExtract = false;
    }
    
    public void CheckExtractPressInput()
    {
        if (!_canValidateExtract) return;
        
        CharacterInputManager.Instance.EnableCodexInputs();
        
        _canValidateExtract = false;
        
        characterAnimator.SetTrigger(DoHarvest);
        _currentIngredientToCollectBehaviour.IngredientToCollectVfxManagerBehaviour.PlayHarvestVfx();
        
        CollectIngredient();
    }
    
    #endregion
    
    
    private void SortIngredientsByDistance()
    {
        CurrentIngredientToCollectBehaviours.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
    }

    private void FaceIngredient(float characterDistance, Transform ingredientTransform)
    {
        Vector3 ingredientPosition = new(ingredientTransform.position.x,
            transform.position.y, ingredientTransform.position.z);
        transform.LookAt(ingredientPosition);
        CharacterInputManager.Instance.DisableMoveInputs();
        CharacterMovementController.Instance.TriggerWalkTransition(ingredientPosition - transform.forward * characterDistance);
        CharacterMovementController.Instance.FinishWalkToLocation.AddListener(LookAtIngredient);
    }

    void LookAtIngredient()
    {
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(LookAtIngredient);
        
        Vector3 ingredientPosition;
        if (_currentIngredientToCollectBehaviour)
        {
            ingredientPosition = new Vector3(_currentIngredientToCollectBehaviour.transform.position.x,
                transform.position.y, _currentIngredientToCollectBehaviour.transform.position.z);

            transform.LookAt(ingredientPosition);
        }
        else if (_currentGardenPlotBehaviour)
        {
            ingredientPosition = new Vector3(_currentGardenPlotBehaviour.transform.position.x,
                transform.position.y, _currentGardenPlotBehaviour.transform.position.z);

            transform.LookAt(ingredientPosition);
        }
    }

    private void CollectIngredient()
    {
        // Audio
        collectAudioSource.Play();
        _currentIngredientToCollectBehaviour.Collect();
        UpdateCounters.Invoke(_currentIngredientToCollectBehaviour.IngredientValuesSo);
        CurrentIngredientToCollectBehaviours.Remove(_currentIngredientToCollectBehaviour);
        _currentIngredientToCollectBehaviour = null;
    }
    
    public void RemoveIngredientToCollectBehaviour(IngredientToCollectBehaviour ingredientToCollectBehaviour)
    {
        CurrentIngredientToCollectBehaviours.Remove(ingredientToCollectBehaviour);
        
        if (_currentIngredientToCollectBehaviour == ingredientToCollectBehaviour)
        {
            _currentIngredientToCollectBehaviour = null;
            _isScrapingHapticChallengeActive = false;
        }
    }

    
    public void OnCollectAnimationEnd()
    {
        CharacterInputManager.Instance.EnableMoveInputs();
        TutorialManager.instance.NotifyFromIngredientReceived();
        if (_callCodexOnAnimationEnd)
        {
            AutoFlip.instance.ControledBook.DisplayNewIngredient();
            _callCodexOnAnimationEnd = false;
        }
    }

    public void CodexCall(IngredientValuesSo arg0)
    {
        _callCodexOnAnimationEnd = true;
    }
}
