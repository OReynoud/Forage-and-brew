using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class GrindingHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static GrindingHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private GrindingHapticChallengeSo grindingHapticChallengeSo;
    
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
    
    public Vector2 JoystickInputValue { get; set; }
    private bool _isChallengeActive;
    private int _currentRouteIndex;
    private float _currentTime;
    
    
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
    
    
    private void StartGrindingChallenge()
    {
        grindingHapticChallengeGameObject.SetActive(true);
        _isChallengeActive = true;
        _currentRouteIndex = Random.Range(0, grindingHapticChallengeSo.Routes.Count);
        _currentTime = 0f;
        
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
    }
    
    public void StopGrindingChallenge()
    {
        if (!_isChallengeActive) return;
        
        grindingHapticChallengeGameObject.SetActive(false);
        _isChallengeActive = false;
        // StopCollectHapticChallenge(); TODO: Implement this method
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
        if (JoystickInputValue == Vector2.zero) return false;
        
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
