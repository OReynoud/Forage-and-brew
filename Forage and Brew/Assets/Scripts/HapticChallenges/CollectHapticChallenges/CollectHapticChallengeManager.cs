using UnityEngine;
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
    
    // Global variables
    private bool _isCollectHapticChallengeActive;
    private IngredientToCollectBehaviour _currentIngredientToCollectBehaviour;
    public Vector2 JoystickInputValue { get; set; }
    
    // Gauge Haptic Challenge
    private GaugeHapticChallengeSo _currentGaugeHapticChallengeSo;
    private bool _isGaugeHapticChallengeActive;
    private bool _isGaugeHapticChallengeGoingUp;
    
    
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
                
                return;
            }
        }
    }

    public void StopCollectHapticChallenge()
    {
        if (_isGaugeHapticChallengeActive)
        {
            StopGaugeHapticChallenge();
        }
        
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

    private void StopGaugeHapticChallenge()
    {
        gaugeHapticChallengeGameObject.SetActive(false);
        _isGaugeHapticChallengeActive = false;
        _isCollectHapticChallengeActive = false;
        _currentIngredientToCollectBehaviour.Collect();
        _currentIngredientToCollectBehaviour = null;
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


    private void OnDrawGizmos()
    {
        if (collectHapticChallengeListSo)
        {
            foreach (var ingredientTypeHapticChallenge in collectHapticChallengeListSo.HapticChallengesByIngredientType)
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
