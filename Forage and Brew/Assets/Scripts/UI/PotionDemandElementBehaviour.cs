using UnityEngine;
using UnityEngine.UI;

public class PotionDemandElementBehaviour : MonoBehaviour
{
    [SerializeField] private Image mainImage;
    [SerializeField] private Image liquidImage;
    [SerializeField] private GameObject checkMarkGameObject;
    
    public void SetImage(PotionDemand potionDemand)
    {
        if (potionDemand.IsSpecific)
        {
            SetPotionImage(potionDemand.Potion.PotionDifficulty.PotionSprite,
                potionDemand.Potion.PotionDifficulty.LiquidSprite, 
                potionDemand.Potion.SpriteLiquidColor);
        }
        else
        {
            SetNonPotionImage(potionDemand.ValidTag.Sprite);
        }
    }
    
    public void SetImage(PotionValuesSo potionValuesSo)
    {
        SetPotionImage(potionValuesSo.PotionDifficulty.PotionSprite,
            potionValuesSo.PotionDifficulty.LiquidSprite, 
            potionValuesSo.SpriteLiquidColor);
    }
    
    public void SetPotionImage(Sprite potionSprite, Sprite liquidSprite, Color liquidColor)
    {
        mainImage.sprite = potionSprite;
        liquidImage.sprite = liquidSprite;
        liquidImage.color = liquidColor;
    }
    
    public void SetNonPotionImage(Sprite sprite)
    {
        mainImage.sprite = sprite;
    }
    
    public void EnableCheckMark()
    {
        checkMarkGameObject.SetActive(true);
    }
    
    public void DisableCheckMark()
    {
        checkMarkGameObject.SetActive(false);
    }
}
