using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Events;

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
        SceneTransitionManager.instance.HandleGoingToSleepTransition(1f);
        // Letters
        GameDontDestroyOnLoadManager.Instance.HasChosenLettersToday = false;
        
        // Orders
        OrderManager.Instance.AddOrdersToValidate();
        OrderManager.Instance.CheckOrdersToValidate();
        foreach (PotionBasketManagerBehaviour potionBasketManagerBehaviour in potionBasketManagerBehaviours)
        {
            potionBasketManagerBehaviour.ReactivateRightPotionBaskets();
        }

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
