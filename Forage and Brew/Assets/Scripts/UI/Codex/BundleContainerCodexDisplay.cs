using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BundleContainerCodexDisplay : MonoBehaviour
{

    public Image[] potionRequirementImages;
    public Image[] potionLiquidImages;

    public Image[] checkmarkImages;

    public TextMeshProUGUI rewardMoneyText;

    public PotionEnsembleSo displayedBundle;

    public bool isInitialized = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void InitBundle(PotionEnsembleSo bundleToDisplay)
    {
        displayedBundle = bundleToDisplay;

        for (int i = 0; i < potionRequirementImages.Length; i++)
        {
            potionRequirementImages[i].enabled = false;
            checkmarkImages[i].enabled = false;
            potionLiquidImages[i].enabled = false;
        }
        for (int i = 0; i < displayedBundle.Potions.Count; i++)
        {
            potionRequirementImages[i].enabled = true;
            potionLiquidImages[i].enabled = true;

            potionRequirementImages[i].sprite = displayedBundle.Potions[i].PotionDifficulty.PotionSprite;
            potionLiquidImages[i].color = displayedBundle.Potions[i].SpriteLiquidColor;
        }

        rewardMoneyText.text = displayedBundle.MoneyReward.ToString();
        isInitialized = true;
    }

    public void UpdateCheckmarks(int index)
    {
        checkmarkImages[index].enabled = true;
        
    }
}
