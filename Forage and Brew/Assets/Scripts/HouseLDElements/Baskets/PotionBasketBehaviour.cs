using System;
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
        if (CharacterInteractController.Instance.CurrentNearPotionBaskets.Contains(this))
        {
            CharacterInteractController.Instance.CurrentNearPotionBaskets.Remove(this);
        }
    }


    public void SetBasketContent(int orderIndex)
    {
        if (meshParentTransform.childCount > 0)
        {
            Destroy(meshParentTransform.GetChild(0).gameObject);
        }
        
        OrderIndex = orderIndex;
        
        if (GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex] != null)
        {
            Instantiate(GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex].MeshGameObject,
                meshParentTransform);
        }
    }

    public void AddPotion(CollectedPotionBehaviour collectedPotionBehaviour)
    {
        GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex] = collectedPotionBehaviour.PotionValuesSo;
        Instantiate(collectedPotionBehaviour.PotionValuesSo.MeshGameObject, meshParentTransform);
        Destroy(collectedPotionBehaviour.gameObject);
    }

    public CollectedPotionBehaviour InstantiateCollectedPotion()
    {
        CollectedPotionBehaviour collectedPotionBehaviour = Instantiate(collectedPotionBehaviourPrefab, transform);
        collectedPotionBehaviour.PotionValuesSo = GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex];
        GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex] = null;

        Destroy(meshParentTransform.GetChild(0).gameObject);
        
        return collectedPotionBehaviour;
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            PotionBasketManagerBehaviour.ManageTriggerEnter();
            
            characterInteractController.CurrentNearPotionBaskets.Add(this);
            
            if (characterInteractController.collectedStack.Count > 0 &&
                (CollectedPotionBehaviour)characterInteractController.collectedStack[0].stackable)
            {
                EnableInteract();
            }
            else if (characterInteractController.collectedStack.Count == 0 &&
                     GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex] != null)
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
                (CollectedPotionBehaviour)characterInteractController.collectedStack[0].stackable)
            {
                EnableInteract();
                DisableCancel();
            }
            else if (characterInteractController.collectedStack.Count == 0 &&
                     GameDontDestroyOnLoadManager.Instance.OrderPotions[OrderIndex][PotionBasketIndex] != null)
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
            PotionBasketManagerBehaviour.ManageTriggerExit();
            
            if (characterInteractController.CurrentNearPotionBaskets.Contains(this))
            {
                characterInteractController.CurrentNearPotionBaskets.Remove(this);
                DisableInteract();
                DisableCancel();
            }
        }
    }
}
