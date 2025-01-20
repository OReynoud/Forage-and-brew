using UnityEngine;

public class BedBehaviour : MonoBehaviour
{
    [SerializeField] private PotionBasketManagerBehaviour[] potionBasketManagerBehaviours;
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private Transform bedSpawnPoint;


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
    

    public void Sleep()
    {
        SceneTransitionManager.instance.HandleGoingToSleepTransition(bedSpawnPoint);
        
        // Ingredients to Collect
        GameDontDestroyOnLoadManager.Instance.HasChosenIngredientsToday = false;
        GameDontDestroyOnLoadManager.Instance.RemainingIngredientToCollectBehaviours.Clear();
        
        // Letters
        GameDontDestroyOnLoadManager.Instance.HasChosenLettersToday = false;
        
        // Orders
        OrderManager.Instance.AddOrdersToValidate();
        OrderManager.Instance.CheckOrdersToValidate();
        foreach (PotionBasketManagerBehaviour potionBasketManagerBehaviour in potionBasketManagerBehaviours)
        {
            potionBasketManagerBehaviour.ReactivateRightPotionBaskets();
        }
        
        CharacterInteractController.Instance.CurrentNearBed = null;
        DisableInteract();
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay == TimeOfDay.Daytime) return;
        
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.CurrentNearBed = this;
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.CurrentNearBed = null;
            DisableInteract();
        }
    }
}
