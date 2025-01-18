using UnityEngine;

public class PotionBasketBehaviour : BasketBehaviour, IPotionAddable
{
    [SerializeField] private CollectedPotionBehaviour collectedPotionBehaviourPrefab;
    public PotionBasketManagerBehaviour PotionBasketManagerBehaviour { get; set; }
    public int PotionBasketIndex { get; set; }
    public int OrderIndex { get; private set; }
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
    }

    private void OnDisable()
    {
        PotionBasketManagerBehaviour.ManageTriggerExit(this);
        
        if (CharacterInteractController.Instance.CurrentNearPotionBaskets.Contains(this))
        {
            CharacterInteractController.Instance.CurrentNearPotionBaskets.Remove(this);
        }
        
        DisableInteract();
        DisableCancel();
    }


    public void SetBasketContent(int orderIndex)
    {
        if (meshParentTransform.childCount > 0)
        {
            Destroy(meshParentTransform.GetChild(0).gameObject);
        }
        
        OrderIndex = orderIndex;
        
        if (GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex].Potions[PotionBasketIndex] != null)
        {
            Instantiate(GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex].Potions[PotionBasketIndex]
                    .PotionDifficulty.MeshGameObjectLiquidColorManager, meshParentTransform);
        }
    }

    public void AddPotion(CollectedPotionBehaviour collectedPotionBehaviour)
    {
        GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex].Potions[PotionBasketIndex] = collectedPotionBehaviour.PotionValuesSo;
        GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Remove(collectedPotionBehaviour);
        Instantiate(collectedPotionBehaviour.PotionValuesSo.PotionDifficulty.MeshGameObjectLiquidColorManager, meshParentTransform);
        Destroy(collectedPotionBehaviour.gameObject);
        PotionBasketManagerBehaviour.CheckCompletion();
    }

    public CollectedPotionBehaviour InstantiateCollectedPotion()
    {
        CollectedPotionBehaviour collectedPotionBehaviour = Instantiate(collectedPotionBehaviourPrefab, transform);
        collectedPotionBehaviour.PotionValuesSo = GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex].Potions[PotionBasketIndex];
        GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Add(collectedPotionBehaviour);
        GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex].Potions[PotionBasketIndex] = null;

        Destroy(meshParentTransform.GetChild(0).gameObject);
        
        return collectedPotionBehaviour;
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            PotionBasketManagerBehaviour.ManageTriggerEnter(this);
            
            characterInteractController.CurrentNearPotionBaskets.Add(this);
            
            if (characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedPotionBehaviour &&
                !GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex].Potions[PotionBasketIndex])
            {
                EnableInteract();
            }
            else if (characterInteractController.collectedStack.Count == 0 &&
                     GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex].Potions[PotionBasketIndex])
            {
                EnableCancel();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!DoesNeedToCheckAvailability) return;
        
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            if (characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedPotionBehaviour &&
                !GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex].Potions[PotionBasketIndex])
            {
                EnableInteract();
                DisableCancel();
            }
            else if (characterInteractController.collectedStack.Count == 0 &&
                     GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex].Potions[PotionBasketIndex])
            {
                EnableCancel();
                DisableInteract();
            }
            else
            {
                DisableInteract();
                DisableCancel();
            }
            
            DoesNeedToCheckAvailability = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            PotionBasketManagerBehaviour.ManageTriggerExit(this);
            
            if (characterInteractController.CurrentNearPotionBaskets.Contains(this))
            {
                characterInteractController.CurrentNearPotionBaskets.Remove(this);
                DisableInteract();
                DisableCancel();
            }
        }
    }
}
