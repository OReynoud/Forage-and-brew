using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderCodexDisplayBehaviour : PageBehavior
{
    public TextMeshProUGUI clientNameText;
    public Image orderBackground;
    
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI delayTimeText;
    public Image outdatedStamp;

    public VerticalLayoutGroup mainContentLayoutGroup;
    public List<PotionDemand> potionsDemanded = new();
    public int daysLeftToComplete;

    public OrderSpecificPotionDemand specificPotionPrefab;
    public GameObject keywordPotionPrefab;
    public List<Transform> potionLists;

    private void Start()
    {
        SceneTransitionManager.instance.OnSleep.AddListener(UpdateDaysLeftToComplete);
    }

    private void UpdateDaysLeftToComplete()
    {
        if (daysLeftToComplete <= 0)
        {
            outdatedStamp.enabled = true;
            delayTimeText.text = "Overdue";
        }
        else
        {
            daysLeftToComplete--;
            delayTimeText.text = daysLeftToComplete + daysLeftToComplete == 1 ? "Day remaining" : " Days remaining";
        }
    }

    public override void InitOrder(ClientSo client,string description, PotionDemand[] Potions, int Reward, int TTC, int index)
    {
        clientNameText.text = client.Name;
        descriptionText.text = description;
        potionsDemanded.Clear();
        potionsDemanded.AddRange(Potions);
        daysLeftToComplete = TTC;
        orderBackground.color = client.AssociatedColor;

        outdatedStamp.enabled = false;
        delayTimeText.text = TTC + (TTC == 1 ? " Day remaining" : " Days remaining");
        
        for (int i = 0; i < potionsDemanded.Count; i++)
        {
            if (potionsDemanded[i].IsSpecific)
            {
                var specificPotion = Instantiate(specificPotionPrefab, potionLists[Mathf.FloorToInt(i / 2f)]);
                specificPotion.potionIcon.sprite = potionsDemanded[i].Potion.PotionDifficulty.PotionSprite;
                specificPotion.liquidIcon.sprite = potionsDemanded[i].Potion.PotionDifficulty.LiquidSprite;
                specificPotion.liquidIcon.color = potionsDemanded[i].Potion.SpriteLiquidColor;
                specificPotion.potionName.text = potionsDemanded[i].Potion.Name;
            }
            else
            {
                var keyword = Instantiate(keywordPotionPrefab, potionLists[Mathf.CeilToInt(i / 2f)]).GetComponentInChildren<TextMeshProUGUI>();
                
                keyword.text = potionsDemanded[i].Keywords;
            }
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainContentLayoutGroup.transform as RectTransform);
        
    }
}
