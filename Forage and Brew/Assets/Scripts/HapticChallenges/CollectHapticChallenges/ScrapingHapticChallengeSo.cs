using UnityEngine;

[CreateAssetMenu(fileName = "D_ScrapingHapticChallenge", menuName = "Haptic Challenges/ScrapingHapticChallengeSo")]
public class ScrapingHapticChallengeSo : CollectHapticChallengeSo
{
    [field: SerializeField] [field: Tooltip("The tolerance for the joystick's magnitude to be considered as doing a scraping action.")] [field: Range(0f, 1f)]
    public float JoystickMagnitudeTolerance { get; private set; }
    
    [field: SerializeField] [field: Tooltip("The angle needed to be traveled to validate the challenge.")] [field: Range(0f, 180f)]
    public float AngleToTravel { get; private set; }
}
