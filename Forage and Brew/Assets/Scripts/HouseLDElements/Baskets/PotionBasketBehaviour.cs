using UnityEngine;

public class PotionBasketBehaviour : BasketBehaviour, IPotionAddable
{
    [SerializeField] private CollectedPotionBehaviour collectedPotionBehaviourPrefab;
    public int OrderIndex { get; set; }
    public int PotionBasketIndex { get; set; }
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        
        if (GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex] != null)
        {
            Instantiate(GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex].MeshGameObject,
                meshParentTransform);
        }
    }
    
    
    public CollectedPotionBehaviour InstantiateCollectedPotion()
    {
        CollectedPotionBehaviour collectedPotionBehaviour = Instantiate(collectedPotionBehaviourPrefab, transform);
        collectedPotionBehaviour.PotionValuesSo = GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex];

        meshParentTransform.gameObject.SetActive(false);
        
        return collectedPotionBehaviour;
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            if (characterInteractController.collectedStack.Count > 0 &&
                (CollectedPotionBehaviour)characterInteractController.collectedStack[0].stackable)
            {
                characterInteractController.CurrentNearPotionBaskets.Add(this);
                EnableInteract();
            }
            else if (characterInteractController.collectedStack.Count == 0)
            {
                characterInteractController.CurrentNearPotionBaskets.Add(this);
                EnableCancel();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.CurrentNearPotionBaskets.Contains(this))
        {
            characterInteractController.CurrentNearPotionBaskets.Remove(this);
            DisableInteract();
            DisableCancel();
        }
    }

    public void AddPotion(CollectedPotionBehaviour collectedPotionBehaviour)
    {
        Destroy(collectedPotionBehaviour.gameObject);
        meshParentTransform.gameObject.SetActive(true);
    }
}
