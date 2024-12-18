using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderCodexDisplayBehaviour : MonoBehaviour
{
    public TextMeshProUGUI clientNameText;
    
    public TextMeshProUGUI descriptionText;

    public List<PotionDemand> potionsDemanded = new();

    public Image[] potionImages;
    public TextMeshProUGUI[] potionKeywords;
    public int pageNumber;
    public int moneyReward;
    public int daysLeftToComplete;

    
    public void InitializeOrder(string client,string description, PotionDemand[] Potions, int Reward, int TTC, int index)
    {
        clientNameText.text = client;
        descriptionText.text = description;
        moneyReward = Reward;
        potionsDemanded.Clear();
        potionsDemanded.AddRange(Potions);
        daysLeftToComplete = TTC;
        pageNumber = index;
        
        foreach (var potionImage in potionImages)
        {
            potionImage.gameObject.SetActive(false);
        }

        foreach (var keyword in potionKeywords)
        {
            keyword.transform.parent.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < potionsDemanded.Count; i++)
        {
            if (potionsDemanded[i].IsSpecific)
            {
                potionImages[i].gameObject.SetActive(true);
                potionImages[i].sprite = potionsDemanded[i].Potion.icon;
            }
            else
            {
                potionKeywords[i].transform.parent.gameObject.SetActive(true);
                potionKeywords[i].text = potionsDemanded[i].Keywords;
            }
        }
    }
}
