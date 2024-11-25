using UnityEngine;

public class BellowsBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject interactInputCanvasGameObject;

    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
    }


    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
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
