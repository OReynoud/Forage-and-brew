using UnityEngine;

public abstract class BasketBehaviour : MonoBehaviour
{
    [SerializeField] protected Transform meshParentTransform;
    [SerializeField] protected GameObject meshGameObject;
    [SerializeField] protected Collider basketCollider;
    [SerializeField] protected Collider basketTrigger;
    [SerializeField] protected GameObject interactInputCanvasGameObject;
    [SerializeField] protected GameObject cancelInputCanvasGameObject;
    [SerializeField] protected GameObject ingredientLocalCanvasGameObject;
    [field: SerializeField] public BasketVfxManager BasketVfxManager { get; private set; }
    public bool DoesNeedToCheckAvailability { get; set; }
    
    public bool IsEnabled { get; protected set; } = true;
    protected bool HasToBeDisabled;
    protected bool HasToBeEnabled;
    protected float CurrentTimeLeft;


    protected abstract void OnDisable();


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
    
    
    public void StartEnable(float duration)
    {
        HasToBeEnabled = true;
        StopDisable();
        CurrentTimeLeft = duration;
    }
    
    public void StopEnable()
    {
        HasToBeEnabled = false;
    }
    
    public void StartDisable(float duration)
    {
        if (!IsEnabled) return; // If already disabled, do nothing
        
        OnDisable();
        HasToBeDisabled = true;
        StopEnable();
        CurrentTimeLeft = duration;
    }
    
    public void StopDisable()
    {
        HasToBeDisabled = false;
    }
}
