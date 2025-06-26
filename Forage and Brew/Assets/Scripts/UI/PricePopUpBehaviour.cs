using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PricePopUpBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject pricePopUpCanvas;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite enoughMoneyBackgroundSprite;
    [SerializeField] private Sprite notEnoughMoneyBackgroundSprite;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Color enoughMoneyColor = Color.green;
    [SerializeField] private Color notEnoughMoneyColor = Color.red;
    [SerializeField] private GameObject purchaseButtonGameObject;


    public void SetPrice(int price)
    {
        priceText.text = price.ToString();
    }


    public void ShowPrice(int price)
    {
        pricePopUpCanvas.SetActive(true);
        backgroundImage.sprite = MoneyManager.Instance.MoneyAmount < price ? notEnoughMoneyBackgroundSprite : enoughMoneyBackgroundSprite;
        priceText.color = MoneyManager.Instance.MoneyAmount < price ? notEnoughMoneyColor : enoughMoneyColor;
        purchaseButtonGameObject.SetActive(MoneyManager.Instance.MoneyAmount >= price);
    }
    
    public void HidePrice()
    {
        pricePopUpCanvas.SetActive(false);
    }
}
