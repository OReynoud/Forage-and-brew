using UnityEngine;

[CreateAssetMenu(fileName = "D_GaugeHapticChallenge", menuName = "Haptic Challenges/GaugeHapticChallengeSo")]
public class GaugeHapticChallengeSo : HapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The total height of the gauge (in pixels, based on 1920x1080 screen).")]
    [field: Min(0f)] public float GaugeTotalHeight { get; private set; } = 3f;
    
    [field: SerializeField] [field: Tooltip("The part of the gauge considered as correct (including perfect part).")]
    [field: Range(0f, 1f)] public float CorrectGaugePart { get; private set; } = 0.67f;
    
    [field: SerializeField] [field: Tooltip("The part of the gauge considered as perfect.")]
    [field: Range(0f, 1f)] public float PerfectGaugePart { get; private set; } = 0.33f;
    
    [field: SerializeField] [field: Tooltip("The speed of the arrow (in pixels per second, based on 1920x1080 screen).")]
    [field: Min(0f)] public float ArrowSpeed { get; private set; } = 2f;
}
