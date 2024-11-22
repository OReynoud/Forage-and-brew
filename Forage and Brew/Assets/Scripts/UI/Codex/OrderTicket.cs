using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderTicket : MonoBehaviour
{

    public TextMeshProUGUI clientNameText;

    public Image clientPortraitImage;
    
    public TextMeshProUGUI descriptionText;

    public List<CodexContentManager.PotionDemand> potionsDemanded = new List<CodexContentManager.PotionDemand>();

    public Image[] potionImages;
    public TextMeshProUGUI[] potionKeywords;
    

    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void InitializeOrder(string client,string description, Sprite clientPortrait, List<CodexContentManager.PotionDemand> Potions)
    {
        clientNameText.text = client;
        descriptionText.text = description;
        clientPortraitImage.sprite = clientPortrait;
        potionsDemanded.Clear();
        potionsDemanded.AddRange(Potions);

        foreach (var potionImage in potionImages)
        {
            potionImage.enabled = false;
        }
        for (int i = 0; i < potionsDemanded.Count; i++)
        {
            potionImages[i].enabled = true;
            potionImages[i].sprite = potionsDemanded[i].relatedIcon;
        }
    }
    
    public void InitializeOrder(string client,string description, List<CodexContentManager.PotionDemand> Potions)
    {
        clientNameText.text = client;
        descriptionText.text = description;
        potionsDemanded.Clear();
        potionsDemanded.AddRange(Potions);
        
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
        //Debug.Log("Oui");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
