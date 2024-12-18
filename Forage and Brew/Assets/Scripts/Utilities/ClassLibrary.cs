using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class Order
{
    [field: SerializeField] public OrderContentSo OrderContent { get; set; }
    [field: SerializeField] public int Days { get; set; }

    public Order(OrderContentSo newOrder)
    {
        OrderContent = newOrder;
        Days = OrderContent.TimeToFulfill;
    }
}

[Serializable]
public class PotionDemand
{
    [field: SerializeField] public bool IsSpecific { get; private set; }
    [field: AllowNesting] [field: ShowIf("IsSpecific")] [field: SerializeField] public PotionValuesSo Potion { get; private set; }
    [field: AllowNesting] [field: HideIf("IsSpecific")] [field: SerializeField] public string Keywords { get; private set; }
    [field: AllowNesting] [field: HideIf("IsSpecific")] [field: SerializeField] public PotionTag ValidTag { get; private set; }

    public PotionDemand(bool newIsSpecific, PotionValuesSo newPotion)
    {
        Potion = newPotion;
        IsSpecific = newIsSpecific;
    }
    
    public PotionDemand(bool newIsSpecific, PotionTag newTag, string newKeywords)
    {
        ValidTag = newTag;
        IsSpecific = newIsSpecific;
        Keywords = newKeywords;
    }
}
