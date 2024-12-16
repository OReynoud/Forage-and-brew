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

    [Header("Ingredient Types")]
    [SerializeField] private IngredientType scythingIngredientType = IngredientType.Herb;
    [SerializeField] private IngredientType unearthingIngredientType = IngredientType.Mushroom;
    [SerializeField] private IngredientType scrapingIngredientType = IngredientType.Moss;
    [SerializeField] private IngredientType harvestIngredientType = IngredientType.Berry;
    
    // Global variables
    private bool _isCollectHapticChallengeActive;
    public List<IngredientToCollectBehaviour> CurrentIngredientToCollectBehaviours { get; } = new();
    public Vector2 JoystickInputValue { get; set; }
    private Vector2 _lastJoystickInputValue;
    
    // Unearthing
    private float _currentUnearthingTime;
    private bool _canValidateUnearthing;
    private int _inputIndexAlreadyDone;
    
    // Scraping
    private Vector2 _firstScrapingJoystickPosition;
    
    // Harvest
    private float _currentHarvestTime;
    private bool _canValidateHarvest;
    
    
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateUnearthing();
        
        UpdateScraping();
        
        UpdateHarvest();
    }


    #region Scything

    public void CheckScythingInput()
    {
        CurrentIngredientToCollectBehaviours.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != scythingIngredientType) continue;
            
            ingredientToCollectBehaviour.Collect();
            CurrentIngredientToCollectBehaviours.Remove(ingredientToCollectBehaviour);
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
        }
    }
    
    public void CheckUnearthingInput(int inputIndex)
    {
        CurrentIngredientToCollectBehaviours.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != unearthingIngredientType) continue;
            
            if (_canValidateUnearthing)
            {
                if (inputIndex != _inputIndexAlreadyDone)
                {
                    ingredientToCollectBehaviour.Collect();
                    CurrentIngredientToCollectBehaviours.Remove(ingredientToCollectBehaviour);
                    return;
                }
            }
            else
            {
                _inputIndexAlreadyDone = inputIndex;
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
        
        if (JoystickInputValue.magnitude > 1f - scrapingHapticChallengeSo.JoystickMagnitudeTolerance &&
            _firstScrapingJoystickPosition != Vector2.zero) return;
        
        CurrentIngredientToCollectBehaviours.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (!ingredientToCollectBehaviour.IngredientValuesSo) continue;

            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != scrapingIngredientType) continue;
            
            if (_firstScrapingJoystickPosition == Vector2.zero)
            {
                _firstScrapingJoystickPosition = JoystickInputValue;
            }
            else
            {
                if (Vector2.Angle(_firstScrapingJoystickPosition, JoystickInputValue) >= scrapingHapticChallengeSo.AngleToTravel)
                {
                    ingredientToCollectBehaviour.Collect();
                    CurrentIngredientToCollectBehaviours.Remove(ingredientToCollectBehaviour);
                    return;
                }
                    
                _firstScrapingJoystickPosition = Vector2.zero;
            }
                
            break;
        }
    }

    #endregion
    
    
    #region Harvest
    
    private void UpdateHarvest()
    {
        if (_currentHarvestTime <= 0f) return;
        
        _currentHarvestTime -= Time.deltaTime;
        
        if (_currentHarvestTime <= 0f)
        {
            _canValidateHarvest = true;
        }
    }
    
    public void CheckHarvestInputPressed()
    {
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != harvestIngredientType) continue;
            
            _currentHarvestTime = harvestHapticChallengeSo.InputReleaseDelayTolerance;
                
            break;
        }
    }
    
    public void CheckHarvestInputReleased()
    {
        _currentHarvestTime = 0f;
        
        if (!_canValidateHarvest) return;
        
        _canValidateHarvest = false;
        
        CurrentIngredientToCollectBehaviours.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
        
        foreach (IngredientToCollectBehaviour ingredientToCollectBehaviour in CurrentIngredientToCollectBehaviours)
        {
            if (ingredientToCollectBehaviour.IngredientValuesSo.Type != harvestIngredientType) continue;
            
            ingredientToCollectBehaviour.Collect();
            CurrentIngredientToCollectBehaviours.Remove(ingredientToCollectBehaviour);
            return;
        }
    }
    
    #endregion
}
