using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
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
    
    [Header("Scraping Haptic Challenge UI")]
    [SerializeField] private GameObject scrapingHapticChallengeGameObject;
    [SerializeField] private SplineContainer scrapingHapticChallengePreviewSplineContainer;
    [SerializeField] private SplineExtrude scrapingHapticChallengePreviewSplineExtrude;
    [SerializeField] private SplineContainer scrapingHapticChallengeDrawnSplineContainer;
    [SerializeField] private SplineExtrude scrapingHapticChallengeDrawnSplineExtrude;
    [SerializeField] private Image scrapingHapticChallengeStartPositionImage;
    [SerializeField] private Image scrapingHapticChallengeEndPositionImage;
    [SerializeField] private Image scrapingHapticChallengeCurrentPositionImage;
    [SerializeField] private float scrapingHapticChallengeCanvasSplineScale = 108f;
    
    [Header("Harvest Haptic Challenge UI")]
    [SerializeField] private GameObject harvestHapticChallengeGameObject;
    [SerializeField] private RectTransform harvestHapticChallengeGaugeRectTransform;
    [SerializeField] private RectTransform harvestHapticChallengeCutLineRectTransform;
    [SerializeField] private RectTransform harvestHapticChallengeGaugeArrowRectTransform;
    
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
    
    // Scraping Haptic Challenge
    private ScrapingHapticChallengeSo _currentScrapingHapticChallengeSo;
    private bool _isScrapingHapticChallengeActive;
    private int _currentScrapingHapticChallengeRouteIndex;
    private float _currentScrapingHapticChallengeTime;
    
    // Harvest Haptic Challenge
    private HarvestHapticChallengeSo _currentHarvestHapticChallengeSo;
    private bool _isHarvestHapticChallengeActive;
    private bool _hasHarvestHapticChallengeBeenTriggered;
    private List<HapticChallengeGaugeParts> _currentHarvestHapticChallengeGaugeParts;
    private int _currentHarvestHapticChallengeTurn;
    
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        scythingHapticChallengeGameObject.SetActive(false);
        unearthingHapticChallengeGameObject.SetActive(false);
        scrapingHapticChallengeGameObject.SetActive(false);
        harvestHapticChallengeGameObject.SetActive(false);
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
        
        if (_isScrapingHapticChallengeActive)
        {
            UpdateScrapingHapticChallenge();
        }
        
        if (_isHarvestHapticChallengeActive)
        {
            UpdateHarvestHapticChallenge();
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
                
                if (ingredientTypeHapticChallenge.CollectHapticChallengeSo is ScrapingHapticChallengeSo scrapingHapticChallengeSo)
                {
                    _currentScrapingHapticChallengeSo = scrapingHapticChallengeSo;
                    StartScrapingHapticChallenge();
                }
                
                if (ingredientTypeHapticChallenge.CollectHapticChallengeSo is HarvestHapticChallengeSo harvestHapticChallengeSo)
                {
                    _currentHarvestHapticChallengeSo = harvestHapticChallengeSo;
                    StartHarvestHapticChallenge();
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


    #region Scything Haptic Challenge

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
        if (JoystickInputValue == Vector2.zero)
        {
            _lastJoystickInputValue = JoystickInputValue;
            return false;
        }
        
        if (JoystickInputValue.x < -1f + _currentScythingHapticChallengeSo.InputDetectionTolerance &&
            JoystickInputValue.y < _currentScythingHapticChallengeSo.InputDetectionTolerance &&
            JoystickInputValue.y > -_currentScythingHapticChallengeSo.InputDetectionTolerance)
        {
            _lastJoystickInputValue = Vector2.left;
            return false;
        }
        
        if (JoystickInputValue.y > 1f - _currentScythingHapticChallengeSo.InputDetectionTolerance &&
            JoystickInputValue.x < _currentScythingHapticChallengeSo.InputDetectionTolerance &&
            JoystickInputValue.x > -_currentScythingHapticChallengeSo.InputDetectionTolerance)
        {
            _lastJoystickInputValue = Vector2.up;
            return false;
        }
        
        if (JoystickInputValue.y < -1f + _currentScythingHapticChallengeSo.InputDetectionTolerance &&
            JoystickInputValue.x < _currentScythingHapticChallengeSo.InputDetectionTolerance &&
            JoystickInputValue.x > -_currentScythingHapticChallengeSo.InputDetectionTolerance)
        {
            _lastJoystickInputValue = Vector2.down;
            return false;
        }

        if (JoystickInputValue.x > 1f - _currentScythingHapticChallengeSo.InputDetectionTolerance &&
            JoystickInputValue.y < _currentScythingHapticChallengeSo.InputDetectionTolerance &&
            JoystickInputValue.y > -_currentScythingHapticChallengeSo.InputDetectionTolerance)
        {
            if (_lastJoystickInputValue == Vector2.zero)
            {
                _lastJoystickInputValue = Vector2.right;
                NextTurnScythingChallenge();
                return true;
            }
            
            _lastJoystickInputValue = Vector2.right;
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

    #endregion


    #region Unearthing Haptic Challenge

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

    #endregion
    
    
    #region Scraping Haptic Challenge
    
    private void StartScrapingHapticChallenge()
    {
        scrapingHapticChallengeGameObject.SetActive(true);
        _isScrapingHapticChallengeActive = true;
        _isCollectHapticChallengeActive = true;
        _currentScrapingHapticChallengeRouteIndex = Random.Range(0, _currentScrapingHapticChallengeSo.Routes.Count);
        _currentScrapingHapticChallengeTime = 0f;
        
        scrapingHapticChallengePreviewSplineContainer.Spline.Clear();
        foreach (Vector3 point in _currentScrapingHapticChallengeSo.Routes[_currentScrapingHapticChallengeRouteIndex].Points)
        {
            scrapingHapticChallengePreviewSplineContainer.Spline.Add(new BezierKnot(point), TangentMode.AutoSmooth);
        }
        scrapingHapticChallengePreviewSplineExtrude.Rebuild();
        
        scrapingHapticChallengeDrawnSplineContainer.Spline.Clear();
        scrapingHapticChallengeDrawnSplineContainer.Spline.Add(new BezierKnot(_currentScrapingHapticChallengeSo.Routes[_currentScrapingHapticChallengeRouteIndex].Points[0]), TangentMode.AutoSmooth);
        scrapingHapticChallengeDrawnSplineExtrude.Rebuild();
        
        scrapingHapticChallengeStartPositionImage.rectTransform.anchoredPosition = new Vector2(
            _currentScrapingHapticChallengeSo.Routes[_currentScrapingHapticChallengeRouteIndex].Points[0].x * scrapingHapticChallengeCanvasSplineScale,
            _currentScrapingHapticChallengeSo.Routes[_currentScrapingHapticChallengeRouteIndex].Points[0].y * scrapingHapticChallengeCanvasSplineScale);
        scrapingHapticChallengeEndPositionImage.rectTransform.anchoredPosition = new Vector2(
            _currentScrapingHapticChallengeSo.Routes[_currentScrapingHapticChallengeRouteIndex].Points[^1].x * scrapingHapticChallengeCanvasSplineScale,
            _currentScrapingHapticChallengeSo.Routes[_currentScrapingHapticChallengeRouteIndex].Points[^1].y * scrapingHapticChallengeCanvasSplineScale);
        scrapingHapticChallengeCurrentPositionImage.rectTransform.anchoredPosition = new Vector2(
            _currentScrapingHapticChallengeSo.Routes[_currentScrapingHapticChallengeRouteIndex].Points[0].x * scrapingHapticChallengeCanvasSplineScale,
            _currentScrapingHapticChallengeSo.Routes[_currentScrapingHapticChallengeRouteIndex].Points[0].y * scrapingHapticChallengeCanvasSplineScale);
    }
    
    public void StopScrapingHapticChallenge()
    {
        if (!_isScrapingHapticChallengeActive) return;
        
        scrapingHapticChallengeGameObject.SetActive(false);
        _isScrapingHapticChallengeActive = false;
        StopCollectHapticChallenge();
    }
    
    private void UpdateScrapingHapticChallenge()
    {
        if (!ProcessInputScrapingChallenge()) return;

        if (CheckEndScrapingChallenge()) return;
        
        _currentScrapingHapticChallengeTime += Time.deltaTime;
        
        if (_currentScrapingHapticChallengeTime >= 1f / _currentScrapingHapticChallengeSo.DrawnPositionsSaveRate)
        {
            _currentScrapingHapticChallengeTime = 0f;
            scrapingHapticChallengeDrawnSplineContainer.Spline.Add(new BezierKnot(
                (Vector3)(scrapingHapticChallengeCurrentPositionImage.rectTransform.anchoredPosition / 
                          scrapingHapticChallengeCanvasSplineScale)), TangentMode.AutoSmooth);
            scrapingHapticChallengeDrawnSplineExtrude.Rebuild();
        }
    }

    private bool ProcessInputScrapingChallenge()
    {
        if (JoystickInputValue == Vector2.zero) return false;
        
        scrapingHapticChallengeCurrentPositionImage.rectTransform.anchoredPosition += JoystickInputValue.normalized *
            (_currentScrapingHapticChallengeSo.CursorSpeed * Time.deltaTime);
        
        return true;
    }

    private bool CheckEndScrapingChallenge()
    {
        if (Vector2.Distance(scrapingHapticChallengeCurrentPositionImage.rectTransform.anchoredPosition,
            scrapingHapticChallengeEndPositionImage.rectTransform.anchoredPosition) <= _currentScrapingHapticChallengeSo.EndPointDistanceTolerance)
        {
            StopScrapingHapticChallenge();
            return true;
        }
        
        return false;
    }

    #endregion
    
    
    #region Harvest Haptic Challenge
    
    private void StartHarvestHapticChallenge()
    {
        harvestHapticChallengeGameObject.SetActive(true);
        _isHarvestHapticChallengeActive = true;
        _isCollectHapticChallengeActive = true;
        _hasHarvestHapticChallengeBeenTriggered = false;
        _currentHarvestHapticChallengeGaugeParts = _currentHarvestHapticChallengeSo
            .GaugeParts[Random.Range(0, _currentHarvestHapticChallengeSo.GaugeParts.Count)].GaugeParts;
        _currentHarvestHapticChallengeTurn = 0;
        
        StartTurnHarvestChallenge();
    }
    
    public void StopHarvestHapticChallenge()
    {
        if (!_isHarvestHapticChallengeActive) return;
        
        harvestHapticChallengeGameObject.SetActive(false);
        _isHarvestHapticChallengeActive = false;
        StopCollectHapticChallenge();
    }
    
    private void UpdateHarvestHapticChallenge()
    {
        if (!_hasHarvestHapticChallengeBeenTriggered) return;
        
        harvestHapticChallengeGaugeArrowRectTransform.anchoredPosition += new Vector2(
            harvestHapticChallengeGaugeRectTransform.sizeDelta.x * _currentHarvestHapticChallengeSo.ArrowSpeed *
            Time.deltaTime, 0f);
    }
    
    private void StartTurnHarvestChallenge()
    {
        harvestHapticChallengeGaugeArrowRectTransform.anchoredPosition = 
            new Vector2(0f, harvestHapticChallengeGaugeArrowRectTransform.anchoredPosition.y);
        HapticChallengeGaugeParts currentGaugeParts = _currentHarvestHapticChallengeGaugeParts[_currentHarvestHapticChallengeTurn];
        harvestHapticChallengeCutLineRectTransform.anchoredPosition = new Vector2(
            ((currentGaugeParts.PerfectGaugeMaxValue - currentGaugeParts.PerfectGaugeMinValue) * 0.5f + 
             currentGaugeParts.PerfectGaugeMinValue) * harvestHapticChallengeGaugeRectTransform.sizeDelta.x,
            harvestHapticChallengeCutLineRectTransform.anchoredPosition.y);
        _hasHarvestHapticChallengeBeenTriggered = false;
    }
    
    public void StopTurnHarvestChallenge()
    {
        if (!_isHarvestHapticChallengeActive) return;
        
        // Check arrow position
        
        NextTurnHarvestChallenge();
    }
    
    private void NextTurnHarvestChallenge()
    {
        _currentHarvestHapticChallengeTurn++;
        
        if (_currentHarvestHapticChallengeTurn == _currentHarvestHapticChallengeGaugeParts.Count)
        {
            StopHarvestHapticChallenge();
            return;
        }
        
        StartTurnHarvestChallenge();
    }
    
    public void ActivateHarvestHapticChallenge()
    {
        if (!_isHarvestHapticChallengeActive) return;
        
        _hasHarvestHapticChallengeBeenTriggered = true;
    }
    
    #endregion
}
