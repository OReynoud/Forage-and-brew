using UnityEngine;

public class BedBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject interactInputCanvasGameObject;


    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        GameDontDestroyOnLoadManager.Instance.generateLetters = true;
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
        GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay = TimeOfDay.Daytime;
        GameDontDestroyOnLoadManager.Instance.generateLetters = true;
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
