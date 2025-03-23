using UnityEngine;

public class BinBehaviour : MonoBehaviour, IPotionAddable
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
    
    
    public void AddPotion(CollectedPotionBehaviour collectedPotionBehaviour)
    {
        GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Remove(collectedPotionBehaviour);
        
        Destroy(collectedPotionBehaviour.gameObject);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.collectedStack.Count > 0 &&
            characterInteractController.collectedStack[0].stackable is CollectedPotionBehaviour)
        {
            characterInteractController.CurrentNearBin = this;
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.CurrentNearBin == this)
        {
            characterInteractController.CurrentNearBin = null;
            DisableInteract();
        }
    }
}
