using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BundleContainerCodexDisplay : MonoBehaviour
{

    public BundleElementContainerDisplay[] potionRequirementElements;

    public TextMeshProUGUI rewardMoneyText;

    public PotionEnsembleSo displayedBundle;

    public bool isInitialized = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void InitBundle(PotionEnsembleSo bundleToDisplay)
    {
        displayedBundle = bundleToDisplay;

        for (int i = 0; i < potionRequirementElements.Length; i++)
        {
            potionRequirementElements[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < displayedBundle.Potions.Count; i++)
        {
            potionRequirementElements[i].gameObject.SetActive(true);
            
            
            potionRequirementElements[i].InitSelf(bundleToDisplay.Potions[i].SpriteLiquidColor,bundleToDisplay.Potions[i].PotionDifficulty.PotionSprite);
        }

        rewardMoneyText.text = displayedBundle.MoneyReward.ToString();
        isInitialized = true;
    }

    public void UpdateCheckmarks(int index)
    {
        potionRequirementElements[index].checkmarkImage.enabled = true;
        
    }
}
