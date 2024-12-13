using UnityEngine;

public class PotionBasketBehaviour : BasketBehaviour, IPotionAddable
{
    [SerializeField] private CollectedPotionBehaviour collectedPotionBehaviourPrefab;
    [SerializeField] private int potionBasketIndex;
    public int OrderIndex { get; private set; }
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    

    public void SetBasketContent(int orderIndex)
    {
        if (meshParentTransform.childCount > 0)
        {
            Destroy(meshParentTransform.GetChild(0).gameObject);
        }
        
        OrderIndex = orderIndex;
        
        if (GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][potionBasketIndex] != null)
        {
            Instantiate(GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][potionBasketIndex].MeshGameObject,
                meshParentTransform);
        }
    }

    public void AddPotion(CollectedPotionBehaviour collectedPotionBehaviour)
    {
        GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][potionBasketIndex] = collectedPotionBehaviour.PotionValuesSo;
        Instantiate(collectedPotionBehaviour.PotionValuesSo.MeshGameObject, meshParentTransform);
        Destroy(collectedPotionBehaviour.gameObject);
    }

    public CollectedPotionBehaviour InstantiateCollectedPotion()
    {
        CollectedPotionBehaviour collectedPotionBehaviour = Instantiate(collectedPotionBehaviourPrefab, transform);
        collectedPotionBehaviour.PotionValuesSo = GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][potionBasketIndex];
        GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][potionBasketIndex] = null;

        Destroy(meshParentTransform.GetChild(0).gameObject);
        
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
            else if (characterInteractController.collectedStack.Count == 0 &&
                     GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][potionBasketIndex] != null)
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
}
