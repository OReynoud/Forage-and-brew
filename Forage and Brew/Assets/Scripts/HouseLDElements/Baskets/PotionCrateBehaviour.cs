using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PotionCrateBehaviour : MonoBehaviour, IPotionAddable
{
    private static readonly int DoEnable = Animator.StringToHash("DoEnable");
    private static readonly int DoComplete = Animator.StringToHash("DoComplete");

    [Header("Dependencies")]
    [SerializeField] private CollectedPotionBehaviour collectedPotionBehaviourPrefab;
    [SerializeField] private Transform meshParentTransform;
    [SerializeField] private GameObject closingColliderObject;
    [SerializeField] private Animator potionCrateAnimator;
    
    public PotionCrateManager PotionCrateManager { get; set; }
    public OrderContentSo OrderContentSo { get; private set; }
    public ClientSo ClientSo { get; private set; }
    
    [Header("Closing Collider")]
    [SerializeField] private float closingColliderOffset = 2f;
    [SerializeField] private float closingColliderDuration = 1f;
    [SerializeField] private AnimationCurve closingColliderCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    private Vector3 _closingColliderDefaultLocalPosition;
    
    public bool DoesNeedToCheckAvailability { get; set; }
    public bool IsFulfilled { get; private set; }
    public List<CollectedPotionBehaviour> ContainedPotions { get; set; } = new();
    
    [Header("UI")]
    [SerializeField] private GameObject interactInputCanvasGameObject;
    
    [SerializeField] private List<PotionCrateLidLayoutBehaviour> lidLayoutBehaviours;
    private PotionCrateLidLayoutBehaviour _currentLidLayoutBehaviour;
    
    [SerializeField] private GameObject clientCanvasGameObject;
    [SerializeField] private TMP_Text clientNameText;
    [SerializeField] private GameObject clientCheckmarkGameObject;
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        
        _closingColliderDefaultLocalPosition = closingColliderObject.transform.localPosition;
    }
    
    private void OnDisable()
    {
        if (CharacterInteractController.Instance.CurrentNearPotionBaskets.Contains(this))
        {
            CharacterInteractController.Instance.CurrentNearPotionBaskets.Remove(this);
        }
        
        DisableInteract();
    }


    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }


    public void EnableCrate(OrderContentSo orderContentSo, ClientSo clientSo)
    {
        gameObject.SetActive(true);
        
        OrderContentSo = orderContentSo;
        ClientSo = clientSo;
        
        foreach (PotionCrateLidLayoutBehaviour lidLayoutBehaviour in lidLayoutBehaviours)
        {
            lidLayoutBehaviour.gameObject.SetActive(false);
        }
        _currentLidLayoutBehaviour = lidLayoutBehaviours[orderContentSo.RequestedPotions.Length-1];
        _currentLidLayoutBehaviour.gameObject.SetActive(true);
        _currentLidLayoutBehaviour.SetLetterImageColor(clientSo.AssociatedColor);
        
        int orderIndex = GameDontDestroyOnLoadManager.Instance.OrderPotions.FindIndex(x => x != null && x.OrderSo == OrderContentSo);
        if (orderIndex >= 0)
        {
            List<FloorCookedPotion> floorCookedPotions = GameDontDestroyOnLoadManager.Instance.OrderPotions[orderIndex]
                .Potions.ToList();
            OutStackableManager.Instance.InstantiateOutCookedPotions(floorCookedPotions, ContainedPotions);
            GameDontDestroyOnLoadManager.Instance.OrderPotions[orderIndex] = null;

            foreach (CollectedPotionBehaviour potion in ContainedPotions)
            {
                potion.DisableInteraction();
                int potionIndex = orderContentSo.RequestedPotions.ToList().FindIndex(x => 
                    (x.IsSpecific && x.Potion == potion.PotionValuesSo) ||
                    (!x.IsSpecific && (x.ValidTag & potion.PotionValuesSo.effectiveTags) != 0));
                _currentLidLayoutBehaviour.EnablePotionCheckMark(potionIndex);
            }
        }
        
        // clientNameText.text = ClientSo.Name;
        
        DoesNeedToCheckAvailability = true;
        
        potionCrateAnimator.SetTrigger(DoEnable);
        
        CheckCompletion();
    }

    public void DisableCrate()
    {
        gameObject.SetActive(false);
        ResetCrateContent();
        DoesNeedToCheckAvailability = true;
        IsFulfilled = false;
        foreach (PotionCrateLidLayoutBehaviour lidLayoutBehaviour in lidLayoutBehaviours)
        {
            lidLayoutBehaviour.DisablePotionCheckMarks();
        }
        // clientCheckmarkGameObject.SetActive(false);
    }


    public void ResetCrateContent()
    {
        for (int i = 0; i < meshParentTransform.childCount; i++)
        {
            Destroy(meshParentTransform.GetChild(i).gameObject);
        }
        
        OrderContentSo = null;
        ClientSo = null;
        ContainedPotions.Clear();
    }

    public void AddPotion(CollectedPotionBehaviour collectedPotionBehaviour)
    {
        ContainedPotions.Add(collectedPotionBehaviour);
        GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Remove(collectedPotionBehaviour);
        
        collectedPotionBehaviour.EnablePhysics();
        collectedPotionBehaviour.transform.SetParent(meshParentTransform);
        CloseCollider();
        
        int potionIndex = OrderContentSo.RequestedPotions.ToList().FindIndex(x => 
            (x.IsSpecific && x.Potion == collectedPotionBehaviour.PotionValuesSo) ||
            (!x.IsSpecific && (x.ValidTag & collectedPotionBehaviour.PotionValuesSo.effectiveTags) != 0));
        _currentLidLayoutBehaviour.EnablePotionCheckMark(potionIndex);
        
        CheckCompletion();
    }
    
    private void CloseCollider()
    {
        closingColliderObject.SetActive(true);

        closingColliderObject.transform.DOKill();
        closingColliderObject.transform.localPosition = _closingColliderDefaultLocalPosition +
            Vector3.up * closingColliderOffset;
        
        closingColliderObject.transform.DOMoveY(_closingColliderDefaultLocalPosition.y, closingColliderDuration)
            .SetEase(closingColliderCurve).OnComplete(() =>
        {
            closingColliderObject.SetActive(false);
        });
    }
    

    public bool CheckPotion(PotionValuesSo collectedPotionSo)
    {
        if (OrderContentSo == null || IsFulfilled) return false;

        List<PotionDemand> remainingRequestedPotions = OrderContentSo.RequestedPotions.ToList();

        foreach (CollectedPotionBehaviour containedPotion in ContainedPotions)
        {
            int index = remainingRequestedPotions.FindIndex(x => x.IsSpecific &&
                x.Potion == containedPotion.PotionValuesSo);
            
            if (index < 0)
            {
                index = remainingRequestedPotions.FindIndex(x => !x.IsSpecific &&
                    (x.ValidTag & containedPotion.PotionValuesSo.effectiveTags) != 0);
            }
            
            remainingRequestedPotions.RemoveAt(index);
        }
        
        foreach (PotionDemand requestedPotion in remainingRequestedPotions)
        {
            if (requestedPotion.IsSpecific && requestedPotion.Potion == collectedPotionSo)
            {
                return true;
            }
            
            if (!requestedPotion.IsSpecific && (requestedPotion.ValidTag & collectedPotionSo.effectiveTags) != 0)
            {
                return true;
            }
        }

        return false;
    }
    
    public void CheckCompletion()
    {
        if (ContainedPotions.Count == OrderContentSo.RequestedPotions.Length)
        {
            // clientCheckmarkGameObject.SetActive(true);
            IsFulfilled = true;
        
            potionCrateAnimator.SetTrigger(DoComplete);
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.CurrentNearPotionBaskets.Add(this);
            
            if (characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedPotionBehaviour &&
                !IsFulfilled)
            {
                EnableInteract();
            } }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!DoesNeedToCheckAvailability) return;
        
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            if (characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedPotionBehaviour &&
                !IsFulfilled)
            {
                EnableInteract();
            }
            else
            {
                DisableInteract();
            }
            
            DoesNeedToCheckAvailability = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            if (characterInteractController.CurrentNearPotionBaskets.Contains(this))
            {
                characterInteractController.CurrentNearPotionBaskets.Remove(this);
                DisableInteract();
            }
        }
    }
}
