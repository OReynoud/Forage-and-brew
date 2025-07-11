using UnityEngine;
using UnityEngine.UI;

public class BundleElementContainerDisplay : MonoBehaviour
{
    
    public Image potionRequirementImage;

    public Image potionLiquidImage;
    public Image checkmarkImage;

    public void InitSelf(Color liquidColor, Sprite potionSprite)
    {
        checkmarkImage.enabled = false;
        potionRequirementImage.sprite = potionSprite;
        potionLiquidImage.color = liquidColor;
    }
}
