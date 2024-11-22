using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CollectHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static CollectHapticChallengeManager Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private CollectHapticChallengeListSo collectHapticChallengeListSo;

    [Header("Gauge Haptic Challenge UI")]
    [SerializeField] private GameObject gaugeHapticChallengeGameObject;
    [SerializeField] private RectTransform gaugeRectTransform;
    [SerializeField] private RectTransform wrongGaugeRectTransform;
    [SerializeField] private RectTransform correctGaugeRectTransform;
    [SerializeField] private RectTransform perfectGaugeRectTransform;
    [SerializeField] private RectTransform gaugeArrowRectTransform;

    [Header("Unearthing Haptic Challenge UI")]
    [SerializeField] private GameObject unearthingHapticChallengeGameObject;
    [SerializeField] private List<HapticChallengeMovementDirectionRectTransform> unearthingHapticChallengeRectTransforms;
    [SerializeField] private Image unearthingHapticChallengeGaugeImage;
    
    // Global variables
    private bool _isCollectHapticChallengeActive;
    private IngredientToCollectBehaviour _currentIngredientToCollectBehaviour;
    public Vector2 JoystickInputValue { get; set; }
    private Vector2 _lastJoystickInputValue;
    
    // Gauge Haptic Challenge
    private GaugeHapticChallengeSo _currentGaugeHapticChallengeSo;
    private bool _isGaugeHapticChallengeActive;
    private bool _isGaugeHapticChallengeGoingUp;
    
    // Unearthing Haptic Challenge
    private UnearthingHapticChallengeSo _currentUnearthingHapticChallengeSo;
    private bool _isUnearthingHapticChallengeActive;
    private int _currentUnearthingHapticChallengeIndex;
    private float _currentUnearthingHapticChallengeTime;
    private bool _hasMovementDirectionBeenTriggeredOnce;
    
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        gaugeHapticChallengeGameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isGaugeHapticChallengeActive)
        {
            UpdateGaugeHapticChallenge();
        }
        
        if (_isUnearthingHapticChallengeActive)
        {
            UpdateUnearthingHapticChallenge();
        }
    }


    public void StartCollectHapticChallenge(IngredientToCollectBehaviour ingredientToCollectBehaviour)
    {
        _currentIngredientToCollectBehaviour = ingredientToCollectBehaviour;
        
        foreach (var ingredientTypeHapticChallenge in collectHapticChallengeListSo.HapticChallengesByIngredientType)
        {
            if (ingredientTypeHapticChallenge.IngredientType == _currentIngredientToCollectBehaviour.IngredientValuesSo.Type)
            {
                if (ingredientTypeHapticChallenge.CollectHapticChallengeSo is GaugeHapticChallengeSo gaugeHapticChallengeSo)
                {
                    _currentGaugeHapticChallengeSo = gaugeHapticChallengeSo;
                    StartGaugeHapticChallenge();
                }
                
                if (ingredientTypeHapticChallenge.CollectHapticChallengeSo is UnearthingHapticChallengeSo unearthingHapticChallengeSo)
                {
                    _currentUnearthingHapticChallengeSo = unearthingHapticChallengeSo;
                    StartUnearthingHapticChallenge();
                }
                
                return;
            }
        }
    }

    private void StopCollectHapticChallenge()
    {
        _isCollectHapticChallengeActive = false;
        _currentIngredientToCollectBehaviour.Collect();
        _currentIngredientToCollectBehaviour = null;
        CharacterInputManager.Instance.EnableMoveInputs();
    }

    
    private void StartGaugeHapticChallenge()
    {
        gaugeHapticChallengeGameObject.SetActive(true);
        gaugeArrowRectTransform.anchoredPosition = new Vector2(gaugeArrowRectTransform.anchoredPosition.x,
            Random.Range(_currentGaugeHapticChallengeSo.GaugeTotalHeight * -0.5f,
                _currentGaugeHapticChallengeSo.GaugeTotalHeight * 0.5f));
        _isGaugeHapticChallengeGoingUp = Random.Range(0, 2) == 0;
        _isGaugeHapticChallengeActive = true;
        _isCollectHapticChallengeActive = true;
    }

    public void StopGaugeHapticChallenge()
    {
        if (!_isGaugeHapticChallengeActive) return;
        
        gaugeHapticChallengeGameObject.SetActive(false);
        _isGaugeHapticChallengeActive = false;
        StopCollectHapticChallenge();
    }
    
    private void UpdateGaugeHapticChallenge()
    {
        if (_isGaugeHapticChallengeGoingUp)
        {
            gaugeArrowRectTransform.anchoredPosition += new Vector2(0f, _currentGaugeHapticChallengeSo.ArrowSpeed * Time.deltaTime);
            if (gaugeArrowRectTransform.anchoredPosition.y >= gaugeRectTransform.sizeDelta.y * 0.5f)
            {
                _isGaugeHapticChallengeGoingUp = false;
            }
        }
        else
        {
            gaugeArrowRectTransform.anchoredPosition -= new Vector2(0f, _currentGaugeHapticChallengeSo.ArrowSpeed * Time.deltaTime);
            if (gaugeArrowRectTransform.anchoredPosition.y <= gaugeRectTransform.sizeDelta.y * -0.5f)
            {
                _isGaugeHapticChallengeGoingUp = true;
            }
        }
    }
    
    
    private void StartUnearthingHapticChallenge()
    {
        unearthingHapticChallengeGameObject.SetActive(true);
        _isUnearthingHapticChallengeActive = true;
        _isCollectHapticChallengeActive = true;
        _currentUnearthingHapticChallengeTime = 0f;
        _hasMovementDirectionBeenTriggeredOnce = false;

        foreach (HapticChallengeMovementDirectionRectTransform unearthingHapticChallengeRectTransform in unearthingHapticChallengeRectTransforms)
        {
            unearthingHapticChallengeRectTransform.RectTransform.gameObject.SetActive(false);
        }
        
        PickRandomMovementDirectionUnearthingChallenge(true);
        unearthingHapticChallengeRectTransforms[_currentUnearthingHapticChallengeIndex].RectTransform.gameObject.SetActive(true);
    }
    
    public void StopUnearthingHapticChallenge()
    {
        if (!_isUnearthingHapticChallengeActive) return;
        
        unearthingHapticChallengeGameObject.SetActive(false);
        _isUnearthingHapticChallengeActive = false;
        StopCollectHapticChallenge();
    }
    
    private void UpdateUnearthingHapticChallenge()
    {
        if (CheckInputUnearthingChallenge()) return;
        
        _currentUnearthingHapticChallengeTime += Time.deltaTime;
        
        if (_currentUnearthingHapticChallengeTime >= _currentUnearthingHapticChallengeSo.MovementDirectionDuration)
        {
            ChangeMovementDirectionUnearthingChallenge();
            _currentUnearthingHapticChallengeTime = 0f;
        }
    }
    
    private void PickRandomMovementDirectionUnearthingChallenge(bool isFirstTime = false)
    {
        int previousIndex = _currentUnearthingHapticChallengeIndex;
        
        _currentUnearthingHapticChallengeIndex = Random.Range(0, unearthingHapticChallengeRectTransforms.Count -
                                                                 (isFirstTime ? 0 : 1));
        
        if (!isFirstTime && _currentUnearthingHapticChallengeIndex >= previousIndex)
        {
            _currentUnearthingHapticChallengeIndex++;
        }
    }
    
    private void ChangeMovementDirectionUnearthingChallenge()
    {
        _hasMovementDirectionBeenTriggeredOnce = false;
        unearthingHapticChallengeRectTransforms[_currentUnearthingHapticChallengeIndex].RectTransform.gameObject.SetActive(false);
        PickRandomMovementDirectionUnearthingChallenge();
        unearthingHapticChallengeRectTransforms[_currentUnearthingHapticChallengeIndex].RectTransform.gameObject.SetActive(true);
    }
    
    private bool CheckInputUnearthingChallenge()
    {
        switch (unearthingHapticChallengeRectTransforms[_currentUnearthingHapticChallengeIndex].HapticChallengeMovementDirection)
        {
            case HapticChallengeMovementDirection.LeftRight:
                ProcessInputUnearthingChallenge(Vector2.left, Vector2.right, _currentUnearthingHapticChallengeSo.CardinalMovementDirectionTolerance);
                break;
            case HapticChallengeMovementDirection.UpDown:
                ProcessInputUnearthingChallenge(Vector2.up, Vector2.down, _currentUnearthingHapticChallengeSo.CardinalMovementDirectionTolerance);
                break;
            case HapticChallengeMovementDirection.UpLeftDownRight:
                ProcessInputUnearthingChallenge((Vector2.up + Vector2.left).normalized, (Vector2.down + Vector2.right).normalized,
                    _currentUnearthingHapticChallengeSo.DiagonalMovementDirectionTolerance);
                break;
            case HapticChallengeMovementDirection.UpRightDownLeft:
                ProcessInputUnearthingChallenge((Vector2.up + Vector2.right).normalized, (Vector2.down + Vector2.left).normalized,
                    _currentUnearthingHapticChallengeSo.DiagonalMovementDirectionTolerance);
                break;
        }

        if (unearthingHapticChallengeGaugeImage.fillAmount >= 1f)
        {
            StopUnearthingHapticChallenge();
            return true;
        }
        
        return false;
    }

    private void ProcessInputUnearthingChallenge(Vector2 firstDirection, Vector2 secondDirection, float tolerance)
    {
        if (!_hasMovementDirectionBeenTriggeredOnce)
        {
            if (JoystickInputValue.x >= firstDirection.x - tolerance && JoystickInputValue.x <= firstDirection.x + tolerance &&
                JoystickInputValue.y >= firstDirection.y - tolerance && JoystickInputValue.y <= firstDirection.y + tolerance)
            {
                _hasMovementDirectionBeenTriggeredOnce = true;
                IncreaseGaugeUnearthingChallenge();
                _lastJoystickInputValue = firstDirection;
                return;
            }

            if (JoystickInputValue.x >= secondDirection.x - tolerance && JoystickInputValue.x <= secondDirection.x + tolerance &&
                JoystickInputValue.y >= secondDirection.y - tolerance && JoystickInputValue.y <= secondDirection.y + tolerance)
            {
                _hasMovementDirectionBeenTriggeredOnce = true;
                IncreaseGaugeUnearthingChallenge();
                _lastJoystickInputValue = secondDirection;
            }
        }
        else
        {
            if (_lastJoystickInputValue == firstDirection)
            {
                if (JoystickInputValue.x >= secondDirection.x - tolerance && JoystickInputValue.x <= secondDirection.x + tolerance &&
                    JoystickInputValue.y >= secondDirection.y - tolerance && JoystickInputValue.y <= secondDirection.y + tolerance)
                {
                    IncreaseGaugeUnearthingChallenge();
                    _lastJoystickInputValue = secondDirection;
                }
            }
            else
            {
                if (JoystickInputValue.x >= firstDirection.x - tolerance && JoystickInputValue.x <= firstDirection.x + tolerance &&
                    JoystickInputValue.y >= firstDirection.y - tolerance && JoystickInputValue.y <= firstDirection.y + tolerance)
                {
                    IncreaseGaugeUnearthingChallenge();
                    _lastJoystickInputValue = firstDirection;
                }
            }
        }
    }

    private void IncreaseGaugeUnearthingChallenge()
    {
        unearthingHapticChallengeGaugeImage.fillAmount += _currentUnearthingHapticChallengeSo.GaugeIncreasePart;
    }


    private void OnDrawGizmos()
    {
        if (collectHapticChallengeListSo)
        {
            foreach (IngredientTypeHapticChallenge ingredientTypeHapticChallenge in collectHapticChallengeListSo.
                         HapticChallengesByIngredientType)
            {
                if (ingredientTypeHapticChallenge.CollectHapticChallengeSo is GaugeHapticChallengeSo gaugeHapticChallengeSo)
                {
                    gaugeRectTransform.sizeDelta = new Vector2(wrongGaugeRectTransform.sizeDelta.x,
                        gaugeHapticChallengeSo.GaugeTotalHeight);
                    wrongGaugeRectTransform.sizeDelta = new Vector2(wrongGaugeRectTransform.sizeDelta.x,
                        gaugeHapticChallengeSo.GaugeTotalHeight);
                    correctGaugeRectTransform.sizeDelta = new Vector2(correctGaugeRectTransform.sizeDelta.x,
                        gaugeHapticChallengeSo.GaugeTotalHeight * gaugeHapticChallengeSo.CorrectGaugePart);
                    perfectGaugeRectTransform.sizeDelta = new Vector2(perfectGaugeRectTransform.sizeDelta.x,
                        gaugeHapticChallengeSo.GaugeTotalHeight * gaugeHapticChallengeSo.PerfectGaugePart);
                }
            }
        }
    }
}
