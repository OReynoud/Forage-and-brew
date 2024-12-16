using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderTicket : MonoBehaviour
{

    public TextMeshProUGUI clientNameText;
    
    public TextMeshProUGUI descriptionText;

    public List<CodexContentManager.PotionDemand> potionsDemanded = new List<CodexContentManager.PotionDemand>();

    public Image[] potionImages;
    public TextMeshProUGUI[] potionKeywords;
    public int pageNumber;
    public float moneyReward;
    public int daysLeftToComplete;
    

    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
    public void InitializeOrder(string client,string description, CodexContentManager.PotionDemand[] Potions, float Reward, int TTC, int index)
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
            if (potionsDemanded[i].isSpecific)
            {
                potionImages[i].gameObject.SetActive(true);
                potionImages[i].sprite = potionsDemanded[i].relatedIcon;
            }
            else
            {
                potionKeywords[i].transform.parent.gameObject.SetActive(true);
                potionKeywords[i].text = potionsDemanded[i].keywords;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
