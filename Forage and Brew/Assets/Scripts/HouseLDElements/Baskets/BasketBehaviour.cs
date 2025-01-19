using UnityEngine;

public class BasketBehaviour : MonoBehaviour
{
    [SerializeField] protected Transform meshParentTransform;
    [SerializeField] protected GameObject meshGameObject;
    [SerializeField] protected Collider basketCollider;
    [SerializeField] protected Collider basketTrigger;
    [SerializeField] protected GameObject interactInputCanvasGameObject;
    [SerializeField] protected GameObject cancelInputCanvasGameObject;
    [field: SerializeField] public BasketVfxManager BasketVfxManager { get; private set; }
    public bool DoesNeedToCheckAvailability { get; set; }
    
    public bool IsEnabled { get; private set; } = true;
    private bool _hasToBeDisabled;
    private bool _hasToBeEnabled;
    private float _currentTimeLeft;
    
    
    private void Update()
    {
        if (_hasToBeDisabled || _hasToBeEnabled)
        {
            _currentTimeLeft -= Time.deltaTime;

            if (_currentTimeLeft > 0f) return;
            
            meshParentTransform.gameObject.SetActive(_hasToBeEnabled);
            meshGameObject.SetActive(_hasToBeEnabled);
            basketCollider.enabled = _hasToBeEnabled;
            basketTrigger.enabled = _hasToBeEnabled;
            IsEnabled = _hasToBeEnabled;
            _hasToBeDisabled = false;
            _hasToBeEnabled = false;
        }
    }


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
        _hasToBeEnabled = true;
        StopDisable();
        _currentTimeLeft = duration;
    }
    
    public void StopEnable()
    {
        _hasToBeEnabled = false;
    }
    
    public void StartDisable(float duration)
    {
        _hasToBeDisabled = true;
        StopEnable();
        _currentTimeLeft = duration;
    }
    
    public void StopDisable()
    {
        _hasToBeDisabled = false;
    }
}
