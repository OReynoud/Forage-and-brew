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
    private GaugeHapticChallengeSo currentGaugeHapticChallengeSo;
    private bool isGaugeHapticChallengeActive;
    private bool isGaugeHapticChallengeGoingUp;
    
    
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
        if (isGaugeHapticChallengeActive)
        {
            UpdateGaugeHapticChallenge();
        }
    }


    public void StartHapticChallenge(IngredientType ingredientType)
    {
        foreach (var ingredientTypeHapticChallenge in hapticChallengeListSo.HapticChallengesByIngredientType)
        {
            if (ingredientTypeHapticChallenge.IngredientType == ingredientType)
            {
                if (ingredientTypeHapticChallenge.HapticChallengeSo is GaugeHapticChallengeSo gaugeHapticChallengeSo)
                {
                    currentGaugeHapticChallengeSo = gaugeHapticChallengeSo;
                    StartGaugeHapticChallenge();
                }
                
                return;
            }
        }
    }

    private void StartGaugeHapticChallenge()
    {
        gaugeHapticChallengeGameObject.SetActive(true);
        gaugeArrowRectTransform.anchoredPosition = new Vector2(gaugeArrowRectTransform.anchoredPosition.x,
            Random.Range(currentGaugeHapticChallengeSo.GaugeTotalHeight * -0.5f,
                currentGaugeHapticChallengeSo.GaugeTotalHeight * 0.5f));
        isGaugeHapticChallengeGoingUp = Random.Range(0, 2) == 0;
        isGaugeHapticChallengeActive = true;
    }
    
    private void UpdateGaugeHapticChallenge()
    {
        if (isGaugeHapticChallengeGoingUp)
        {
            gaugeArrowRectTransform.anchoredPosition += new Vector2(0f, currentGaugeHapticChallengeSo.ArrowSpeed * Time.deltaTime);
            if (gaugeArrowRectTransform.anchoredPosition.y >= gaugeRectTransform.sizeDelta.y * 0.5f)
            {
                isGaugeHapticChallengeGoingUp = false;
            }
        }
        else
        {
            gaugeArrowRectTransform.anchoredPosition -= new Vector2(0f, currentGaugeHapticChallengeSo.ArrowSpeed * Time.deltaTime);
            if (gaugeArrowRectTransform.anchoredPosition.y <= gaugeRectTransform.sizeDelta.y * -0.5f)
            {
                isGaugeHapticChallengeGoingUp = true;
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (hapticChallengeListSo)
        {
            foreach (var ingredientTypeHapticChallenge in hapticChallengeListSo.HapticChallengesByIngredientType)
            {
                if (ingredientTypeHapticChallenge.HapticChallengeSo is GaugeHapticChallengeSo gaugeHapticChallengeSo)
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
