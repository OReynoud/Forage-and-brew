using UnityEngine;

public class BedBehaviour : MonoBehaviour
{
    [SerializeField] private PotionCrateManager[] potionBasketManagerBehaviours;
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private Transform bedSpawnPoint;
    [SerializeField] private float timeBeforeBed = 10f;


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
        OrderManager.Instance.CheckOrdersToValidate();
        foreach (PotionCrateManager potionBasketManagerBehaviour in potionBasketManagerBehaviours)
        {
            potionBasketManagerBehaviour.ReactivateRightPotionCrates();
        }
        
        CharacterInteractController.Instance.CurrentNearBed = null;
        MailBoxBehaviour.instance.MailNewDayMethod();
        DisableInteract();
        GameDontDestroyOnLoadManager.Instance.SetTimeBeforeBed(timeBeforeBed);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (GameDontDestroyOnLoadManager.Instance.TimeBeforeBed > 0f) return;
        
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
