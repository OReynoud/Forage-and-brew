using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static TemperatureHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private TemperatureHapticChallengeGlobalValuesSo temperatureHapticChallengeGlobalValuesSo;
    
    [Header("UI")]
    [SerializeField] private GameObject temperatureChallengeGameObject;
    [SerializeField] private RectTransform gaugeRectTransform;
    [SerializeField] private RectTransform lowHeatGaugeRectTransform;
    [SerializeField] private RectTransform mediumHeatGaugeRectTransform;
    [SerializeField] private RectTransform highHeatGaugeRectTransform;
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
    
    [Header("Start Values")]
    [SerializeField] private Temperature startTemperature;
    
    private bool _isChallengeActive;
    private bool _hasInput;
    private float _currentBlowTime;
    private float _temperatureMaintenanceTime;
    private float _previousTemperatureMaintenanceTime;
    private Temperature _currentTemperature;
    
    public BellowsBehaviour CurrentBellows { get; set; }
    
    // Input
    public Vector2 JoystickInputValue { get; set; }
    
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        temperatureChallengeGameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isChallengeActive) return;
        
        UpdateTemperatureChallenge();
    }
    
    
    public void StartTemperatureChallenge()
    {
        if (!CurrentBellows) return;
        
        _previousCameraPreset = CameraController.instance.TargetCamSettings;
        CameraController.instance.ApplyScriptableCamSettings(stirChallengeCameraPreset, cauldronCameraTransitionTime);

        transform.position = CurrentBellows.transform.position + CurrentBellows.transform.rotation * characterBlowPosition;
        transform.rotation = CurrentBellows.transform.rotation * Quaternion.Euler(characterBlowRotation);
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnableHapticChallengeJoystickInputs();
        
        _isChallengeActive = true;
        _currentBlowTime = 0f;
        _temperatureMaintenanceTime = 0f;
        _currentTemperature = startTemperature;
        
        temperatureChallengeGameObject.SetActive(true);
        
        float startTemperatureValue = 0f;
        switch (startTemperature)
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
        
        if (CheckInputReset())
        {
            _hasInput = false;
        }
        
        if (!_hasInput && CheckInput())
        {
            _hasInput = true;
            _currentBlowTime = temperatureHapticChallengeGlobalValuesSo.HeatIncreaseDuration;
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
        _previousTemperatureMaintenanceTime = _temperatureMaintenanceTime;
        
        switch (_currentTemperature)
        {
            case Temperature.LowHeat:
                if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.MediumHeat;
                }
                else if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue &&
                         gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.LowHeatMaxValue)
                {
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
            case Temperature.MediumHeat:
                if (gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.LowHeatMaxValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.LowHeat;
                }
                else if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.HighHeat;
                }
                else if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue &&
                         gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.MediumHeatMaxValue)
                {
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
            case Temperature.HighHeat:
                if (gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.MediumHeatMaxValue)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.MediumHeat;
                }
                else if (gaugeImage.fillAmount >= temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue &&
                         gaugeImage.fillAmount <= temperatureHapticChallengeGlobalValuesSo.HighHeatMaxValue)
                {
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
        }

        temperatureMaintenanceTimeText.text = Mathf.CeilToInt(temperatureHapticChallengeGlobalValuesSo.
            TemperatureMaintenanceDuration - _temperatureMaintenanceTime).ToString();

        if (Mathf.Approximately(_temperatureMaintenanceTime, _previousTemperatureMaintenanceTime))
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

    private void StopTemperatureChallenge()
    {
        CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, cauldronCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        
        startTemperature = _currentTemperature;
        _isChallengeActive = false;
        temperatureChallengeGameObject.SetActive(false);
    }

    private bool CheckInput()
    {
        return JoystickInputValue == Vector2.down;
    }
    
    private bool CheckInputReset()
    {
        return JoystickInputValue == Vector2.zero;
    }


    private void OnDrawGizmos()
    {
        lowHeatGaugeRectTransform.sizeDelta = new Vector2(lowHeatGaugeRectTransform.sizeDelta.x,
            gaugeRectTransform.sizeDelta.y * (temperatureHapticChallengeGlobalValuesSo.LowHeatMaxValue -
                                              temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue));
        lowHeatGaugeRectTransform.anchoredPosition = new Vector2(lowHeatGaugeRectTransform.anchoredPosition.x,
            gaugeRectTransform.sizeDelta.y * (temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue +
                                              (temperatureHapticChallengeGlobalValuesSo.LowHeatMaxValue -
                                               temperatureHapticChallengeGlobalValuesSo.LowHeatMinValue) * 0.5f));
        mediumHeatGaugeRectTransform.sizeDelta = new Vector2(mediumHeatGaugeRectTransform.sizeDelta.x,
            gaugeRectTransform.sizeDelta.y * (temperatureHapticChallengeGlobalValuesSo.MediumHeatMaxValue -
                                              temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue));
        mediumHeatGaugeRectTransform.anchoredPosition = new Vector2(mediumHeatGaugeRectTransform.anchoredPosition.x,
            gaugeRectTransform.sizeDelta.y * (temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue +
                                              (temperatureHapticChallengeGlobalValuesSo.MediumHeatMaxValue -
                                               temperatureHapticChallengeGlobalValuesSo.MediumHeatMinValue) * 0.5f));
        highHeatGaugeRectTransform.sizeDelta = new Vector2(highHeatGaugeRectTransform.sizeDelta.x,
            gaugeRectTransform.sizeDelta.y * (temperatureHapticChallengeGlobalValuesSo.HighHeatMaxValue -
                                              temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue));
        highHeatGaugeRectTransform.anchoredPosition = new Vector2(highHeatGaugeRectTransform.anchoredPosition.x,
            gaugeRectTransform.sizeDelta.y * (temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue +
                                              (temperatureHapticChallengeGlobalValuesSo.HighHeatMaxValue -
                                               temperatureHapticChallengeGlobalValuesSo.HighHeatMinValue) * 0.5f));
    }
}
