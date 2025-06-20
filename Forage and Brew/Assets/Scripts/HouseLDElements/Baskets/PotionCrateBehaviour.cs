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
    
    [SerializeField] private GameObject lidCanvasGameObject;
    [SerializeField] private List<PotionCrateLidLayoutBehaviour> lidLayoutBehaviours;
    private PotionCrateLidLayoutBehaviour _currentLidLayoutBehaviour;
    
    [SerializeField] private GameObject popupCanvasGameObject;
    [SerializeField] private TMP_Text clientNameText;
    [SerializeField] private PotionDemandElementBehaviour potionElementPrefab;
    [SerializeField] private Transform potionElementParentTransform;
    private readonly List<PotionDemandElementBehaviour> _potionElements = new();
    [SerializeField] private TMP_Text priceText;
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        DisablePopup();
        
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
    
    
    public void EnablePopup()
    {
        popupCanvasGameObject.SetActive(true);
    }
    
    public void DisablePopup()
    {
        popupCanvasGameObject.SetActive(false);
    }


    public void EnableCrate(OrderContentSo orderContentSo, ClientSo clientSo)
    {
        if (gameObject.activeSelf) return;
        
        gameObject.SetActive(true);
        
        OrderContentSo = orderContentSo;
        ClientSo = clientSo;
        
        lidCanvasGameObject.SetActive(true);
        foreach (PotionCrateLidLayoutBehaviour lidLayoutBehaviour in lidLayoutBehaviours)
        {
            lidLayoutBehaviour.gameObject.SetActive(false);
        }
        _currentLidLayoutBehaviour = lidLayoutBehaviours[orderContentSo.RequestedPotions.Length-1];
        _currentLidLayoutBehaviour.gameObject.SetActive(true);
        _currentLidLayoutBehaviour.SetLetterImageColor(clientSo.AssociatedColor);
        
        clientNameText.text = ClientSo.Name;
        
        for (int i = 0; i < OrderContentSo.RequestedPotions.Length; i++)
        {
            PotionDemandElementBehaviour elementBehaviour = Instantiate(potionElementPrefab, potionElementParentTransform);
            
            elementBehaviour.SetImage(OrderContentSo.RequestedPotions[i]);
            
            _potionElements.Add(elementBehaviour);
        }

        priceText.text = OrderContentSo.MoneyReward.ToString();
        
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
                    (!x.IsSpecific && potion.PotionValuesSo.Tags.Any(t => t.InducedTags.Contains(x.ValidTag))));
                _currentLidLayoutBehaviour.EnablePotionCheckMark(potionIndex);
                _potionElements[potionIndex].EnableCheckMark();
            }
        }
        
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
        
        foreach (PotionDemandElementBehaviour potionElement in _potionElements)
        {
            Destroy(potionElement.gameObject);
        }
        _potionElements.Clear();
        
        foreach (PotionCrateLidLayoutBehaviour lidLayoutBehaviour in lidLayoutBehaviours)
        {
            lidLayoutBehaviour.DisablePotionCheckMarks();
        }
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
            (!x.IsSpecific && collectedPotionBehaviour.PotionValuesSo.Tags.Any(t => t.InducedTags.Contains(x.ValidTag))));
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

        return true;
        
        List<PotionDemand> remainingRequestedPotions = OrderContentSo.RequestedPotions.ToList();

        foreach (CollectedPotionBehaviour containedPotion in ContainedPotions)
        {
            int index = remainingRequestedPotions.FindIndex(x => x.IsSpecific &&
                x.Potion == containedPotion.PotionValuesSo);
            
            if (index < 0)
            {
                index = remainingRequestedPotions.FindIndex(x => !x.IsSpecific &&
                    collectedPotionSo.Tags.Any(t => t.InducedTags.Contains(x.ValidTag)));
            }
            
            remainingRequestedPotions.RemoveAt(index);
        }
        
        foreach (PotionDemand requestedPotion in remainingRequestedPotions)
        {
            if (requestedPotion.IsSpecific && requestedPotion.Potion == collectedPotionSo)
            {
                return true;
            }
            
            if (!requestedPotion.IsSpecific &&
                collectedPotionSo.Tags.Any(t => t.InducedTags.Contains(requestedPotion.ValidTag)))
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
            lidCanvasGameObject.SetActive(false);
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        PotionCrateManager.ManageTriggerEnter(this);
        
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.CurrentNearPotionBaskets.Add(this);
            
            if (characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedPotionBehaviour &&
                !IsFulfilled)
            {
                EnableInteract();
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
        PotionCrateManager.ManageTriggerExit(this);
        
        DisablePopup();
        
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
