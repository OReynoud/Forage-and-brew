using UnityEngine;

public class BedBehaviour : MonoBehaviour
{
    [SerializeField] private PotionBasketManagerBehaviour[] potionBasketManagerBehaviours;
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
    

    public void Sleep()
    {
        // Time
        GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay = TimeOfDay.Daytime;
        GameDontDestroyOnLoadManager.Instance.DayPassed++;
        InfoDisplayManager.instance.DisplayDays();
        
        // Letters
        GameDontDestroyOnLoadManager.Instance.HasChosenLettersToday = false;
        
        // Orders
        OrderManager.Instance.CheckOrdersToValidate();
        foreach (PotionBasketManagerBehaviour potionBasketManagerBehaviour in potionBasketManagerBehaviours)
        {
            potionBasketManagerBehaviour.ReactivateRightPotionBaskets();
        }
        
        // Cycles
        WeatherManager.Instance.PassToNextWeatherState();
        LunarCycleManager.Instance.PassToNextLunarCycleState();
        
        Debug.Log("It's daytime now");
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
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
