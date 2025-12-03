using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class GateBehaviour : MonoBehaviour
{
    [SerializeField] public List<ChargedBiomeAreaSo> chargedBiomeAreaSos;
    [SerializeField] public PricePopUpBehaviour pricePopUpBehaviour;
    [SerializeField] private List<Renderer> gateMeshRenderers;
    [SerializeField] private List<Collider> gateColliders;
    [SerializeField] private float disableCollidersDelay = 1.5f;
    [SerializeField] private DissolveSettingsSo unlockDissolveSettingsSo;
    
    [SerializeField] private bool isOverridenByPurchasableHouseItem;
    [SerializeField] [ShowIf("isOverridenByPurchasableHouseItem")] private PurchasableHouseItemBehaviour linkedPurchasableHouseItemBehaviour;
    
    public bool Unlocked { get; private set; }
    
    private static readonly int CutoffHeight = Shader.PropertyToID("_CutoffHeight");
    private static readonly int CutoffHeight2 = Shader.PropertyToID("_Cutoff_Height");
    private static readonly int CatNo = Animator.StringToHash("DoNo");
    
    
    private void Start()
    {
        InitChargedGate();

        if (chargedBiomeAreaSos.Count == 0)
        {
            Unlocked = true;
        
            gameObject.SetActive(false);
        }
        else
        {
            pricePopUpBehaviour.SetPrice(chargedBiomeAreaSos[0].PurchaseCost);
            pricePopUpBehaviour.HidePrice();
        
            GateManager.Instance.OnAreaPurchased.AddListener(Start);
            InitChargedGate();
        }
    }
    
    
    public void InitChargedGate()
    {
        foreach (ChargedBiomeAreaSo chargedBiomeAreaSo in chargedBiomeAreaSos.ToList())
        {
            if (GameDontDestroyOnLoadManager.Instance.UnlockedChargedBiomeAreas.Contains(chargedBiomeAreaSo))
            {
                chargedBiomeAreaSos.Remove(chargedBiomeAreaSo);
            }
        }
    }
    

    public bool Purchase(bool isCalledByOther = false)
    {
        if (MoneyManager.Instance.MoneyAmount < chargedBiomeAreaSos[0].PurchaseCost)
        {
            CharacterAnimManager.instance.animator.SetTrigger(CatNo);
            return false;
        }

        MoneyManager.Instance.SubtractMoney(chargedBiomeAreaSos[0].PurchaseCost);
        Unlocked = true;
        GameDontDestroyOnLoadManager.Instance.UnlockedChargedBiomeAreas.Add(chargedBiomeAreaSos[0]);
        GateManager.Instance.OnAreaPurchased.RemoveListener(Start);
        GateManager.Instance.OnAreaPurchased.Invoke();

        if (isOverridenByPurchasableHouseItem && !isCalledByOther)
        {
            linkedPurchasableHouseItemBehaviour.PurchaseItem(true);
        }
        
        pricePopUpBehaviour.HidePrice();
        
        foreach (Renderer gateMeshRenderer in gateMeshRenderers)
        {
            gateMeshRenderer.material.DOFloat(gateMeshRenderer is MeshRenderer ? 0f : 1f,
                    gateMeshRenderer is MeshRenderer ? CutoffHeight : CutoffHeight2, 
                    unlockDissolveSettingsSo.AnimationDuration).SetEase(unlockDissolveSettingsSo.AnimationCurve)
                .OnComplete(() => gameObject.SetActive(false));
        }
        
        StartCoroutine(DisableColliders());
        
        return true;
    }
    
    private IEnumerator DisableColliders()
    {
        yield return new WaitForSeconds(disableCollidersDelay);
        
        foreach (Collider gateCollider in gateColliders)
        {
            gateCollider.enabled = false;
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!Unlocked)
        {
            if (isOverridenByPurchasableHouseItem)
            {
                if (linkedPurchasableHouseItemBehaviour.selfIndex > GameDontDestroyOnLoadManager.Instance.WorkshopProgressionIndex) return;
                
                linkedPurchasableHouseItemBehaviour.LastTriggeredCollider = other;
                linkedPurchasableHouseItemBehaviour.Triggerers.Add(gameObject);
            }
            
            if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
                characterInteractController.collectedStack.Count == 0)
            {
                pricePopUpBehaviour.ShowPrice(chargedBiomeAreaSos[0].PurchaseCost);
                
                characterInteractController.CurrentNearChargedGate = this;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.CurrentNearChargedGate == this)
        {
            characterInteractController.CurrentNearChargedGate = null;
        }

        if (isOverridenByPurchasableHouseItem)
        {
            if (linkedPurchasableHouseItemBehaviour.Triggerers.Contains(gameObject))
            {
                linkedPurchasableHouseItemBehaviour.Triggerers.Remove(gameObject);

                if (linkedPurchasableHouseItemBehaviour.Triggerers.Count == 0)
                {
                    if (linkedPurchasableHouseItemBehaviour.LastTriggeredCollider == other)
                    {
                        linkedPurchasableHouseItemBehaviour.LastTriggeredCollider = null;
                    }
        
                    pricePopUpBehaviour.HidePrice();
                }
            }
        }
        else
        {
            pricePopUpBehaviour.HidePrice();
        }
    }
}
