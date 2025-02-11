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
    [SerializeField] private CollectedPotionBehaviour collectedPotionPrefab;
    
    [Header("UI")]
    [SerializeField] private GameObject stirChallengeGameObject;
    [SerializeField] private GameObject visualIndicationGameObject;
    [SerializeField] private GameObject clockwiseArrowGameObject;
    [SerializeField] private Image clockwiseArrowImage;
    [SerializeField] private GameObject rotationMarkerGameObject;
    [SerializeField] private Image rotationMarkerImage;
    [SerializeField] private Transform confirmationCircleParentTransform;
    [SerializeField] private ConfirmationCircleBehaviour confirmationCirclePrefab;
    private readonly List<ConfirmationCircleBehaviour> _confirmationCircles = new();
    [SerializeField] private RectTransform obtainedPotionRectTransform;
    [SerializeField] private Image obtainedPotionImage;
    
    [Header("Camera")]
    [SerializeField] private float cauldronCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
    
    [Header("Character")]
    [SerializeField] private Vector3 characterStirPosition;
    [SerializeField] private Vector3 characterStirRotation;
    
    private PotionValuesSo _currentPotion;
    private StirHapticChallengeSo _currentChallenge;
    private float _currentStirTime;
    private int _currentStirIndex;
    private int _currentCheckIndex;
    private bool _isCurrentStirClockwise;
    private bool _isInPreview;
    private bool _isInPreviewPause;
    private bool _isObtainedPotionAnimationPlaying;
    private float _currentObtainedPotionAnimationTime;
    public CauldronBehaviour CurrentCauldron { get; set; }
    
    // Input
    public Vector2 JoystickInputValue { get; set; }
    private readonly List<Vector2> _storedJoystickInputValues = new();
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
        
        if (_isObtainedPotionAnimationPlaying)
        {
            UpdateObtainedPotionAnimation();

            return;
        }
        
        UpdateStirChallenge();
    }
    
    
    private void PickRightPotion()
    {
        List<TemperatureChallengeIngredients> temperatureAndIngredients = CurrentCauldron.ClearIngredients();
        PinnedRecipe.instance.UpdateRecipeStepsCounter();
        
        foreach (PotionValuesSo potion in potionListSo.Potions) // For each potion
        {
            if (potion.TemperatureChallengeIngredients.Length != temperatureAndIngredients.Count) continue;
            
            bool isRightPotion = true;

            for (int i = 0; i < potion.TemperatureChallengeIngredients.Length; i++)
            {
                List<CookedIngredientForm> potionIngredients = new();
                List<CookedIngredientForm> cauldronIngredients = new();

                // Store all ingredients in the cauldron
                foreach (CookedIngredientForm cookedIngredientForm in temperatureAndIngredients[i].CookedIngredients)
                {
                    cauldronIngredients.Add(cookedIngredientForm);
                }

                // Store all ingredients and ingredient types in the potion step
                foreach (CookedIngredientForm cookedIngredientForm in potion.TemperatureChallengeIngredients[i].CookedIngredients)
                {
                    potionIngredients.Add(cookedIngredientForm);
                }

                // Check if the cauldron has the right ingredient count
                if (cauldronIngredients.Count != potionIngredients.Count)
                {
                    isRightPotion = false;
                    break;
                }

                // Check if the cauldron has the right ingredients
                foreach (CookedIngredientForm cookedIngredientForm in potionIngredients)
                {
                    if (cookedIngredientForm.IsAType) continue;
                    
                    if (!cauldronIngredients.Exists(ingredientForm =>
                            ingredientForm.Ingredient == cookedIngredientForm.Ingredient &&
                            ingredientForm.CookedForm == cookedIngredientForm.CookedForm))
                    {
                        isRightPotion = false;
                        break;
                    }
                    
                    cauldronIngredients.Remove(cauldronIngredients.Find(ingredientForm =>
                        ingredientForm.Ingredient == cookedIngredientForm.Ingredient &&
                        ingredientForm.CookedForm == cookedIngredientForm.CookedForm));
                }

                if (!isRightPotion) break;

                foreach (CookedIngredientForm cookedIngredientForm in potionIngredients)
                {
                    if (!cookedIngredientForm.IsAType) continue;
                    
                    IngredientTypeSo ingredientType = cookedIngredientForm.IngredientType;
                    
                    if (!cauldronIngredients.Exists(ingredient => ingredient.Ingredient.Type == ingredientType &&
                                                                  ingredient.CookedForm == cookedIngredientForm.CookedForm))
                    {
                        isRightPotion = false;
                        break;
                    }

                    cauldronIngredients.Remove(cauldronIngredients.Find(ingredient =>
                        ingredient.Ingredient.Type == ingredientType &&
                        ingredient.CookedForm == cookedIngredientForm.CookedForm));
                }

                if (!isRightPotion) break;
                
                // Check if the cauldron has the right temperature
                if (potion.TemperatureChallengeIngredients[i].Temperature != temperatureAndIngredients[i].Temperature)
                {
                    isRightPotion = false;
                    break;
                }
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

        transform.position = CurrentCauldron.transform.position + characterStirPosition;
        transform.rotation = Quaternion.Euler(characterStirRotation);
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnableHapticChallengeJoystickInputs();
                
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = true;
        
        CurrentCauldron.DisableInteract();
        
        PickRightPotion();
        Debug.Log(_currentPotion.Name + " Stir Challenge");
        _currentChallenge = _currentPotion.StirHapticChallenge;
        _currentStirTime = 0;
        _currentStirIndex = 0;
        _currentCheckIndex = 0;
        _isCurrentStirClockwise = Random.Range(0, 2) == 0;
        _joystickInputDifferences.Clear();
        
        stirChallengeGameObject.SetActive(true);
        obtainedPotionRectTransform.gameObject.SetActive(false);

        foreach (StirCameraAndDuration _ in _currentChallenge.StirCamerasAndDurations)
        {
            _confirmationCircles.Add(Instantiate(confirmationCirclePrefab, confirmationCircleParentTransform));
        }
        
        StartPreview();
    }

    private void StartPreview()
    {
        _isInPreview = true;
        _storedJoystickInputValues.Clear();
        clockwiseArrowImage.color = new Color(clockwiseArrowImage.color.r, clockwiseArrowImage.color.g, clockwiseArrowImage.color.b, 0.5f);
        rotationMarkerImage.color = new Color(rotationMarkerImage.color.r, rotationMarkerImage.color.g, rotationMarkerImage.color.b, 0.5f);
        _confirmationCircles[_currentStirIndex].SetCurrentCircle();
        CameraController.instance.ApplyScriptableCamSettings(_currentChallenge.StirCamerasAndDurations[_currentStirIndex].Camera,
            cauldronCameraTransitionTime);
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

            if (_currentStirTime >= _currentChallenge.StirCamerasAndDurations[_currentStirIndex].Duration)
            {
                _isInPreviewPause = true;
                _currentStirTime = 0;
            }
        }
        else
        {
            if (Mathf.FloorToInt(_currentStirTime / stirHapticChallengeGlobalValuesSo.CheckPositionInterval) > _currentCheckIndex ||
                _currentStirTime >= _currentChallenge.StirCamerasAndDurations[_currentStirIndex].Duration)
            {
                _currentCheckIndex = Mathf.FloorToInt(_currentStirTime / stirHapticChallengeGlobalValuesSo.CheckPositionInterval);
                if (!CheckInput()) return;
            }
        
            if (_currentStirTime >= _currentChallenge.StirCamerasAndDurations[_currentStirIndex].Duration) return;
        }
        
        _currentStirTime += Time.deltaTime;
        rotationMarkerGameObject.transform.Rotate(0, 0, (_isCurrentStirClockwise ? -1 : 1) * 360 * Time.deltaTime /
                                                        _currentChallenge.StirCamerasAndDurations[_currentStirIndex].Duration);
    }

    private void UpdateObtainedPotionAnimation()
    {
        _currentObtainedPotionAnimationTime += Time.deltaTime;
        
        if (_currentObtainedPotionAnimationTime <= stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration)
        {
            obtainedPotionRectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one,
                stirHapticChallengeGlobalValuesSo.ObtainedPotionScaleAnimationCurve.Evaluate(
                    _currentObtainedPotionAnimationTime /
                    stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration));
            
            obtainedPotionRectTransform.anchoredPosition = Vector3.Lerp(
                stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationStartPosition,
                stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationEndPosition,
                stirHapticChallengeGlobalValuesSo.ObtainedPotionPositionAnimationCurve.Evaluate(
                    _currentObtainedPotionAnimationTime /
                    stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration));
        }
        else if (_currentObtainedPotionAnimationTime <= stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration +
                 stirHapticChallengeGlobalValuesSo.ObtainedPotionStayDuration)
        {
            obtainedPotionRectTransform.localScale = Vector3.one;
            obtainedPotionRectTransform.anchoredPosition =
                stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationEndPosition;
        }
        else
        {
            obtainedPotionRectTransform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.zero,
                stirHapticChallengeGlobalValuesSo.ObtainedPotionScaleEndAnimationCurve.Evaluate(
                    (_currentObtainedPotionAnimationTime - stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration
                    - stirHapticChallengeGlobalValuesSo.ObtainedPotionStayDuration) /
                    stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationEndDuration));
        }
            
        if (_currentObtainedPotionAnimationTime >= stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration +
            stirHapticChallengeGlobalValuesSo.ObtainedPotionStayDuration +
            stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationEndDuration)
        {
            _isObtainedPotionAnimationPlaying = false;
            StopStirChallenge();
        }
    }

    private void NextStirTurn()
    {
        _confirmationCircles[_currentStirIndex].SetRightCircle();
        
        RumbleManager.Instance.PlayRumble(stirHapticChallengeGlobalValuesSo.StirTurnVibrationDuration,
            stirHapticChallengeGlobalValuesSo.StirTurnVibrationPower);
        
        _currentStirTime = 0;
        _currentStirIndex++;
        _currentCheckIndex = 0;
            
        if (_currentStirIndex >= _currentChallenge.StirCamerasAndDurations.Length)
        {
            ObtainPotion();
            return;
        }
        
        _isCurrentStirClockwise = Random.Range(0, 2) == 0;
            
        StartPreview();
    }

    private void ObtainPotion()
    {
        CauldronVfxManager.Instance.PlayObtainedPotionVfx();
        obtainedPotionImage.sprite = _currentPotion.icon;
        obtainedPotionRectTransform.localScale = Vector3.zero;
        obtainedPotionRectTransform.anchoredPosition =
            stirHapticChallengeGlobalValuesSo.ObtainedPotionAnimationStartPosition;
        obtainedPotionRectTransform.gameObject.SetActive(true);
        _isObtainedPotionAnimationPlaying = true;
        _currentObtainedPotionAnimationTime = 0f;
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

        CollectedPotionBehaviour collectedPotionBehaviour = Instantiate(collectedPotionPrefab,
            CurrentCauldron.transform.position, Quaternion.identity);
        collectedPotionBehaviour.PotionValuesSo = _currentPotion;
        CharacterInteractController.Instance.AddToPile(collectedPotionBehaviour);
        GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Add(collectedPotionBehaviour);

        foreach (ConfirmationCircleBehaviour confirmationCircle in _confirmationCircles)
        {
            Destroy(confirmationCircle.gameObject);
        }
        _confirmationCircles.Clear();
        stirChallengeGameObject.SetActive(false);
        _currentPotion = null;
        _currentChallenge = null;
        
        CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, cauldronCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        CurrentCauldron.EnableInteract(false);        
        
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = false;
    }
    
    
    private bool CheckInput()
    {
        if (_storedJoystickInputValues.Count == 0 && JoystickInputValue == Vector2.zero) return true;
        
        if (JoystickInputValue == Vector2.zero)
        {
            // TODO: Failure
            NextStirTurn();
            return false;
        }
        
        if (_storedJoystickInputValues.Count == 3 &&
            Vector2.Angle(JoystickInputValue.normalized, Vector2.down) <=
            stirHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
        {
            NextStirTurn();
            return false;
        }

        if (_storedJoystickInputValues.Count == 2 &&
            Vector2.Angle(JoystickInputValue.normalized, _isCurrentStirClockwise ? Vector2.right : Vector2.left) <=
            stirHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
        {
            _storedJoystickInputValues.Add(_isCurrentStirClockwise ? Vector2.right : Vector2.left);
        }

        if (_storedJoystickInputValues.Count == 1 &&
            Vector2.Angle(JoystickInputValue.normalized, Vector2.up) <=
            stirHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
        {
            _storedJoystickInputValues.Add(Vector2.up);
        }

        if (_storedJoystickInputValues.Count == 0 &&
            Vector2.Angle(JoystickInputValue.normalized, _isCurrentStirClockwise ? Vector2.left : Vector2.right) <=
            stirHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
        {
            _storedJoystickInputValues.Add(_isCurrentStirClockwise ? Vector2.left : Vector2.right);
        }
        
        float joystickInputAngle = -Vector2.SignedAngle(Vector2.down, JoystickInputValue.normalized);
        
        CurrentCauldron.SpoonTransform.localRotation = Quaternion.Euler(0, joystickInputAngle, 0);
        
        _joystickInputDifferences.Add(Mathf.Abs(rotationMarkerGameObject.transform.localEulerAngles.z -
                                                joystickInputAngle));
        
        return true;
    }
    
    private bool CheckInputPreview()
    {
        return Vector2.Angle(JoystickInputValue.normalized, Vector2.down) >
               stirHapticChallengeGlobalValuesSo.AngleToleranceForPreviewEnd;
    }
}
