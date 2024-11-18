using UnityEngine;
using Random = UnityEngine.Random;

public class HapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static HapticChallengeManager Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private HapticChallengeListSo hapticChallengeListSo;

    [Header("Gauge Haptic Challenge UI")]
    [SerializeField] private GameObject gaugeHapticChallengeGameObject;
    [SerializeField] private RectTransform gaugeRectTransform;
    [SerializeField] private RectTransform wrongGaugeRectTransform;
    [SerializeField] private RectTransform correctGaugeRectTransform;
    [SerializeField] private RectTransform perfectGaugeRectTransform;
    [SerializeField] private RectTransform gaugeArrowRectTransform;
    
    // Gauge Haptic Challenge
    private GaugeCollectHapticChallengeSo _currentGaugeCollectHapticChallengeSo;
    private bool _isGaugeHapticChallengeActive;
    private bool _isGaugeHapticChallengeGoingUp;
    
    // Global variables
    private bool _isHapticChallengeActive;
    private IngredientToCollectBehaviour _currentIngredientToCollectBehaviour;
    
    
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


    public void StartHapticChallenge(IngredientToCollectBehaviour ingredientToCollectBehaviour)
    {
        _currentIngredientToCollectBehaviour = ingredientToCollectBehaviour;
        
        foreach (var ingredientTypeHapticChallenge in hapticChallengeListSo.HapticChallengesByIngredientType)
        {
            if (ingredientTypeHapticChallenge.IngredientType == _currentIngredientToCollectBehaviour.IngredientValuesSo.Type)
            {
                if (ingredientTypeHapticChallenge.CollectHapticChallengeSo is GaugeCollectHapticChallengeSo gaugeHapticChallengeSo)
                {
                    _currentGaugeCollectHapticChallengeSo = gaugeHapticChallengeSo;
                    StartGaugeHapticChallenge();
                }
                
                return;
            }
        }
    }

    public void StopHapticChallenge()
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
            Random.Range(_currentGaugeCollectHapticChallengeSo.GaugeTotalHeight * -0.5f,
                _currentGaugeCollectHapticChallengeSo.GaugeTotalHeight * 0.5f));
        _isGaugeHapticChallengeGoingUp = Random.Range(0, 2) == 0;
        _isGaugeHapticChallengeActive = true;
        _isHapticChallengeActive = true;
    }

    private void StopGaugeHapticChallenge()
    {
        gaugeHapticChallengeGameObject.SetActive(false);
        _isGaugeHapticChallengeActive = false;
        _isHapticChallengeActive = false;
        _currentIngredientToCollectBehaviour.Collect();
        _currentIngredientToCollectBehaviour = null;
    }
    
    private void UpdateGaugeHapticChallenge()
    {
        if (_isGaugeHapticChallengeGoingUp)
        {
            gaugeArrowRectTransform.anchoredPosition += new Vector2(0f, _currentGaugeCollectHapticChallengeSo.ArrowSpeed * Time.deltaTime);
            if (gaugeArrowRectTransform.anchoredPosition.y >= gaugeRectTransform.sizeDelta.y * 0.5f)
            {
                _isGaugeHapticChallengeGoingUp = false;
            }
        }
        else
        {
            gaugeArrowRectTransform.anchoredPosition -= new Vector2(0f, _currentGaugeCollectHapticChallengeSo.ArrowSpeed * Time.deltaTime);
            if (gaugeArrowRectTransform.anchoredPosition.y <= gaugeRectTransform.sizeDelta.y * -0.5f)
            {
                _isGaugeHapticChallengeGoingUp = true;
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (hapticChallengeListSo)
        {
            foreach (var ingredientTypeHapticChallenge in hapticChallengeListSo.HapticChallengesByIngredientType)
            {
                if (ingredientTypeHapticChallenge.CollectHapticChallengeSo is GaugeCollectHapticChallengeSo gaugeHapticChallengeSo)
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
