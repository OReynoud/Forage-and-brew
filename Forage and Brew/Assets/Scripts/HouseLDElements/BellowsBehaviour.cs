using UnityEngine;

public class BellowsBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject interactInputCanvasGameObject;

    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
    }


    private void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    private void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out StirHapticChallengeManager stirHapticChallengeManager))
        {
            // characterInteractController.CurrentNearCauldron = this;
            // stirHapticChallengeManager.CurrentCauldron = this;
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out StirHapticChallengeManager stirHapticChallengeManager) /*&&
            characterInteractController.CurrentNearCauldron == this*/)
        {
            // characterInteractController.CurrentNearCauldron = null;
            // stirHapticChallengeManager.CurrentCauldron = null;
            DisableInteract();
        }
    }
}
