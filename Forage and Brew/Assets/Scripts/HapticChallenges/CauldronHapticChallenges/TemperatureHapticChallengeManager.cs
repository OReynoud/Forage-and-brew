using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static TemperatureHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private TemperatureHapticChallengeGlobalValuesSo temperatureHapticChallengeGlobalValuesSo;
    [SerializeField] private Animator characterAnimator;
    
    [Header("UI")]
    [SerializeField] private GameObject temperatureChallengeGameObject;
    [SerializeField] private RectTransform gaugeRectTransform;
    [SerializeField] private RectTransform minLowHeatCutLineRectTransform;
    [SerializeField] private RectTransform cutLineBetweenLowAndMediumHeatRectTransform;
    [SerializeField] private RectTransform cutLineBetweenMediumAndHighHeatRectTransform;
    [SerializeField] private RectTransform maxHighHeatCutLineRectTransform;
    [SerializeField] private RectTransform lowHeatFlameRectTransform;
    [SerializeField] private RectTransform mediumHeatFlameRectTransform;
    [SerializeField] private RectTransform highHeatFlameRectTransform;
    [SerializeField] private RectTransform gaugeArrowRectTransform;
    [SerializeField] private Image gaugeImage;
    [SerializeField] private Image temperatureMaintenanceTimeImage;
    [SerializeField] private TMP_Text temperatureMaintenanceTimeText;
    
    [Header("Camera")]
    [SerializeField] private CameraPreset stirChallengeCameraPreset;
    [SerializeField] private float cauldronCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
    
    [Header("Character")]
    [SerializeField] private Vector3 characterBlowPosition;
    [SerializeField] private Vector3 characterBlowRotation;
    
    public bool IsChallengeActive { get; set; }
    private float _currentBlowTime;
    private float _temperatureMaintenanceTime;
    private Temperature _currentTemperature;
    
    public BellowsBehaviour CurrentBellows { get; set; }
    
    // Animator Hashes
    private static readonly int DoPushBellows = Animator.StringToHash("DoPushBellows");
    
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        temperatureChallengeGameObject.SetActive(false);
        
        if (CauldronVfxManager.Instance)
        {
            _currentTemperature = GameDontDestroyOnLoadManager.Instance.CauldronTemperature;
            CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
        }
    }

    private void Update()
    {
        if (!IsChallengeActive) return;
        
        UpdateTemperatureChallenge();
    }
    
    
    public void StartTemperatureChallenge()
    {
        if (!CurrentBellows) return;
        IsChallengeActive = true;
        
        _previousCameraPreset = CameraController.instance.TargetCamSettings;
        CameraController.instance.ApplyScriptableCamSettings(stirChallengeCameraPreset, cauldronCameraTransitionTime);

        transform.position = CurrentBellows.transform.position + CurrentBellows.transform.rotation * characterBlowPosition;
        transform.rotation = CurrentBellows.transform.rotation * Quaternion.Euler(characterBlowRotation);
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnableTemperatureHapticChallengeInputs();        
        CharacterInputManager.Instance.EnableQuitHapticChallengeInputs();
        
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = true;
        
        CurrentBellows.DisableInteract();
        CurrentBellows.EnterTemperatureHapticChallenge();
        
        _currentBlowTime = 0f;
        _temperatureMaintenanceTime = 0f;
        
        temperatureChallengeGameObject.SetActive(true);
        
        float startTemperatureValue = 0f;
        switch (_currentTemperature)
        {
            case Temperature.LowHeat:
                startTemperatureValue = (temperatureHapticChallengeGlobalValuesSo.LowHeatMaxValue -
                                         temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue) * 0.5f +
                                        temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue;
                break;
            case Temperature.MediumHeat:
                startTemperatureValue = (temperatureHapticChallengeGlobalValuesSo.MediumHeatMaxValue -
                                         temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue) * 0.5f +
                                        temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue;
                break;
            case Temperature.HighHeat:
                startTemperatureValue = (temperatureHapticChallengeGlobalValuesSo.HighHeatMaxValue -
                                         temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue) * 0.5f +
                                        temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue;
                break;
        }
        
        gaugeImage.fillAmount = startTemperatureValue;
        gaugeArrowRectTransform.anchoredPosition = new Vector2(gaugeArrowRectTransform.anchoredPosition.x,
            startTemperatureValue * gaugeRectTransform.rect.height);
    }
    
    private void UpdateTemperatureChallenge()
    {
        if (_currentBlowTime > 0f)
        {
            _currentBlowTime -= Time.deltaTime;
            gaugeImage.fillAmount = Mathf.Min(1f, gaugeImage.fillAmount + Time.deltaTime *
                temperatureHapticChallengeGlobalValuesSo.HeatIncreaseQuantity /
                temperatureHapticChallengeGlobalValuesSo.HeatIncreaseDuration);
            gaugeArrowRectTransform.anchoredPosition = new Vector2(gaugeArrowRectTransform.anchoredPosition.x,
                gaugeImage.fillAmount * gaugeRectTransform.rect.height);
            UpdateCurrentTemperatureAndMaintenanceTime();
            return;
        }
        
        gaugeImage.fillAmount = Mathf.Max(0f, gaugeImage.fillAmount - Time.deltaTime *
            temperatureHapticChallengeGlobalValuesSo.HeatDecreaseSpeed);
        gaugeArrowRectTransform.anchoredPosition = new Vector2(gaugeArrowRectTransform.anchoredPosition.x,
            gaugeImage.fillAmount * gaugeRectTransform.rect.height);

        UpdateCurrentTemperatureAndMaintenanceTime();
        
        if (_temperatureMaintenanceTime > temperatureHapticChallengeGlobalValuesSo.TemperatureMaintenanceDuration)
        {
            StopTemperatureChallenge();
        }
    }

    private void UpdateCurrentTemperatureAndMaintenanceTime()
    {
        switch (_currentTemperature)
        {
            case Temperature.LowHeat:
                if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.MediumHeat;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                }
                else if (gaugeImage.fillAmount < temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue ||
                         gaugeImage.fillAmount > temperatureHapticChallengeGlobalValuesSo.LowHeatMaxValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.None;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                }
                else
                {
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
            case Temperature.MediumHeat:
                if (gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.LowHeatMaxValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.LowHeat;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                }
                else if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.HighHeat;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                }
                else if (gaugeImage.fillAmount < temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue ||
                         gaugeImage.fillAmount > temperatureHapticChallengeGlobalValuesSo.MediumHeatMaxValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.None;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                }
                else
                {
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
            case Temperature.HighHeat:
                if (gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.MediumHeatMaxValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.MediumHeat;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                }
                else if (gaugeImage.fillAmount < temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue ||
                         gaugeImage.fillAmount > temperatureHapticChallengeGlobalValuesSo.HighHeatMaxValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.None;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                }
                else
                {
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
            case Temperature.None:
                if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue &&
                    gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.LowHeatMaxValue)
                {
                    _currentTemperature = Temperature.LowHeat;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                else if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue &&
                         gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.MediumHeatMaxValue)
                {
                    _currentTemperature = Temperature.MediumHeat;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                else if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue &&
                         gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.HighHeatMaxValue)
                {
                    _currentTemperature = Temperature.HighHeat;
                    CauldronVfxManager.Instance.ChangeTemperatureVfx(_currentTemperature);
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
        }

        temperatureMaintenanceTimeText.text = Mathf.CeilToInt(temperatureHapticChallengeGlobalValuesSo.
            TemperatureMaintenanceDuration - _temperatureMaintenanceTime).ToString();

        if (_currentTemperature == Temperature.None)
        {
            temperatureMaintenanceTimeImage.color = new Color(temperatureMaintenanceTimeImage.color.r,
                temperatureMaintenanceTimeImage.color.g, temperatureMaintenanceTimeImage.color.b, 0.5f);
            temperatureMaintenanceTimeText.color = new Color(temperatureMaintenanceTimeText.color.r,
                temperatureMaintenanceTimeText.color.g, temperatureMaintenanceTimeText.color.b, 0.5f);
        }
        else
        {
            temperatureMaintenanceTimeImage.color = new Color(temperatureMaintenanceTimeImage.color.r,
                temperatureMaintenanceTimeImage.color.g, temperatureMaintenanceTimeImage.color.b, 1f);
            temperatureMaintenanceTimeText.color = new Color(temperatureMaintenanceTimeText.color.r,
                temperatureMaintenanceTimeText.color.g, temperatureMaintenanceTimeText.color.b, 1f);
        }
    }

    public void StopTemperatureChallenge(bool isSuccessful =  true)
    {
        if (!IsChallengeActive) return;
        
        CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, cauldronCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        CurrentBellows.EnableInteract();
        CurrentBellows.ExitTemperatureHapticChallenge();

        if (isSuccessful)
        {
            CauldronBehaviour.instance.AddTemperature(_currentTemperature);
            GameDontDestroyOnLoadManager.Instance.CauldronTemperature = _currentTemperature;
        }
        
        IsChallengeActive = false;
        temperatureChallengeGameObject.SetActive(false);        
        
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = false;
    }
    
    public void IncreaseTemperature()
    {
        if (!IsChallengeActive) return;
        
        characterAnimator.SetTrigger(DoPushBellows);
        CurrentBellows.PlayBellowsAnimation();
        
        _currentBlowTime = temperatureHapticChallengeGlobalValuesSo.HeatIncreaseDuration;
    }


    private void OnDrawGizmos()
    {
        minLowHeatCutLineRectTransform.anchoredPosition = new Vector2(minLowHeatCutLineRectTransform.anchoredPosition.x,
            temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue * gaugeRectTransform.rect.height);
        cutLineBetweenLowAndMediumHeatRectTransform.anchoredPosition = new Vector2(cutLineBetweenLowAndMediumHeatRectTransform.anchoredPosition.x,
            temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue * gaugeRectTransform.rect.height);
        cutLineBetweenMediumAndHighHeatRectTransform.anchoredPosition = new Vector2(cutLineBetweenMediumAndHighHeatRectTransform.anchoredPosition.x,
            temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue * gaugeRectTransform.rect.height);
        maxHighHeatCutLineRectTransform.anchoredPosition = new Vector2(maxHighHeatCutLineRectTransform.anchoredPosition.x,
            temperatureHapticChallengeGlobalValuesSo.HighHeatMaxValue * gaugeRectTransform.rect.height);
        lowHeatFlameRectTransform.anchoredPosition = new Vector2(lowHeatFlameRectTransform.anchoredPosition.x,
            ((temperatureHapticChallengeGlobalValuesSo.LowHeatMaxValue - temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue) * 0.5f 
             + temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue) * gaugeRectTransform.rect.height);
        mediumHeatFlameRectTransform.anchoredPosition = new Vector2(mediumHeatFlameRectTransform.anchoredPosition.x,
            ((temperatureHapticChallengeGlobalValuesSo.MediumHeatMaxValue - temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue) * 0.5f
             + temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue) * gaugeRectTransform.rect.height);
        highHeatFlameRectTransform.anchoredPosition = new Vector2(highHeatFlameRectTransform.anchoredPosition.x,
            ((temperatureHapticChallengeGlobalValuesSo.HighHeatMaxValue - temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue) * 0.5f
             + temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue) * gaugeRectTransform.rect.height);
    }
}
