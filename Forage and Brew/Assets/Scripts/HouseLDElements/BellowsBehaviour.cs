using UnityEngine;

public class BellowsBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private Animator bellowsAnimator;
    
    [SerializeField] private float pushSpeedOutsideTemperatureChallenge = 0.5f;
    [SerializeField] private float pushSpeedInsideTemperatureChallenge = 2f;
    
    // Animator Hashes
    private static readonly int DoPushBellows = Animator.StringToHash("DoPushBellows");
    private static readonly int IsInTemperatureChallenge = Animator.StringToHash("IsInTemperatureChallenge");
    private static readonly int PushSpeed = Animator.StringToHash("PushSpeed");


    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        bellowsAnimator.SetFloat(PushSpeed, pushSpeedOutsideTemperatureChallenge);
    }


    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    
    public void PlayBellowsAnimation()
    {
        bellowsAnimator.SetTrigger(DoPushBellows);
    }
    
    public void EnterTemperatureHapticChallenge()
    {
        bellowsAnimator.SetBool(IsInTemperatureChallenge, true);
        bellowsAnimator.SetFloat(PushSpeed, pushSpeedInsideTemperatureChallenge);
    }
    
    public void ExitTemperatureHapticChallenge()
    {
        bellowsAnimator.SetBool(IsInTemperatureChallenge, false);
        bellowsAnimator.SetFloat(PushSpeed, pushSpeedOutsideTemperatureChallenge);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (GameDontDestroyOnLoadManager.Instance.DayPassed == 0 ||
            (GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay == TimeOfDay.Daytime &&
             GameDontDestroyOnLoadManager.Instance.DayPassed == 1)) return;
        
        if (other.TryGetComponent(out TemperatureHapticChallengeManager temperatureHapticChallengeManager))
        {
            temperatureHapticChallengeManager.CurrentBellows = this;
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out TemperatureHapticChallengeManager temperatureHapticChallengeManager) &&
            temperatureHapticChallengeManager.CurrentBellows == this)
        {
            temperatureHapticChallengeManager.CurrentBellows = null;
            DisableInteract();
        }
    }
}
