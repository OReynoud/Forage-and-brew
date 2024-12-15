using UnityEngine;

public class BasketBehaviour : MonoBehaviour
{
    [SerializeField] protected Transform meshParentTransform;
    [SerializeField] protected GameObject interactInputCanvasGameObject;
    [SerializeField] protected GameObject cancelInputCanvasGameObject;
    public bool DoesNeedToCheckAvailability { get; set; }


    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }


    public void EnableCancel()
    {
        cancelInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableCancel()
    {
        cancelInputCanvasGameObject.SetActive(false);
    }
}
