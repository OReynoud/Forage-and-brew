using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CompostHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static CompostHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private CompostHapticChallengeGlobalValuesSo compostHapticChallengeGlobalValuesSo;
    [SerializeField] private PotionListSo potionListSo;
    [SerializeField] private CollectedPotionBehaviour collectedPotionPrefab;
    
    [Header("UI")]
    [SerializeField] private GameObject compostChallengeGameObject;
    [SerializeField] private GameObject visualIndicationGameObject;
    [SerializeField] private GameObject clockwiseArrowGameObject;
    [SerializeField] private Image clockwiseArrowImage;
    [SerializeField] private JoystickAnimationManagerBehaviour joystickAnimationManagerBehaviour;
    [SerializeField] private Transform confirmationCircleParentTransform;
    [SerializeField] private ConfirmationCircleBehaviour confirmationCirclePrefab;
    private readonly List<ConfirmationCircleBehaviour> _confirmationCircles = new();
    [SerializeField] private RectTransform obtainedPotionRectTransform;
    [SerializeField] private TMP_Text obtainedPotionNameText;
    [SerializeField] private Image obtainedPotionImage;
    [SerializeField] private Image obtainedPotionLiquidImage;
    
    [Header("Camera")]
    [SerializeField] private float compostCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
    
    [Header("Character")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Vector3 characterCompostPosition;
    [SerializeField] private Vector3 characterCompostRotation;
    
    private float _currentStirTime;
    private int _currentStirIndex;
    private bool _isCurrentStirClockwise;
    private bool _isInPreview;
    private bool _isInPreviewPause;
    private bool _isObtainedPotionAnimationPlaying;
    private float _currentObtainedPotionAnimationTime;
    public CauldronBehaviour CurrentCauldron { get; set; }
    
    // Input
    public Vector2 JoystickInputValue { get; set; }
    private readonly List<Vector2> _storedJoystickInputValues = new();
    
    // Animator Hashes
    private static readonly int IsStirring = Animator.StringToHash("IsStirring");
    private static readonly int PotionSuccess = Animator.StringToHash("PotionSuccess");
    
    public UnityEvent<IngredientValuesSo> OnAddIngredient = new();


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
        if (!_currentChallenge) return;
        
        if (_isObtainedPotionAnimationPlaying)
        {
            UpdateObtainedPotionAnimation();

            return;
        }
        if (_currentStirIndex >= _currentChallenge.StirCamerasAndDurations.Length)
            return;
        
        UpdateStirChallenge();
    }
    
    
    public void StartStirChallenge()
    {
        // Check if player is near a cauldron
        if (!CurrentCauldron) return;
        
        // Camera
        ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = true;
        _previousCameraPreset = SimpleCameraBehavior.instance.TargetCamSettings;

        // Character
        transform.position = CurrentCauldron.transform.position + characterCompostPosition;
        transform.rotation = Quaternion.Euler(characterCompostRotation);
        characterAnimator.SetBool(IsStirring, true);
        
        // Inputs
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnableHapticChallengeJoystickInputs();
        CharacterInputManager.Instance.EnableQuitHapticChallengeInputs();
        
        // Cauldron
        CurrentCauldron.DisableInteract();
        visualIndicationGameObject.SetActive(true);
        
        // Challenge
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = true;
        
        Debug.Log(_currentPotion.Name + " Stir Challenge");
        _currentChallenge = _currentPotion.StirHapticChallenge;
        _currentStirTime = 0;
        _currentStirIndex = 0;
        
        _isCurrentStirClockwise = _currentChallenge.StirCamerasAndDurations[_currentStirIndex].Direction ==
                                  StirDirection.Clockwise;
        
        // UI
        compostChallengeGameObject.SetActive(true);
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
        _confirmationCircles[_currentStirIndex].SetCurrentCircle();
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(_currentChallenge.StirCamerasAndDurations[_currentStirIndex].Camera,
            compostCameraTransitionTime);
        CurrentCauldron.PlayBrewingSound(_currentStirIndex);
        StartStirTurn();
    }

    private void StopPreview()
    {
        _isInPreview = false;
        _isInPreviewPause = false;
        clockwiseArrowImage.color = new Color(clockwiseArrowImage.color.r, clockwiseArrowImage.color.g, clockwiseArrowImage.color.b, 1f);
        _currentStirTime = 0;
        StartStirTurn();
    }

    private void StartStirTurn()
    {
        clockwiseArrowGameObject.SetActive(true);
        clockwiseArrowGameObject.transform.localScale = new Vector3(_isCurrentStirClockwise ? 1 : -1, 1, 1);
        
        if (_isCurrentStirClockwise)
        {
            joystickAnimationManagerBehaviour.PlayClockwiseAnimation();
        }
        else
        {
            joystickAnimationManagerBehaviour.PlayCounterClockwiseAnimation();
        }
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
                if (_currentStirTime >= compostHapticChallengeGlobalValuesSo.PreviewPauseBetweenTurnsDuration)
                {
                    _isInPreviewPause = false;
                    _currentStirTime = 0;
                    StartStirTurn();
                }
                
                _currentStirTime += Time.deltaTime;
                return;
            }

            if (_currentStirTime >= joystickAnimationManagerBehaviour.AnimationDuration)
            {
                _isInPreviewPause = true;
                joystickAnimationManagerBehaviour.StopAnimation();
                joystickAnimationManagerBehaviour.ResetClockwiseAnimation();
                _currentStirTime = 0;
            }
        }
        else
        {
            if (!CheckInput()) return;
        
            if (_currentStirTime >= joystickAnimationManagerBehaviour.AnimationDuration) return;
        }
        
        _currentStirTime += Time.deltaTime;
    }

    private void NextStirTurn()
    {
        _confirmationCircles[_currentStirIndex].SetRightCircle();
        
        RumbleManager.Instance.PlayRumble(compostHapticChallengeGlobalValuesSo.StirTurnVibrationDuration,
            compostHapticChallengeGlobalValuesSo.StirTurnVibrationPower);
        CurrentCauldron.PlayCheckInputSound(_currentStirIndex);
        
        _currentStirTime = 0;
        _currentStirIndex++;
            
        if (_currentStirIndex >= _currentChallenge.StirCamerasAndDurations.Length)
        {
            ObtainPotion(false);
            return;
        }
        
        _isCurrentStirClockwise = _currentChallenge.StirCamerasAndDurations[_currentStirIndex].Direction ==
                                  StirDirection.Clockwise;
            
        StartPreview();
    }

    public void StopStirChallenge(bool isSuccessful = true)
    {
        if (!_currentChallenge) return;
        
        if (_isObtainedPotionAnimationPlaying) return;
        
        if (isSuccessful)
        {
            CollectedPotionBehaviour collectedPotionBehaviour = Instantiate(collectedPotionPrefab,
                CurrentCauldron.transform.position, Quaternion.identity);
            collectedPotionBehaviour.PotionValuesSo = _currentPotion;
            CharacterInteractController.Instance.AddToPile(collectedPotionBehaviour);
            GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Add(collectedPotionBehaviour);
            GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients.Clear();
        }
        
        foreach (ConfirmationCircleBehaviour confirmationCircle in _confirmationCircles)
        {
            Destroy(confirmationCircle.gameObject);
        }
        _confirmationCircles.Clear();
        
        compostChallengeGameObject.SetActive(false);
        _currentPotion = null;
        _currentChallenge = null;
        
        // Sound
        CurrentCauldron.StopBrewingSound();
        characterAnimator.SetBool(IsStirring, false);
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(_previousCameraPreset, compostCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        CurrentCauldron.EnableInteract(false);
        
        if (isSuccessful)
        {
            CurrentCauldron.LockCauldron();
        }
        
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = false;
        
        DOTween.To(() => transform.position, x => transform.position = x, transform.position,
            compostCameraTransitionTime).OnComplete(
            () =>
            {
                ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = false;
            });
    }
    
    
    private bool CheckInput()
    {
        if (JoystickInputValue == Vector2.zero)
        {
            _storedJoystickInputValues.Clear();
            return true;
        }

        Quaternion baseRotation = Quaternion.identity;

        if (_storedJoystickInputValues.Count > 0)
        {
            baseRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.down, _storedJoystickInputValues[0]));
        }
        
        if (_storedJoystickInputValues.Count == 4)
        {
            if (Vector2.Angle(JoystickInputValue.normalized, baseRotation * Vector2.down) <=
                compostHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
            {
                NextStirTurn();
                CurrentCauldron.SpoonTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                return false;
            }

            if (Vector2.Angle(JoystickInputValue.normalized, baseRotation * Vector2.up) <=
                compostHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
            {
                _storedJoystickInputValues.Clear();
                return true;
            }
        }

        if (_storedJoystickInputValues.Count == 3)
        {
            if (Vector2.Angle(JoystickInputValue.normalized, baseRotation * (_isCurrentStirClockwise ?
                    Vector2.right : Vector2.left)) <= compostHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
            {
                _storedJoystickInputValues.Add(_isCurrentStirClockwise ? Vector2.right : Vector2.left);
            }
            else if (Vector2.Angle(JoystickInputValue.normalized, baseRotation * (_isCurrentStirClockwise ?
                         Vector2.left : Vector2.right)) <= compostHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
            {
                _storedJoystickInputValues.Clear();
                return true;
            }
        }

        if (_storedJoystickInputValues.Count == 2)
        {
            if (Vector2.Angle(JoystickInputValue.normalized, baseRotation * Vector2.up) <=
                compostHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
            {
                _storedJoystickInputValues.Add(Vector2.up);
            }
            else if (Vector2.Angle(JoystickInputValue.normalized, baseRotation * Vector2.down) <=
                     compostHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
            {
                _storedJoystickInputValues.Clear();
                return true;
            }
        }

        if (_storedJoystickInputValues.Count == 1)
        {
            if (Vector2.Angle(JoystickInputValue.normalized, baseRotation * (_isCurrentStirClockwise ?
                    Vector2.left : Vector2.right)) <= compostHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
            {
                _storedJoystickInputValues.Add(_isCurrentStirClockwise ? Vector2.left : Vector2.right);
            }
            else if (Vector2.Angle(JoystickInputValue.normalized, baseRotation * (_isCurrentStirClockwise ? 
                         Vector2.right : Vector2.left)) <= compostHapticChallengeGlobalValuesSo.AngleToleranceForTurnEnd)
            {
                _storedJoystickInputValues.Clear();
                return true;
            }
        }
        
        if (_storedJoystickInputValues.Count == 0)
        {
            _storedJoystickInputValues.Add(JoystickInputValue.normalized);
        }
        
        float joystickInputAngle = -Vector2.SignedAngle(Vector2.down, JoystickInputValue.normalized);
        
        CurrentCauldron.SpoonTransform.localRotation = Quaternion.Euler(0, joystickInputAngle, 0);
        
        return true;
    }
    
    private bool CheckInputPreview()
    {
        if (JoystickInputValue == Vector2.zero)
        {
            _storedJoystickInputValues.Clear();
            return false;
        }
        
        if (_storedJoystickInputValues.Count == 0)
        {
            _storedJoystickInputValues.Add(JoystickInputValue.normalized);
            return false;
        }
        
        return Vector2.Angle(JoystickInputValue.normalized, _storedJoystickInputValues[0]) >
               compostHapticChallengeGlobalValuesSo.AngleToleranceForPreviewEnd;
    }


    #region Obtained Potion

    private void UpdateObtainedPotionAnimation()
    {
        _currentObtainedPotionAnimationTime += Time.deltaTime;
        
        if (_currentObtainedPotionAnimationTime <= compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration)
        {
            obtainedPotionRectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one,
                compostHapticChallengeGlobalValuesSo.ObtainedPotionScaleAnimationCurve.Evaluate(
                    _currentObtainedPotionAnimationTime /
                    compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration));
            
            obtainedPotionRectTransform.anchoredPosition = Vector3.Lerp(
                compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationStartPosition,
                compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationEndPosition,
                compostHapticChallengeGlobalValuesSo.ObtainedPotionPositionAnimationCurve.Evaluate(
                    _currentObtainedPotionAnimationTime /
                    compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration));
        }
        else if (_currentObtainedPotionAnimationTime <= compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration +
                 compostHapticChallengeGlobalValuesSo.ObtainedPotionStayDuration)
        {
            obtainedPotionRectTransform.localScale = Vector3.one;
            obtainedPotionRectTransform.anchoredPosition =
                compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationEndPosition;
        }
        else
        {
            obtainedPotionRectTransform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.zero,
                compostHapticChallengeGlobalValuesSo.ObtainedPotionScaleEndAnimationCurve.Evaluate(
                    (_currentObtainedPotionAnimationTime - compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration
                                                         - compostHapticChallengeGlobalValuesSo.ObtainedPotionStayDuration) /
                    compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationEndDuration));
        }
            
        if (_currentObtainedPotionAnimationTime >= compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationDuration +
            compostHapticChallengeGlobalValuesSo.ObtainedPotionStayDuration +
            compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationEndDuration)
        {
            _isObtainedPotionAnimationPlaying = false;
            StopStirChallenge();
        }
    }

    public void ObtainPotion(bool doCanvasAnim)
    {
        if (!doCanvasAnim)
        {
            foreach (ConfirmationCircleBehaviour confirmationCircle in _confirmationCircles)
            {
                Destroy(confirmationCircle.gameObject);
            }
            _confirmationCircles.Clear();
            visualIndicationGameObject.SetActive(false);
            
            CurrentCauldron.PlayCheckInputFinalSound();
            characterAnimator.SetBool(IsStirring,false);
            if (_currentPotion == potionListSo.DefaultPotion)
            {
                characterAnimator.SetTrigger(PotionFail);
            }
            else
            {
                characterAnimator.SetTrigger(PotionSuccess);
            }
            return;
        }

        
        CauldronVfxManager.Instance.PlayObtainedPotionVfx();
        obtainedPotionImage.sprite = _currentPotion.PotionDifficulty.PotionSprite;
        obtainedPotionLiquidImage.sprite = _currentPotion.PotionDifficulty.LiquidSprite;
        obtainedPotionLiquidImage.color = _currentPotion.SpriteLiquidColor;
        obtainedPotionNameText.text = _currentPotion.Name;
        obtainedPotionRectTransform.localScale = Vector3.zero;
        obtainedPotionRectTransform.anchoredPosition =
            compostHapticChallengeGlobalValuesSo.ObtainedPotionAnimationStartPosition;
        obtainedPotionRectTransform.gameObject.SetActive(true);
        _isObtainedPotionAnimationPlaying = true;
        _currentObtainedPotionAnimationTime = 0f;
    }

    #endregion
}
