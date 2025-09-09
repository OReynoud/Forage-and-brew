using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class PurchasableHouseItemBehaviour : WeedContainerBehavior
{
    [Header("Purchasable House Item Values")]
    [SerializeField]
    protected int selfIndex;
    [SerializeField] protected int purchaseCost;
    public bool CanPurchase { get; set; }
    public bool Unlocked { get; set; }
    
    [Header("Purchasable House Item Material")]
    [SerializeField] private List<Renderer> purchasableItemMeshRenderers;
    [SerializeField] private DissolveSettingsSo unlockDissolveSettingsSo;
    
    [Header("Purchasable House Item UI")]
    [SerializeField] protected PricePopUpBehaviour pricePopUpBehaviour;
    [SerializeField] protected LockBehaviour lockBehaviour;

    protected Collider LastTriggeredCollider;
    
    private static readonly int CutoffHeight = Shader.PropertyToID("_CutoffHeight");
    private static readonly int CatNo = Animator.StringToHash("DoNo");
    
    
    protected virtual void Start()
    {
        pricePopUpBehaviour.SetPrice(purchaseCost);
        pricePopUpBehaviour.HidePrice();

        PurchasableHouseItemManager.Instance.OnItemPurchased.AddListener(InitPurchasableHouseItem);
        InitPurchasableHouseItem();

        if (Unlocked)
        {
            lockBehaviour.Disable();
        }

        foreach (Renderer purchasableItemMeshRenderer in purchasableItemMeshRenderers)
        {
            purchasableItemMeshRenderer.material.SetFloat(CutoffHeight, Unlocked ? 1f : 0f);
        }
    }
    

    public virtual void InitPurchasableHouseItem()
    {
        if (GameDontDestroyOnLoadManager.Instance.WorkshopProgressionIndex == selfIndex)
        {
            CanPurchase = true;
        }
        else if (GameDontDestroyOnLoadManager.Instance.WorkshopProgressionIndex > selfIndex)
        {
            Unlocked = true;
        }
    }
    

    public virtual void PurchaseItem()
    {
        if (MoneyManager.Instance.MoneyAmount < purchaseCost)
        {
            CharacterAnimManager.instance.animator.SetTrigger(CatNo);
            return;
        }

        MoneyManager.Instance.SubtractMoney(purchaseCost);
        CanPurchase = false;
        Unlocked = true;
        GameDontDestroyOnLoadManager.Instance.WorkshopProgressionIndex++;
        pricePopUpBehaviour.HidePrice();
        lockBehaviour.Unlock();
        ManageCharacterNear(LastTriggeredCollider);
        foreach (Renderer purchasableItemMeshRenderer in purchasableItemMeshRenderers)
        {
            purchasableItemMeshRenderer.material.DOFloat(1f, CutoffHeight, unlockDissolveSettingsSo.AnimationDuration)
                .SetEase(unlockDissolveSettingsSo.AnimationCurve);
        }
        PurchasableHouseItemManager.Instance.OnItemPurchased.Invoke();
    }


    protected abstract void ManageCharacterNear(Collider other);

    protected abstract void ManageCharacterFar(Collider other);
    
    
    private void OnTriggerEnter(Collider other)
    {
        ManageCharacterNear(other);
    }

    private void OnTriggerExit(Collider other)
    {
        ManageCharacterFar(other);
    }
}
