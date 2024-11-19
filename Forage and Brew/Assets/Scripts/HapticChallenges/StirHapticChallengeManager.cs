using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StirHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static StirHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private StirHapticChallengeGlobalValuesSo stirHapticChallengeGlobalValuesSo;
    
    [Header("UI")]
    [SerializeField] private GameObject stirChallengeGameObject;
    [SerializeField] private GameObject clockwiseArrowGameObject;
    [SerializeField] private Image clockwiseArrowImage;
    [SerializeField] private GameObject rotationMarkerGameObject;
    [SerializeField] private Image rotationMarkerImage;
    [SerializeField] private StirHapticChallengeSo stirHapticChallengeSo;
    
    private StirHapticChallengeSo _currentChallenge;
    private float _currentStirTime;
    private int _currentStirIndex;
    private int _currentCheckIndex;
    private bool _isCurrentStirClockwise;
    private bool _isInPreview;
    
    // Input
    public Vector2 JoystickInputValue { get; set; }
    private Vector2 _lastJoystickInputValue;
    
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        stirChallengeGameObject.SetActive(false);
        
        StartStirChallenge(stirHapticChallengeSo);
    }

    private void Update()
    {
        if (!_currentChallenge) return;
        
        UpdateStirChallenge();
    }
    
    
    public void StartStirChallenge(StirHapticChallengeSo challenge)
    {
        _currentChallenge = challenge;
        _currentStirTime = 0;
        _currentStirIndex = 0;
        _currentCheckIndex = 0;
        _isCurrentStirClockwise = Random.Range(0, 2) == 0;
        
        stirChallengeGameObject.SetActive(true);
        
        StartPreview();
    }

    private void StartPreview()
    {
        _isInPreview = true;
        clockwiseArrowImage.color = new Color(clockwiseArrowImage.color.r, clockwiseArrowImage.color.g, clockwiseArrowImage.color.b, 0.5f);
        rotationMarkerImage.color = new Color(rotationMarkerImage.color.r, rotationMarkerImage.color.g, rotationMarkerImage.color.b, 0.5f);
        StartStirTurn();
    }

    private void StopPreview()
    {
        _isInPreview = false;
        clockwiseArrowImage.color = new Color(clockwiseArrowImage.color.r, clockwiseArrowImage.color.g, clockwiseArrowImage.color.b, 1f);
        rotationMarkerImage.color = new Color(rotationMarkerImage.color.r, rotationMarkerImage.color.g, rotationMarkerImage.color.b, 1f);
        _currentStirTime = 0;
        StartStirTurn();
    }

    private void StartStirTurn()
    {
        clockwiseArrowGameObject.SetActive(true);
        clockwiseArrowGameObject.transform.localScale = new Vector3(_isCurrentStirClockwise ? 1 : -1, 1, 1);
        rotationMarkerGameObject.SetActive(true);
        rotationMarkerGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
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
            
            if (_currentStirTime >= _currentChallenge.StirDurations[_currentStirIndex]) return;
        }
        else
        {
            if (Mathf.FloorToInt(_currentStirTime / stirHapticChallengeGlobalValuesSo.CheckPositionInterval) > _currentCheckIndex ||
                _currentStirTime >= _currentChallenge.StirDurations[_currentStirIndex])
            {
                _currentCheckIndex++;
                if (!CheckInput()) return;
            }
        
            if (_currentStirTime >= _currentChallenge.StirDurations[_currentStirIndex]) return;
        }
        
        _currentStirTime += Time.deltaTime;
        rotationMarkerGameObject.transform.Rotate(0, 0, (_isCurrentStirClockwise ? -1 : 1) * 360 * Time.deltaTime / _currentChallenge.StirDurations[_currentStirIndex]);
    }

    private void NextStirTurn()
    {
        _currentStirTime = 0;
        _currentStirIndex++;
        _currentCheckIndex = 0;
            
        if (_currentStirIndex >= _currentChallenge.StirDurations.Length)
        {
            StopStirChallenge();
            return;
        }
        
        _isCurrentStirClockwise = Random.Range(0, 2) == 0;
            
        StartPreview();
    }

    private void StopStirChallenge()
    {
        stirChallengeGameObject.SetActive(false);
        _currentChallenge = null;
    }
    
    
    private bool CheckInput()
    {
        if (_lastJoystickInputValue == Vector2.zero && JoystickInputValue == Vector2.zero) return true;
        
        if (JoystickInputValue == Vector2.zero)
        {
            _lastJoystickInputValue = Vector2.zero;
            NextStirTurn();
            return false;
        }
        
        _lastJoystickInputValue = JoystickInputValue;
        
        return true;
    }
    
    private bool CheckInputPreview()
    {
        return JoystickInputValue != Vector2.zero;
    }
}
