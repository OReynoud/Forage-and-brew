using NaughtyAttributes;
using UnityEngine;

public class BinBehaviour : MonoBehaviour, IPotionAddable
{
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private Animator binAnimator;
    [field: SerializeField] public bool UseEndPoint { get; set; }
    [field: ShowIf("UseEndPoint")][field: SerializeField] public Transform EndPoint { get; set; }
    [field: ShowIf("UseEndPoint")][field: SerializeField] public float heightShove { get; set; }
    
    private static readonly int DoThrowAway = Animator.StringToHash("DoThrowAway");
    
    
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
        collectedPotionBehaviour.OnPotionDropEnd.AddListener(DestroyPotion);
        
        binAnimator.SetTrigger(DoThrowAway);
    }
    
    private void DestroyPotion(CollectedPotionBehaviour collectedPotionBehaviour)
    {
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
