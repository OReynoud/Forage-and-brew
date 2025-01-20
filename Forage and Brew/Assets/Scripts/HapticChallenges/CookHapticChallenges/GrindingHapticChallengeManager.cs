using System.Collections.Generic;
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
    [SerializeField] private GameObject splinesGameObject;
    [SerializeField] private GameObject grindingHapticChallengeGameObject;
    [SerializeField] private SplineContainer previewSplineContainer;
    [SerializeField] private SplineExtrude previewSplineExtrude;
    [SerializeField] private SplineContainer drawnSplineContainer;
    [SerializeField] private SplineExtrude drawnSplineExtrude;
    [SerializeField] private Image startPositionImage;
    [SerializeField] private Image endPositionImage;
    [SerializeField] private Image currentPositionImage;
    [SerializeField] private float canvasSplineScale = 108f;
    [SerializeField] private List<RectTransform> crushInput1RectTransforms;
    [SerializeField] private List<RectTransform> crushInput2RectTransforms;
    private readonly List<GrindingCrushInputBehaviour> _crushInput1Behaviours = new();
    private readonly List<GrindingCrushInputBehaviour> _crushInput2Behaviours = new();
    private int _currentCrushInput1Index;
    private int _currentCrushInput2Index;
    
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
        foreach (RectTransform crushInputRectTransform in crushInput1RectTransforms)
        {
            _crushInput1Behaviours.Add(crushInputRectTransform.GetComponent<GrindingCrushInputBehaviour>());
        }

        foreach (RectTransform crushInputRectTransform in crushInput2RectTransforms)
        {
            _crushInput2Behaviours.Add(crushInputRectTransform.GetComponent<GrindingCrushInputBehaviour>());
        }
        
        grindingHapticChallengeGameObject.SetActive(false);
        splinesGameObject.SetActive(false);
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
        splinesGameObject.SetActive(true);
        
        // Splines
        previewSplineContainer.Spline.Clear();
        foreach (Vector3 point in grindingHapticChallengeSo.Routes[_currentRouteIndex].Points)
        {
            previewSplineContainer.Spline.Add(new BezierKnot(point), TangentMode.AutoSmooth);
        }
        previewSplineExtrude.Rebuild();
        
        drawnSplineContainer.Spline.Clear();
        drawnSplineContainer.Spline.Add(new BezierKnot(grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0]), TangentMode.AutoSmooth);
        drawnSplineExtrude.Rebuild();
        
        // Position Points
        startPositionImage.rectTransform.anchoredPosition = new Vector2(
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0].x * canvasSplineScale,
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0].y * canvasSplineScale);
        endPositionImage.rectTransform.anchoredPosition = new Vector2(
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[^1].x * canvasSplineScale,
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[^1].y * canvasSplineScale);
        currentPositionImage.rectTransform.anchoredPosition = new Vector2(
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0].x * canvasSplineScale,
            grindingHapticChallengeSo.Routes[_currentRouteIndex].Points[0].y * canvasSplineScale);
        
        // Crush Inputs
        foreach (RectTransform crushInputRectTransform in crushInput1RectTransforms)
        {
            crushInputRectTransform.gameObject.SetActive(false);
        }
        
        foreach (RectTransform crushInputRectTransform in crushInput2RectTransforms)
        {
            crushInputRectTransform.gameObject.SetActive(false);
        }
        
        _currentCrushInput1Index = 0;
        _currentCrushInput2Index = 0;
        
        foreach (GrindingHapticChallengeCrushInput crushInput in grindingHapticChallengeSo.Routes[_currentRouteIndex].CrushInputs)
        {
            switch (crushInput.Input)
            {
                case 1:
                    crushInput1RectTransforms[_currentCrushInput1Index].anchoredPosition = crushInput.Position;
                    crushInput1RectTransforms[_currentCrushInput1Index].gameObject.SetActive(true);
                    _currentCrushInput1Index++;
                    break;
                case 2:
                    crushInput2RectTransforms[_currentCrushInput2Index].anchoredPosition = crushInput.Position;
                    crushInput2RectTransforms[_currentCrushInput2Index].gameObject.SetActive(true);
                    _currentCrushInput2Index++;
                    break;
            }
        }
        
        // Countertop
        CurrentGrindingCountertopBehaviour.DisableInteract();
        
        // Inputs
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnableGrindingHapticChallengeInputs();
        
        // Camera
        _previousCameraPreset = CameraController.instance.TargetCamSettings;
        CameraController.instance.ApplyScriptableCamSettings(grindingChallengeCameraPreset, grindingCameraTransitionTime);

        // Character
        transform.position = CurrentGrindingCountertopBehaviour.transform.position +
                             CurrentGrindingCountertopBehaviour.transform.rotation * characterGrindingPosition;
        transform.rotation = CurrentGrindingCountertopBehaviour.transform.rotation * Quaternion.Euler(characterGrindingRotation);
        
        // Animation
        characterAnimator.SetBool(IsGrinding, true);
        mortarGameObject.SetActive(true);
        
        // Audio
        CurrentGrindingCountertopBehaviour.EnterGrindingChallenge();
                
        // Put in HapticChallenge
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = true;
    }
    
    public void StopGrindingChallenge()
    {
        if (!_isChallengeActive) return;
        
        _isChallengeActive = false;

        grindingHapticChallengeGameObject.SetActive(false);
        splinesGameObject.SetActive(false);
        
        CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, grindingCameraTransitionTime);
        CharacterInputManager.Instance.EnableInputs();
        // CurrentGrindingCountertopBehaviour.EnableInteract();
        characterAnimator.SetBool(IsGrinding, false);
        mortarGameObject.SetActive(false);
        CurrentGrindingCountertopBehaviour.ExitGrindingChallenge();
        CurrentGrindingCountertopBehaviour.GrindIngredient(grindingHapticChallengeSo);    
        
        GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge = false;
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

    public void CheckInputGrindingChallenge(int inputIndex)
    {
        if (!_isChallengeActive) return;

        switch (inputIndex)
        {
            case 1:
                CheckCrushInput(crushInput1RectTransforms, _crushInput1Behaviours);
                break;
            case 2:
                CheckCrushInput(crushInput2RectTransforms, _crushInput2Behaviours);
                break;
        }
    }

    private void CheckCrushInput(List<RectTransform> crushInputRectTransforms, List<GrindingCrushInputBehaviour> crushInputBehaviours)
    {
        for (int i = 0; i < crushInputRectTransforms.Count; i++)
        {
            RectTransform crushInputRectTransform = crushInputRectTransforms[i];
            GrindingCrushInputBehaviour crushInputBehaviour = crushInputBehaviours[i];

            if (!crushInputRectTransform.gameObject.activeSelf) continue;
            
            if (!(Vector2.Distance(currentPositionImage.rectTransform.anchoredPosition,
                      crushInputRectTransform.anchoredPosition) <=
                  grindingHapticChallengeSo.CrushInputDistanceTolerance)) continue;
            
            characterAnimator.SetTrigger(DoCrush);
            CurrentGrindingCountertopBehaviour.CountertopVfxManager.PlayCrushVfx();
            CurrentGrindingCountertopBehaviour.PlayCrushSound();
            crushInputBehaviour.SetCorrectInput();
            return;
        }
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
