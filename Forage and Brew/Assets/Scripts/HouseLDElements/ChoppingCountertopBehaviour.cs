using UnityEngine;

public class ChoppingCountertopBehaviour : MonoBehaviour
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
        if (other.TryGetComponent(out ChoppingHapticChallengeManager cookHapticChallengeManager))
        {
            cookHapticChallengeManager.CurrentChoppingCountertopBehaviour = this;
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ChoppingHapticChallengeManager cookHapticChallengeManager) &&
            cookHapticChallengeManager.CurrentChoppingCountertopBehaviour == this)
        {
            cookHapticChallengeManager.CurrentChoppingCountertopBehaviour = null;
            DisableInteract();
        }
    }
}
