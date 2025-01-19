using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderCodexDisplayBehaviour : MonoBehaviour
{
    public TextMeshProUGUI clientNameText;
    public Image orderBackground;
    
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI delayTimeText;
    public Image outdatedStamp;

    public List<PotionDemand> potionsDemanded = new();

    public int pageNumber;
    public int moneyReward;
    public int daysLeftToComplete;

    public OrderSpecificPotionDemand specificPotionPrefab;
    public GameObject keywordPotionPrefab;
    public Transform potionList;

    private void Start()
    {
        SceneTransitionManager.instance.OnSleep.AddListener(UpdateDaysLeftToComplete);
    }

    private void UpdateDaysLeftToComplete()
    {
        daysLeftToComplete--;
        delayTimeText.text = daysLeftToComplete + " Days";
        
        if (daysLeftToComplete <= 0)
        {
            outdatedStamp.enabled = true;
        }
        
    }

    public void InitializeOrder(ClientSo client,string description, PotionDemand[] Potions, int Reward, int TTC, int index)
    {
        clientNameText.text = client.Name;
        descriptionText.text = description;
        moneyReward = Reward;
        potionsDemanded.Clear();
        potionsDemanded.AddRange(Potions);
        daysLeftToComplete = TTC;
        pageNumber = index;
        orderBackground.color = client.AssociatedColor;

        outdatedStamp.enabled = false;
        delayTimeText.text = TTC + " Days";
        
        for (int i = 0; i < potionsDemanded.Count; i++)
        {
            if (potionsDemanded[i].IsSpecific)
            {
                var specificPotion = Instantiate(specificPotionPrefab,potionList);
                specificPotion.potionIcon.sprite = potionsDemanded[i].Potion.PotionDifficulty.PotionSprite;
                specificPotion.liquidIcon.sprite = potionsDemanded[i].Potion.PotionDifficulty.LiquidSprite;
                specificPotion.liquidIcon.color = potionsDemanded[i].Potion.SpriteLiquidColor;
                specificPotion.potionName.text = potionsDemanded[i].Potion.Name;
            }
            else
            {
                var keyword = Instantiate(keywordPotionPrefab,potionList).GetComponentInChildren<TextMeshProUGUI>();
                
                keyword.text = potionsDemanded[i].Keywords;
            }
        }
    }
}
