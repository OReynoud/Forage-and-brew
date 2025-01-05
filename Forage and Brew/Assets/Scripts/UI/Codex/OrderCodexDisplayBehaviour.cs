using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderCodexDisplayBehaviour : MonoBehaviour
{
    public TextMeshProUGUI clientNameText;
    
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI delayTimeText;

    public List<PotionDemand> potionsDemanded = new();

    public int pageNumber;
    public int moneyReward;
    public int daysLeftToComplete;

    public GameObject specificPotionPrefab;
    public GameObject keywordPotionPrefab;
    public Transform potionList;

    
    public void InitializeOrder(string client,string description, PotionDemand[] Potions, int Reward, int TTC, int index)
    {
        clientNameText.text = client;
        descriptionText.text = description;
        moneyReward = Reward;
        potionsDemanded.Clear();
        potionsDemanded.AddRange(Potions);
        daysLeftToComplete = TTC;
        pageNumber = index;

        delayTimeText.text = TTC + " Days";
        
        for (int i = 0; i < potionsDemanded.Count; i++)
        {
            if (potionsDemanded[i].IsSpecific)
            {
                var specificImage = Instantiate(specificPotionPrefab,potionList).GetComponent<Image>();
                var specificName = specificImage.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                specificImage.sprite = potionsDemanded[i].Potion.icon;
                specificName.text = potionsDemanded[i].Potion.Name;
            }
            else
            {
                var keyword = Instantiate(keywordPotionPrefab,potionList).GetComponentInChildren<TextMeshProUGUI>();
                
                keyword.text = potionsDemanded[i].Keywords;
            }
        }
    }
}
