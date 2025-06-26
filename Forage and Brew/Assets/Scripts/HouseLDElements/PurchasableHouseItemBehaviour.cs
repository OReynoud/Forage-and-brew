using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PurchasableHouseItemBehaviour : MonoBehaviour
{
    [Header("Purchasable House Item Values")]
    [SerializeField] private int selfIndex;
    [SerializeField] private int purchaseCost;
    
    public bool CanPurchase { get; private set; }
    public bool Unlocked { get; private set; }
    
    [Header("Purchasable House Item Material")]
    [SerializeField] private List<Renderer> purchasableItemMeshRenderers;
    [SerializeField] private float unlockAnimationDuration = 0.5f;
    [SerializeField] private AnimationCurve unlockAnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Purchasable House Item UI")]
    [SerializeField] private GameObject priceUI;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite enoughMoneyBackgroundSprite;
    [SerializeField] private Sprite notEnoughMoneyBackgroundSprite;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Color enoughMoneyColor = Color.green;
    [SerializeField] private Color notEnoughMoneyColor = Color.red;
    [SerializeField] private GameObject purchaseButtonGameObject;
    
    protected Collider LastTriggeredCollider { get; set; }
    
    private static readonly int CutoffHeight = Shader.PropertyToID("_CutoffHeight");
    
    
    protected virtual void Start()
    {
        priceText.text = purchaseCost.ToString();
        HidePrice();

        foreach (Renderer purchasableItemMeshRenderer in purchasableItemMeshRenderers)
        {
            purchasableItemMeshRenderer.material.SetFloat(CutoffHeight,
                GameDontDestroyOnLoadManager.Instance.PotionsUpgradeIndex > selfIndex ? 1f : 0f);
        }

        PurchasableHouseItemManager.Instance.OnItemPurchased.AddListener(InitPurchasableHouseItem);
        InitPurchasableHouseItem();
    }
    

    public void InitPurchasableHouseItem()
    {
        if (GameDontDestroyOnLoadManager.Instance.PotionsUpgradeIndex == selfIndex)
        {
            CanPurchase = true;
        }
        else if (GameDontDestroyOnLoadManager.Instance.PotionsUpgradeIndex > selfIndex)
        {
            Unlocked = true;
        }
    }


    public void ShowPrice()
    {
        priceUI.SetActive(true);
        backgroundImage.sprite = MoneyManager.Instance.MoneyAmount < purchaseCost ? notEnoughMoneyBackgroundSprite : enoughMoneyBackgroundSprite;
        priceText.color = MoneyManager.Instance.MoneyAmount < purchaseCost ? notEnoughMoneyColor : enoughMoneyColor;
        purchaseButtonGameObject.SetActive(MoneyManager.Instance.MoneyAmount >= purchaseCost);
    }
    
    public void HidePrice()
    {
        priceUI.SetActive(false);
    }
    

    public virtual void PurchaseItem()
    {
        if (MoneyManager.Instance.MoneyAmount < purchaseCost) return;

        Debug.Log("Bought an upgrade");
        MoneyManager.Instance.SubtractMoney(purchaseCost);
        CanPurchase = false;
        Unlocked = true;
        GameDontDestroyOnLoadManager.Instance.PotionsUpgradeIndex++;
        HidePrice();
        ManageCharacterNear(LastTriggeredCollider);
        foreach (Renderer purchasableItemMeshRenderer in purchasableItemMeshRenderers)
        {
            purchasableItemMeshRenderer.material.DOFloat(1f, CutoffHeight, unlockAnimationDuration)
                .SetEase(unlockAnimationCurve);
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
