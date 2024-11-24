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

    [Header("Scything Haptic Challenge UI")]
    [SerializeField] private GameObject scythingHapticChallengeGameObject;
    [SerializeField] private RectTransform scythingHapticChallengeGaugeRectTransform;
    [SerializeField] private RectTransform scythingHapticChallengeCutLineRectTransform;
    [SerializeField] private RectTransform scythingHapticChallengeGaugeArrowRectTransform;

    [Header("Unearthing Haptic Challenge UI")]
    [SerializeField] private GameObject unearthingHapticChallengeGameObject;
    [SerializeField] private List<HapticChallengeMovementDirectionRectTransform> unearthingHapticChallengeRectTransforms;
    [SerializeField] private Image unearthingHapticChallengeGaugeImage;
    
    // Global variables
    private bool _isCollectHapticChallengeActive;
    private IngredientToCollectBehaviour _currentIngredientToCollectBehaviour;
    public Vector2 JoystickInputValue { get; set; }
    private Vector2 _lastJoystickInputValue;
    
    // Scything Haptic Challenge
    private ScythingHapticChallengeSo _currentScythingHapticChallengeSo;
    private bool _isScythingHapticChallengeActive;
    private bool _isScythingHapticChallengeGoingUp;
    private List<HapticChallengeGaugeParts> _currentScythingHapticChallengeGaugeParts;
    private int _currentScythingHapticChallengeTurn;
    
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
        scythingHapticChallengeGameObject.SetActive(false);
        unearthingHapticChallengeGameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isScythingHapticChallengeActive)
        {
            UpdateScythingHapticChallenge();
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
                if (ingredientTypeHapticChallenge.CollectHapticChallengeSo is ScythingHapticChallengeSo scythingHapticChallengeSo)
                {
                    _currentScythingHapticChallengeSo = scythingHapticChallengeSo;
                    StartScythingHapticChallenge();
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

    
    private void StartScythingHapticChallenge()
    {
        scythingHapticChallengeGameObject.SetActive(true);
        _currentScythingHapticChallengeGaugeParts = _currentScythingHapticChallengeSo
            .GaugeParts[Random.Range(0, _currentScythingHapticChallengeSo.GaugeParts.Count)].GaugeParts;
        scythingHapticChallengeGaugeArrowRectTransform.anchoredPosition = 
            new Vector2(scythingHapticChallengeGaugeArrowRectTransform.anchoredPosition.x,
            Random.Range(scythingHapticChallengeGaugeRectTransform.sizeDelta.y * -0.5f,
                scythingHapticChallengeGaugeRectTransform.sizeDelta.y * 0.5f));
        _isScythingHapticChallengeGoingUp = Random.Range(0, 2) == 0;
        _currentScythingHapticChallengeTurn = 0;
        _isScythingHapticChallengeActive = true;
        _isCollectHapticChallengeActive = true;
        
        StartTurnScythingChallenge();
    }

    public void StopScythingHapticChallenge()
    {
        if (!_isScythingHapticChallengeActive) return;
        
        scythingHapticChallengeGameObject.SetActive(false);
        _isScythingHapticChallengeActive = false;
        StopCollectHapticChallenge();
    }
    
    private void UpdateScythingHapticChallenge()
    {
        if (CheckInputScythingChallenge()) return;
        
        if (_isScythingHapticChallengeGoingUp)
        {
            scythingHapticChallengeGaugeArrowRectTransform.anchoredPosition += new Vector2(0f,
                _currentScythingHapticChallengeSo.ArrowSpeed * scythingHapticChallengeGaugeRectTransform.sizeDelta.y * Time.deltaTime);
            if (scythingHapticChallengeGaugeArrowRectTransform.anchoredPosition.y >= scythingHapticChallengeGaugeRectTransform.sizeDelta.y * 0.5f)
            {
                _isScythingHapticChallengeGoingUp = false;
            }
        }
        else
        {
            scythingHapticChallengeGaugeArrowRectTransform.anchoredPosition -= new Vector2(0f,
                _currentScythingHapticChallengeSo.ArrowSpeed * scythingHapticChallengeGaugeRectTransform.sizeDelta.y * Time.deltaTime);
            if (scythingHapticChallengeGaugeArrowRectTransform.anchoredPosition.y <= scythingHapticChallengeGaugeRectTransform.sizeDelta.y * -0.5f)
            {
                _isScythingHapticChallengeGoingUp = true;
            }
        }
    }

    private bool CheckInputScythingChallenge()
    {
        if (JoystickInputValue == Vector2.zero || JoystickInputValue == Vector2.left ||
            JoystickInputValue == Vector2.up || JoystickInputValue == Vector2.down)
        {
            _lastJoystickInputValue = JoystickInputValue;
            return false;
        }

        if (JoystickInputValue == Vector2.right)
        {
            if (_lastJoystickInputValue == Vector2.zero)
            {
                _lastJoystickInputValue = JoystickInputValue;
                NextTurnScythingChallenge();
                return true;
            }
            
            _lastJoystickInputValue = JoystickInputValue;
            return false;
        }

        return false;
    }
    
    private void StartTurnScythingChallenge()
    {
        HapticChallengeGaugeParts currentGaugeParts = _currentScythingHapticChallengeGaugeParts[_currentScythingHapticChallengeTurn];
        scythingHapticChallengeCutLineRectTransform.anchoredPosition = new Vector2(
            scythingHapticChallengeCutLineRectTransform.anchoredPosition.x,
            ((currentGaugeParts.PerfectGaugeMaxValue - currentGaugeParts.PerfectGaugeMinValue) * 0.5f +
             currentGaugeParts.PerfectGaugeMinValue) * scythingHapticChallengeGaugeRectTransform.sizeDelta.y);
    }
    
    private void NextTurnScythingChallenge()
    {
        _currentScythingHapticChallengeTurn++;
        
        if (_currentScythingHapticChallengeTurn == _currentScythingHapticChallengeGaugeParts.Count)
        {
            StopScythingHapticChallenge();
            return;
        }
        
        StartTurnScythingChallenge();
    }


    private void StartUnearthingHapticChallenge()
    {
        unearthingHapticChallengeGameObject.SetActive(true);
        _isUnearthingHapticChallengeActive = true;
        _isCollectHapticChallengeActive = true;
        _currentUnearthingHapticChallengeTime = 0f;
        unearthingHapticChallengeGaugeImage.fillAmount = 0f;
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
}
