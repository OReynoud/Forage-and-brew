using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StirHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static StirHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private StirHapticChallengeGlobalValuesSo stirHapticChallengeGlobalValuesSo;
    [SerializeField] private PotionListSo potionListSo;
    
    [Header("UI")]
    [SerializeField] private GameObject stirChallengeGameObject;
    [SerializeField] private GameObject clockwiseArrowGameObject;
    [SerializeField] private Image clockwiseArrowImage;
    [SerializeField] private GameObject rotationMarkerGameObject;
    [SerializeField] private Image rotationMarkerImage;
    
    [Header("Camera")]
    [SerializeField] private CameraPreset cauldronCameraPreset;
    [SerializeField] private float cauldronCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
    
    private PotionValuesSo _currentPotion;
    private StirHapticChallengeSo _currentChallenge;
    private float _currentStirTime;
    private int _currentStirIndex;
    private int _currentCheckIndex;
    private bool _isCurrentStirClockwise;
    private bool _isInPreview;
    private bool _isInPreviewPause;
    private bool _isWaitingForInputReset;
    public CauldronBehaviour CurrentCauldron { get; set; }
    
    // Input
    public Vector2 JoystickInputValue { get; set; }
    private Vector2 _lastJoystickInputValue;
    private readonly List<float> _joystickInputDifferences = new();
    
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        stirChallengeGameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_currentChallenge) return;
        
        UpdateStirChallenge();
    }
    
    
    private void PickRightPotion()
    {
        foreach (PotionValuesSo potion in potionListSo.Potions)
        {
            List<IngredientValuesSo> ingredients = new();
            List<IngredientType> ingredientTypes = new();
            List<IngredientValuesSo> cauldronIngredients = new();

            foreach (IngredientValuesSo ingredient in CurrentCauldron.Ingredients)
            {
                cauldronIngredients.Add(ingredient);
            }
            
            foreach (CookedIngredientForm cookedIngredientForm in potion.CauldronHapticChallengeIngredients[0].CookedIngredients)
            {
                if (cookedIngredientForm.IsAType)
                {
                    ingredientTypes.Add(cookedIngredientForm.IngredientType);
                }
                else
                {
                    ingredients.Add(cookedIngredientForm.Ingredient);
                }
            }
            
            if (cauldronIngredients.Count != ingredients.Count + ingredientTypes.Count) continue;
            
            bool isRightPotion = true;
            
            foreach (IngredientValuesSo ingredient in ingredients)
            {
                if (!cauldronIngredients.Remove(ingredient))
                {
                    isRightPotion = false;
                    break;
                }
            }
            
            if (!isRightPotion) continue;
            
            foreach (IngredientType ingredientType in ingredientTypes)
            {
                if (!cauldronIngredients.Exists(ingredient => ingredient.Type == ingredientType))
                {
                    isRightPotion = false;
                    break;
                }
                
                cauldronIngredients.Remove(cauldronIngredients.Find(ingredient => ingredient.Type == ingredientType));
            }
            
            if (!isRightPotion) continue;
            
            _currentPotion = potion;
            
            return;
        }
        
        _currentPotion = potionListSo.DefaultPotion;
    }
    
    public void StartStirChallenge()
    {
        if (!CurrentCauldron) return;
        
        _previousCameraPreset = CameraController.instance.TargetCamSettings;
        CameraController.instance.ApplyScriptableCamSettings(cauldronCameraPreset, cauldronCameraTransitionTime);
        
        PickRightPotion();
        _currentChallenge = _currentPotion.StirHapticChallenge;
        _currentStirTime = 0;
        _currentStirIndex = 0;
        _currentCheckIndex = 0;
        _isCurrentStirClockwise = Random.Range(0, 2) == 0;
        _joystickInputDifferences.Clear();
        
        stirChallengeGameObject.SetActive(true);
        
        StartPreview();
    }

    private void StartPreview()
    {
        _isInPreview = true;
        clockwiseArrowImage.color = new Color(clockwiseArrowImage.color.r, clockwiseArrowImage.color.g, clockwiseArrowImage.color.b, 0.5f);
        rotationMarkerImage.color = new Color(rotationMarkerImage.color.r, rotationMarkerImage.color.g, rotationMarkerImage.color.b, 0.5f);
        StartStirTurn();
    }

    private void StopPreview()
    {
        _isInPreview = false;
        _isInPreviewPause = false;
        clockwiseArrowImage.color = new Color(clockwiseArrowImage.color.r, clockwiseArrowImage.color.g, clockwiseArrowImage.color.b, 1f);
        rotationMarkerImage.color = new Color(rotationMarkerImage.color.r, rotationMarkerImage.color.g, rotationMarkerImage.color.b, 1f);
        _currentStirTime = 0;
        StartStirTurn();
    }

    private void StartStirTurn()
    {
        clockwiseArrowGameObject.SetActive(true);
        clockwiseArrowGameObject.transform.localScale = new Vector3(_isCurrentStirClockwise ? 1 : -1, 1, 1);
        rotationMarkerGameObject.SetActive(true);
        rotationMarkerGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void UpdateStirChallenge()
    {
        if (_isWaitingForInputReset)
        {
            if (CheckInputReset())
            {
                StopWaitingForInputReset();
            }
            
            return;
        }
        
        if (_isInPreview)
        {
            if (CheckInputPreview())
            {
                StopPreview();
                return;
            }
            
            if (_isInPreviewPause)
            {
                if (_currentStirTime >= stirHapticChallengeGlobalValuesSo.PreviewPauseBetweenTurnsDuration)
                {
                    _isInPreviewPause = false;
                    _currentStirTime = 0;
                    StartStirTurn();
                }
                
                _currentStirTime += Time.deltaTime;
                return;
            }

            if (_currentStirTime >= _currentChallenge.StirDurations[_currentStirIndex])
            {
                _isInPreviewPause = true;
                _currentStirTime = 0;
            }
        }
        else
        {
            if (Mathf.FloorToInt(_currentStirTime / stirHapticChallengeGlobalValuesSo.CheckPositionInterval) > _currentCheckIndex ||
                _currentStirTime >= _currentChallenge.StirDurations[_currentStirIndex])
            {
                _currentCheckIndex = Mathf.FloorToInt(_currentStirTime / stirHapticChallengeGlobalValuesSo.CheckPositionInterval);
                if (!CheckInput()) return;
            }
        
            if (_currentStirTime >= _currentChallenge.StirDurations[_currentStirIndex]) return;
        }
        
        _currentStirTime += Time.deltaTime;
        rotationMarkerGameObject.transform.Rotate(0, 0, (_isCurrentStirClockwise ? -1 : 1) * 360 * Time.deltaTime / _currentChallenge.StirDurations[_currentStirIndex]);
    }

    private void NextStirTurn()
    {
        _currentStirTime = 0;
        _currentStirIndex++;
        _currentCheckIndex = 0;
        _lastJoystickInputValue = Vector2.zero;
            
        if (_currentStirIndex >= _currentChallenge.StirDurations.Length)
        {
            StopStirChallenge();
            return;
        }
        
        _isCurrentStirClockwise = Random.Range(0, 2) == 0;
            
        StartPreview();
    }

    private void StopStirChallenge()
    {
        float averageDifference = 0;
        
        foreach (float difference in _joystickInputDifferences)
        {
            averageDifference += difference;
        }
        
        averageDifference /= _joystickInputDifferences.Count;
        Debug.Log("Average Difference: " + averageDifference);
        
        Debug.Log(_currentPotion.Name + " Stir Challenge Finished");
        
        stirChallengeGameObject.SetActive(false);
        _currentPotion = null;
        _currentChallenge = null;
        
        CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, cauldronCameraTransitionTime);
    }
    
    private void StartWaitingForInputReset()
    {
        _isWaitingForInputReset = true;
        stirChallengeGameObject.SetActive(false);
    }
    
    private void StopWaitingForInputReset()
    {
        _isWaitingForInputReset = false;
        stirChallengeGameObject.SetActive(true);
        
        NextStirTurn();
    }
    
    
    private bool CheckInput()
    {
        if (_lastJoystickInputValue == Vector2.zero && JoystickInputValue == Vector2.zero) return true;
        
        if (JoystickInputValue == Vector2.zero)
        {
            // TODO: Failure
            NextStirTurn();
            return false;
        }
        
        if (_lastJoystickInputValue != Vector2.zero && _lastJoystickInputValue != Vector2.down &&
            JoystickInputValue == Vector2.down)
        {
            StartWaitingForInputReset();
            return false;
        }
        
        _lastJoystickInputValue = JoystickInputValue;
        
        float joystickInputAngle = -Vector2.SignedAngle(Vector2.down, JoystickInputValue);
        
        CurrentCauldron.SpoonTransform.localRotation = Quaternion.Euler(0, joystickInputAngle, 0);
        
        _joystickInputDifferences.Add(Mathf.Abs(rotationMarkerGameObject.transform.localEulerAngles.z -
                                                joystickInputAngle));
        
        return true;
    }
    
    private bool CheckInputPreview()
    {
        return JoystickInputValue != Vector2.zero;
    }
    
    private bool CheckInputReset()
    {
        return JoystickInputValue == Vector2.zero;
    }
}
