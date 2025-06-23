using TMPro;
using UnityEngine;

public class PurchasableHouseItemBehaviour : MonoBehaviour
{
    [field: SerializeField] public int SelfIndex { get; private set; }
    [field: SerializeField] public int PurchaseCost { get; private set; }
    [field: SerializeField] public bool CanPurchase{ get; set; }
    [field: SerializeField] public bool Unlocked{ get; set; }
    [SerializeField] private GameObject PriceUI;
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private TextMeshProUGUI priceText;
    
    
    private void Start()
    {
        DisableInteract();
        HidePrice();
        if (GameDontDestroyOnLoadManager.Instance.PotionsUpgradeIndex == SelfIndex)
        {
            priceText.text = PurchaseCost.ToString();
            CanPurchase = true;
        }
        else if (GameDontDestroyOnLoadManager.Instance.PotionsUpgradeIndex > SelfIndex)
        {
            Unlocked = true;
        }
    }


    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    public void ShowPrice()
    {
        PriceUI.SetActive(true);
    }
    
    public void HidePrice()
    {
        PriceUI.SetActive(false);
    }

    public void PurchaseItem()
    {
        if (MoneyManager.Instance.MoneyAmount < PurchaseCost)
            return;

        Debug.Log("Bought an upgrade");
        MoneyManager.Instance.SubtractMoney(PurchaseCost);
        CanPurchase = false;
        Unlocked = true;
        GameDontDestroyOnLoadManager.Instance.PotionsUpgradeIndex++;
        HidePrice();

    }
    
}
