using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class GrindingHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static GrindingHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private GrindingHapticChallengeSo grindingHapticChallengeSo;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private GameObject mortarGameObject;
    
    [Header("UI")]
    [SerializeField] private GameObject grindingHapticChallengeGameObject;
    [SerializeField] private SplineContainer previewSplineContainer;
    [SerializeField] private SplineExtrude previewSplineExtrude;
    [SerializeField] private SplineContainer drawnSplineContainer;
    [SerializeField] private SplineExtrude drawnSplineExtrude;
    [SerializeField] private Image startPositionImage;
    [SerializeField] private Image endPositionImage;
    [SerializeField] private Image currentPositionImage;
    [SerializeField] private float canvasSplineScale = 108f;
    
    [Header("Camera")]
    [SerializeField] private CameraPreset grindingChallengeCameraPreset;
    [SerializeField] private float grindingCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
    
    [Header("Character")]
    [SerializeField] private Vector3 characterGrindingPosition;
    [SerializeField] private Vector3 characterGrindingRotation;
    
    public GrindingCountertopBehaviour CurrentGrindingCountertopBehaviour { get; set; }
    public Vector2 JoystickInputValue { get; set; }
    private bool _isChallengeActive;
    private int _currentRouteIndex;
    private float _currentTime;
    
    // Animator Hashes
    private static readonly int IsGrinding = Animator.StringToHash("IsGrinding");
    private static readonly int GrindSpeed = Animator.StringToHash("GrindSpeed");
    private static readonly int DoCrush = Animator.StringToHash("DoCrush");

    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        grindingHapticChallengeGameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isChallengeActive) return;
        
        UpdateGrindingChallenge();
    }
    
    
    public void StartGrindingChallenge()
    {
        if (!CurrentGrindingCountertopBehaviour) return;
        
        // Challenge variables
        _isChallengeActive = true;
        _currentRouteIndex = Random.Range(0, grindingHapticChallengeSo.Routes.Count);
        _currentTime = 0f;
        
        // UI
        grindingHapticChallengeGameObject.SetActive(true);
        
        previewSplineContainer.Spline.Clear();
        foreach (Vector3 point in grindingHapticChallengeSo.Routes[_currentRouteIndex].Points)
        {
            previewSplineContainer.Spline.Add(new BezierKnot(point), TangentMode.AutoSmooth);
        }
        previewSplineExtrude.Rebuild();
        
        drawnSplineContainer.Spline.Clear();
        drawnSplineContainer.Spline.Add(new BezierKnot(grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0]), TangentMode.AutoSmooth);
        drawnSplineExtrude.Rebuild();
        
        startPositionImage.rectTransform.anchoredPosition = new Vector2(
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0].x * canvasSplineScale,
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0].y * canvasSplineScale);
        endPositionImage.rectTransform.anchoredPosition = new Vector2(
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[^1].x * canvasSplineScale,
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[^1].y * canvasSplineScale);
        currentPositionImage.rectTransform.anchoredPosition = new Vector2(
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0].x * canvasSplineScale,
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0].y * canvasSplineScale);
        
        // Countertop
        CurrentGrindingCountertopBehaviour.DisableInteract();
        
        // Inputs
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnableGrindingHapticChallengeInputs();
        
        // Camera
        _previousCameraPreset = CameraController.instance.TargetCamSettings;
        CameraController.instance.ApplyScriptableCamSettings(grindingChallengeCameraPreset, grindingCameraTransitionTime);

        // Character
        transform.position = CurrentGrindingCountertopBehaviour.transform.position + characterGrindingPosition;
        transform.rotation = CurrentGrindingCountertopBehaviour.transform.rotation * Quaternion.Euler(characterGrindingRotation);
        
        // Animation
        characterAnimator.SetBool(IsGrinding, true);
        mortarGameObject.SetActive(true);
    }
    
    public void StopGrindingChallenge()
    {
        if (!_isChallengeActive) return;
        
        _isChallengeActive = false;

        grindingHapticChallengeGameObject.SetActive(false);
        
        CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, grindingCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        // CurrentGrindingCountertopBehaviour.EnableInteract();
        characterAnimator.SetBool(IsGrinding, false);
        mortarGameObject.SetActive(false);
        CurrentGrindingCountertopBehaviour.GrindIngredient(grindingHapticChallengeSo);
    }
    
    private void UpdateGrindingChallenge()
    {
        if (!ProcessInputGrindingChallenge()) return;

        if (CheckEndGrindingChallenge()) return;
        
        _currentTime += Time.deltaTime;
        
        if (_currentTime >= 1f / grindingHapticChallengeSo.DrawnPositionsSaveRate)
        {
            _currentTime = 0f;
            drawnSplineContainer.Spline.Add(new BezierKnot(
                (Vector3)(currentPositionImage.rectTransform.anchoredPosition / 
                          canvasSplineScale)), TangentMode.AutoSmooth);
            drawnSplineExtrude.Rebuild();
        }
    }

    private bool ProcessInputGrindingChallenge()
    {
        if (JoystickInputValue == Vector2.zero)
        {
            characterAnimator.SetFloat(GrindSpeed, grindingHapticChallengeSo.GrindingAnimationMinSpeed);
            return false;
        }
        
        characterAnimator.SetFloat(GrindSpeed, 1f);
        
        currentPositionImage.rectTransform.anchoredPosition += JoystickInputValue.normalized *
            (grindingHapticChallengeSo.CursorSpeed * Time.deltaTime);
        
        return true;
    }

    private bool CheckEndGrindingChallenge()
    {
        if (Vector2.Distance(currentPositionImage.rectTransform.anchoredPosition,
            endPositionImage.rectTransform.anchoredPosition) <= grindingHapticChallengeSo.EndPointDistanceTolerance)
        {
            StopGrindingChallenge();
            return true;
        }
        
        return false;
    }
}
