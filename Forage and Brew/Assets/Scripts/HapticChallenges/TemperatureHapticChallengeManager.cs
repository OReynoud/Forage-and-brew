using UnityEngine;
using UnityEngine.UI;

public class TemperatureHapticChallengeManager : MonoBehaviour
{
    // Singleton
    public static StirHapticChallengeManager Instance { get; private set; }
    
    [Header("Dependencies")]
    // [SerializeField] private StirHapticChallengeGlobalValuesSo stirHapticChallengeGlobalValuesSo;
    
    [Header("UI")]
    [SerializeField] private GameObject temperatureChallengeGameObject;
    [SerializeField] private RectTransform gaugeArrowRectTransform;
    [SerializeField] private Image gaugeImage;
    
    [Header("Camera")]
    [SerializeField] private CameraPreset stirChallengeCameraPreset;
    [SerializeField] private float cauldronCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
}
