using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class PurchasableHouseItemBehaviour : MonoBehaviour
{
    [Header("Purchasable House Item Values")]
    [SerializeField] private int selfIndex;
    [SerializeField] protected int purchaseCost;
    
    public bool CanPurchase { get; private set; }
    public bool Unlocked { get; private set; }
    
    [Header("Purchasable House Item Material")]
    [SerializeField] private List<Renderer> purchasableItemMeshRenderers;
    [SerializeField] private DissolveSettingsSo unlockDissolveSettingsSo;
    
    [Header("Purchasable House Item UI")]
    [SerializeField] protected PricePopUpBehaviour pricePopUpBehaviour;

    protected Collider LastTriggeredCollider;
    
    private static readonly int CutoffHeight = Shader.PropertyToID("_CutoffHeight");
    
    
    protected virtual void Start()
    {
        pricePopUpBehaviour.SetPrice(purchaseCost);
        pricePopUpBehaviour.HidePrice();

        foreach (Renderer purchasableItemMeshRenderer in purchasableItemMeshRenderers)
        {
            purchasableItemMeshRenderer.material.SetFloat(CutoffHeight,
                GameDontDestroyOnLoadManager.Instance.WorkshopProgressionIndex > selfIndex ? 1f : 0f);
        }

        PurchasableHouseItemManager.Instance.OnItemPurchased.AddListener(InitPurchasableHouseItem);
        InitPurchasableHouseItem();
    }
    

    public void InitPurchasableHouseItem()
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
        if (MoneyManager.Instance.MoneyAmount < purchaseCost) return;

        MoneyManager.Instance.SubtractMoney(purchaseCost);
        CanPurchase = false;
        Unlocked = true;
        GameDontDestroyOnLoadManager.Instance.WorkshopProgressionIndex++;
        pricePopUpBehaviour.HidePrice();
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
