using System.Collections.Generic;
using UnityEngine;

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
    
    [Header("Visuals")]
    [SerializeField] private float characterScythingDistance = 1f;
    [SerializeField] private float characterScrapingDistance = 1f;
    [SerializeField] private float characterHarvestDistance = 1f;
    
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
    
    // Animator Hashes
    private static readonly int DoBuildUpHarvest = Animator.StringToHash("DoBuildUpHarvest");
    private static readonly int DoCancelHarvest = Animator.StringToHash("DoCancelHarvest");
    private static readonly int DoHarvest = Animator.StringToHash("DoHarvest");
    private static readonly int DoScrape = Animator.StringToHash("DoScrape");
    private static readonly int DoScythe = Animator.StringToHash("DoScythe");


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
        
            foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
            {
                if (ingredientToCollectBehaviour.IngredientValuesSo.Type != unearthingIngredientType) continue;
            
                ingredientToCollectBehaviour.PressUnearthing();
                
                break;
            }
        }
    }
    
    public void CheckUnearthingInputPressed(int inputIndex)
    {
        CurrentIngredientToCollectBehaviours.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != unearthingIngredientType) continue;
            
            if (_unearthingInputIndexAlreadyPressed != 0)
            {
                if (inputIndex != _unearthingInputIndexAlreadyPressed)
                {
                    _areBothUnearthingInputsPressed = true;
                    ingredientToCollectBehaviour.ReleaseUnearthing();
                }
            }
            else
            {
                _unearthingInputIndexAlreadyPressed = inputIndex;
            }
                
            break;
        }
    }
    
    public void CheckUnearthingInputReleased(int inputIndex)
    {
        if (!_areBothUnearthingInputsPressed)
        {
            _unearthingInputIndexAlreadyPressed = 0;
            return;
        }
        
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != unearthingIngredientType) continue;
            
            if (_canValidateUnearthing)
            {
                if (inputIndex != _unearthingInputIndexAlreadyReleased)
                {
                    ingredientToCollectBehaviour.Collect();
                    ingredientToCollectBehaviour.IngredientToCollectVfxManagerBehaviour.PlayUnearthingVfx();
                    CurrentIngredientToCollectBehaviours.Remove(ingredientToCollectBehaviour);
                    _unearthingInputIndexAlreadyPressed = 0;
                    _unearthingInputIndexAlreadyReleased = 0;
                    _areBothUnearthingInputsPressed = false;
                    _canValidateUnearthing = false;
                    return;
                }
            }
            else
            {
                _unearthingInputIndexAlreadyReleased = inputIndex;
                _unearthingInputIndexAlreadyPressed = inputIndex % 2 + 1;
                _canValidateUnearthing = true;
                _currentUnearthingTime = unearthingHapticChallengeSo.InputReleaseDelayTolerance;
            }
                
            break;
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

    public void OnHarvestAnimationEnd()
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
        transform.position = ingredientPosition - transform.forward * characterDistance;
    }

    private void CollectIngredient()
    {
        _currentIngredientToCollectBehaviour.Collect();
        CurrentIngredientToCollectBehaviours.Remove(_currentIngredientToCollectBehaviour);
        _currentIngredientToCollectBehaviour = null;
    }
}
