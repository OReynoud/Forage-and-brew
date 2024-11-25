using System.Collections.Generic;
using UnityEngine;

public class CodexContentManager : MonoBehaviour
{
    public class PotionDemand
    {
        public bool isSpecific;
        public PotionValuesSo potion;
        public string keywords;
        public PotionTag validTag;
        public Sprite relatedIcon;
        public bool isSubmitted = false;

        public PotionDemand(bool Specific, PotionValuesSo Potion, Sprite Icon)
        {
            potion = Potion;
            isSpecific = Specific;
            relatedIcon = Icon;
        }
        public PotionDemand(bool Specific, PotionTag Tag, Sprite Icon, string Keywords)
        {
            validTag = Tag;
            isSpecific = Specific;
            keywords = Keywords;
            relatedIcon = Icon;
        }
    }
    public List<OrderTicket> tickets = new List<OrderTicket>();

    public Sprite potionIcon;
    public PotionTag testTag;

    public PotionValuesSo testPotion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var ticket in tickets)
        {
            ticket.gameObject.SetActive(false);
        }
        var temp = new List<PotionDemand>();
        temp.Add(new PotionDemand(true,testPotion,potionIcon));
        tickets[0].gameObject.SetActive(true);
        tickets[0].InitializeOrder("Jean-Eude","Je me suis coupé le doigt, tu peux me passer de la pommade s'il te plait?",temp);
        temp.Clear();
        temp.Add(new PotionDemand(false,testTag,potionIcon,"Something against a fever"));
        tickets[1].gameObject.SetActive(true);
        tickets[1].InitializeOrder("Paul","J'ai de la fièvre, t'as quelque chose pour m'aider?",temp);
        temp.Clear();
        temp.Add(new PotionDemand(true,testPotion,potionIcon));
        temp.Add(new PotionDemand(true,testPotion,potionIcon));
        tickets[2].gameObject.SetActive(true);
        tickets[2].InitializeOrder("Marie","J'ai besoin de comparer la saveur de ces deux jus, peux-tu me les préparer?",temp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
