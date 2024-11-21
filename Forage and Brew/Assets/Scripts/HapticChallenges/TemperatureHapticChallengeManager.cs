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
    [SerializeField] private RectTransform gaugeArrowRectTransform;
    [SerializeField] private RectTransform gaugeRectTransform;
    [SerializeField] private Image gaugeImage;
    
    [Header("Camera")]
    [SerializeField] private CameraPreset stirChallengeCameraPreset;
    [SerializeField] private float cauldronCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
    
    [Header("Character")]
    [SerializeField] private Vector3 characterStirPosition;
    [SerializeField] private Vector3 characterStirRotation;
    
    [Header("Start Values")]
    [SerializeField] private Temperature startTemperature;
    
    private bool _isChallengeActive;
    private bool _hasInput;
    private float _currentBlowTime;
    private float _temperatureMaintenanceTime;
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

        transform.position = CurrentBellows.transform.position + characterStirPosition;
        transform.rotation = Quaternion.Euler(characterStirRotation);
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
                startTemperatureValue = temperatureHapticChallengeGlobalValuesSo.LowHeatPart * 0.5f;
                break;
            case Temperature.MediumHeat:
                startTemperatureValue = (1f - temperatureHapticChallengeGlobalValuesSo.HighHeatPart -
                                         temperatureHapticChallengeGlobalValuesSo.LowHeatPart) * 0.5f +
                                        temperatureHapticChallengeGlobalValuesSo.LowHeatPart;
                break;
            case Temperature.HighHeat:
                startTemperatureValue = 1f - temperatureHapticChallengeGlobalValuesSo.HighHeatPart * 0.5f;
                break;
        }
        
        gaugeImage.fillAmount = startTemperatureValue;
        gaugeArrowRectTransform.anchoredPosition = new Vector2(gaugeArrowRectTransform.anchoredPosition.x,
            startTemperatureValue * gaugeRectTransform.rect.height);
    }
    
    private void UpdateTemperatureChallenge()
    {
        if (CheckInputReset())
        {
            _hasInput = false;
        }
        
        if (_currentBlowTime > 0f)
        {
            _currentBlowTime -= Time.deltaTime;
            gaugeImage.fillAmount = Mathf.Min(1f, gaugeImage.fillAmount + Time.deltaTime *
                temperatureHapticChallengeGlobalValuesSo.HeatIncreaseQuantity /
                temperatureHapticChallengeGlobalValuesSo.HeatIncreaseDuration);
            gaugeArrowRectTransform.anchoredPosition = new Vector2(gaugeArrowRectTransform.anchoredPosition.x,
                gaugeImage.fillAmount * gaugeRectTransform.rect.height);
            return;
        }

        if (_hasInput) return;
        
        if (CheckInput())
        {
            _hasInput = true;
            _currentBlowTime = temperatureHapticChallengeGlobalValuesSo.HeatIncreaseDuration;
            return;
        }
        
        gaugeImage.fillAmount = Mathf.Max(0f, gaugeImage.fillAmount - Time.deltaTime *
            temperatureHapticChallengeGlobalValuesSo.HeatDecreaseSpeed);
        gaugeArrowRectTransform.anchoredPosition = new Vector2(gaugeArrowRectTransform.anchoredPosition.x,
            gaugeImage.fillAmount * gaugeRectTransform.rect.height);

        switch (_currentTemperature)
        {
            case Temperature.LowHeat:
                if (gaugeImage.fillAmount > temperatureHapticChallengeGlobalValuesSo.LowHeatPart)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.MediumHeat;
                }
                else
                {
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
            case Temperature.MediumHeat:
                if (gaugeImage.fillAmount < temperatureHapticChallengeGlobalValuesSo.LowHeatPart)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.LowHeat;
                }
                else if (gaugeImage.fillAmount > 1f - temperatureHapticChallengeGlobalValuesSo.HighHeatPart)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.HighHeat;
                }
                else
                {
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
            case Temperature.HighHeat:
                if (gaugeImage.fillAmount < 1f - temperatureHapticChallengeGlobalValuesSo.HighHeatPart)
                {
                    _temperatureMaintenanceTime = 0f;
                    _currentTemperature = Temperature.MediumHeat;
                }
                else
                {
                    _temperatureMaintenanceTime += Time.deltaTime;
                }
                break;
        }
        
        if (_temperatureMaintenanceTime > temperatureHapticChallengeGlobalValuesSo.TemperatureMaintenanceDuration)
        {
            StopTemperatureChallenge();
        }
    }

    private void StopTemperatureChallenge()
    {
        CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, cauldronCameraTransitionTime);
        CharacterInputManager.Instance.EnableMoveInputs();
        
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
}
