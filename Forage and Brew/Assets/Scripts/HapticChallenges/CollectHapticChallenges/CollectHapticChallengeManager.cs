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
    [SerializeField] private WeedingHapticChallengeSo weedingHapticChallengeSo;
    [SerializeField] private Animator characterAnimator;

    [Header("Ingredient Types")]
    [SerializeField] private IngredientTypeSo scythingIngredientType;
    [SerializeField] private IngredientTypeSo unearthingIngredientType;
    [SerializeField] private IngredientTypeSo scrapingIngredientType;
    [SerializeField] private IngredientTypeSo harvestIngredientType;
    
    [Header("Visuals")]
    [SerializeField] private float characterScythingDistance = 1f;
    [SerializeField] private float characterUnearthingDistance = 1f;
    [SerializeField] private float characterScrapingDistance = 1f;
    [SerializeField] private float characterHarvestDistance = 1f;
    [SerializeField] private float characterWeedingDistance = 1f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource collectAudioSource;
    
    [HideInInspector]public UnityEvent<IngredientValuesSo> UpdateCounters = new UnityEvent<IngredientValuesSo>();
    
    // Global variables
    private bool _isCollectHapticChallengeActive;
    private bool _callCodexOnAnimationEnd;
    public List<IngredientToCollectBehaviour> CurrentIngredientToCollectBehaviours { get; } = new();
    private IngredientToCollectBehaviour _currentIngredientToCollectBehaviour;
    public Vector2 JoystickInputValue { get; set; }
    private Vector2 _lastJoystickInputValue;
    
    // Unearthing
    private float _currentUnearthingTime;
    private int _unearthingInputIndexAlreadyPressed;
    private bool _areBothUnearthingInputsPressed;
    private bool _canValidateUnearthing;
    private int _unearthingInputIndexAlreadyReleased;
    
    // Scraping
    private Vector2 _firstScrapingJoystickPosition;
    
    // Harvest
    private float _currentHarvestTime;
    private bool _canValidateHarvest;
    
    // Weeding
    private int _currentWeedingInputIndex;
    private float _currentWeedingTime;
    private bool _canValidateWeeding;
    
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
        
        UpdateWeeding();
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

            FaceIngredient(characterScythingDistance);

            CollectIngredient();
            
            return;
        }
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
            
            FaceIngredient(characterUnearthingDistance);
        }
    }
    
    public void CheckUnearthingInputReleased(int inputIndex)
    {
        if (!_areBothUnearthingInputsPressed)
        {
            _unearthingInputIndexAlreadyPressed = 0;
            _currentIngredientToCollectBehaviour = null;
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

        if (!_currentIngredientToCollectBehaviour)
        {
            SortIngredientsByDistance();
        
            foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
            {
                if (!ingredientToCollectBehaviour.IngredientValuesSo) continue;

                if (ingredientToCollectBehaviour.IngredientValuesSo.Type != scrapingIngredientType) continue;
            
                _currentIngredientToCollectBehaviour = ingredientToCollectBehaviour;
                _firstScrapingJoystickPosition = JoystickInputValue;
                
                break;
            }
            
            return;
        }

        if (JoystickInputValue.magnitude < 1f - scrapingHapticChallengeSo.JoystickMagnitudeTolerance)
        {
            _firstScrapingJoystickPosition = Vector2.zero;
            _currentIngredientToCollectBehaviour = null;
            return;
        }
        
        _firstScrapingJoystickPosition = Vector2.zero;
        
        characterAnimator.SetTrigger(DoScrape);
        _currentIngredientToCollectBehaviour.IngredientToCollectVfxManagerBehaviour.PlayScrapingVfx();
        CharacterInputManager.Instance.DisableMoveInputs();

        FaceIngredient(characterScrapingDistance);
        
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
            
            FaceIngredient(characterHarvestDistance);
            
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
    
    
    #region Weeding
    
    private void UpdateWeeding()
    {
        if (_currentWeedingTime <= 0f) return;
        
        _currentWeedingTime -= Time.deltaTime;
        _currentIngredientToCollectBehaviour.SetWeedingValue(1f - _currentWeedingTime /
            weedingHapticChallengeSo.InputReleaseDelayTolerances[_currentWeedingInputIndex],
            _currentWeedingInputIndex);
        
        if (_currentWeedingTime <= 0f)
        {
            _canValidateWeeding = true;
            
            RumbleManager.Instance.PlayRumble(weedingHapticChallengeSo.InputReleaseVibrationDuration,
                weedingHapticChallengeSo.InputReleaseVibrationPower);
            
            _currentIngredientToCollectBehaviour.ReleaseWeeding();
        }
    }
    
    public void CheckWeedingInputPressed()
    {
        if (!_isCollectHapticChallengeActive)
        {
            SortIngredientsByDistance();
        
            foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
            {
                if (!ingredientToCollectBehaviour.IsWeed) continue;
                
                _isCollectHapticChallengeActive = true;
            
                _currentIngredientToCollectBehaviour = ingredientToCollectBehaviour;
            
                FaceIngredient(characterWeedingDistance);
            
                break;
            }
            
            if (!_isCollectHapticChallengeActive) return;
        }
        
        _currentWeedingTime = weedingHapticChallengeSo.InputReleaseDelayTolerances[_currentWeedingInputIndex];
        
        CharacterInputManager.Instance.DisableMoveInputs();
        CharacterInputManager.Instance.DisableCodexInputs();
        characterAnimator.SetTrigger(DoBuildUpHarvest);
    }
    
    public void CheckWeedingInputReleased()
    {
        if (_currentWeedingTime == 0f && !_canValidateWeeding) return;
        
        _currentWeedingTime = 0f;
        
        CharacterInputManager.Instance.EnableCodexInputs();
        
        if (!_canValidateWeeding)
        {
            _currentIngredientToCollectBehaviour.SetWeedingValue(0f, _currentWeedingInputIndex);
            characterAnimator.SetTrigger(DoCancelHarvest);
            CharacterInputManager.Instance.EnableMoveInputs();
            return;
        }
        
        _canValidateWeeding = false;
        
        _currentIngredientToCollectBehaviour.PressWeeding();
        characterAnimator.SetTrigger(DoHarvest);
        
        _currentWeedingInputIndex++;
        
        if (_currentWeedingInputIndex >= weedingHapticChallengeSo.InputReleaseDelayTolerances.Count)
        {
            ResetWeeding();
            _isCollectHapticChallengeActive = false;
            
            _currentIngredientToCollectBehaviour.IngredientToCollectVfxManagerBehaviour.PlayWeedingVfx();
        
            CollectIngredient(true);
        }
        else
        {
            _currentIngredientToCollectBehaviour.ChangeWeedingInputIndex(_currentWeedingInputIndex);
        }
    }
    
    public void ResetWeeding()
    {
        _currentWeedingInputIndex = 0;
        _currentIngredientToCollectBehaviour.ResetWeedingInputIndex();
    }
    
    #endregion
    
    
    private void SortIngredientsByDistance()
    {
        CurrentIngredientToCollectBehaviours.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
    }

    private void FaceIngredient(float characterDistance)
    {
        Vector3 ingredientPosition = new(_currentIngredientToCollectBehaviour.transform.position.x,
            transform.position.y, _currentIngredientToCollectBehaviour.transform.position.z);
        transform.LookAt(ingredientPosition);
        CharacterInputManager.Instance.DisableMoveInputs();
        CharacterMovementController.Instance.TriggerWalkTransition(ingredientPosition - transform.forward * characterDistance);
        CharacterMovementController.Instance.FinishWalkToLocation.AddListener(LookAtIngredient);
    }

    void LookAtIngredient()
    {
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(LookAtIngredient);
        Vector3 ingredientPosition = new(_currentIngredientToCollectBehaviour.transform.position.x,
            transform.position.y, _currentIngredientToCollectBehaviour.transform.position.z);
        transform.LookAt(ingredientPosition);
    }

    private void CollectIngredient(bool isWeed = false)
    {
        // Audio
        collectAudioSource.Play();

        if (isWeed)
        {
            _currentIngredientToCollectBehaviour.RemoveWeed();
        }
        else
        {
            _currentIngredientToCollectBehaviour.Collect();
            UpdateCounters.Invoke(_currentIngredientToCollectBehaviour.IngredientValuesSo);
        }
        
        CurrentIngredientToCollectBehaviours.Remove(_currentIngredientToCollectBehaviour);
        _currentIngredientToCollectBehaviour = null;
    }
    
    public void RemoveIngredientToCollectBehaviour(IngredientToCollectBehaviour ingredientToCollectBehaviour)
    {
        CurrentIngredientToCollectBehaviours.Remove(ingredientToCollectBehaviour);
        
        if (_currentIngredientToCollectBehaviour == ingredientToCollectBehaviour)
        {
            ResetWeeding();
            _currentIngredientToCollectBehaviour = null;
            _isCollectHapticChallengeActive = false;
        }
    }

    
    public void OnCollectAnimationEnd()
    {
        CharacterInputManager.Instance.EnableMoveInputs();
        if (_callCodexOnAnimationEnd)
        {
            TutorialManager.instance.NotifyFromIngredientReceived();
            AutoFlip.instance.ControledBook.DisplayNewIngredient();
            _callCodexOnAnimationEnd = false;
        }
    }

    public void CodexCall(IngredientValuesSo arg0)
    {
        _callCodexOnAnimationEnd = true;
    }
}
