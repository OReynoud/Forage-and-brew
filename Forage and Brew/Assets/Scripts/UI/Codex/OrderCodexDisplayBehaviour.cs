using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderCodexDisplayBehaviour : PageBehavior
{
    public TextMeshProUGUI clientNameText;
    public Image orderBackground;
    public TextMeshProUGUI moneyAmount;
    
    public TextMeshProUGUI descriptionText;

    public VerticalLayoutGroup mainContentLayoutGroup;
    public List<PotionDemand> potionsDemanded = new();

    public OrderSpecificPotionDemand specificPotionPrefab;
    public OrderSpecificPotionDemand keywordPotionPrefab;
    public List<Transform> potionLists = new();
    public List<OrderSpecificPotionDemand> demandedPotionsList = new();


    private void OnDisable()
    {
        // Debug.Log("gne");
    }

    public override void InitOrder(ClientSo client,string description, PotionDemand[] Potions, int Reward, int index)
    {
        clientNameText.text = client.Name;
        descriptionText.text = description;
        potionsDemanded.Clear();
        potionsDemanded.AddRange(Potions);
        orderBackground.color = client.AssociatedColor;
        moneyAmount.text = Reward.ToString();
        
        for (int i = 0; i < potionsDemanded.Count; i++)
        {
            if (potionsDemanded[i].IsSpecific)
            {
                var specificPotion = Instantiate(specificPotionPrefab, potionLists[Mathf.FloorToInt(i / 2f)]);
                specificPotion.potionIcon.sprite = potionsDemanded[i].Potion.PotionDifficulty.PotionSprite;
                specificPotion.liquidIcon.sprite = potionsDemanded[i].Potion.PotionDifficulty.LiquidSprite;
                specificPotion.liquidIcon.color = potionsDemanded[i].Potion.SpriteLiquidColor;
                specificPotion.potionName.text = potionsDemanded[i].Potion.Name;
                specificPotion.checkMark.enabled = false;
                demandedPotionsList.Add(specificPotion);
            }
            else
            {
                var keyword = Instantiate(keywordPotionPrefab, potionLists[Mathf.CeilToInt(i / 2f)]);
                
                keyword.potionName.text = potionsDemanded[i].Keywords;
                keyword.checkMark.enabled = false;
                demandedPotionsList.Add(keyword);
            }
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainContentLayoutGroup.transform as RectTransform);
    }
}
