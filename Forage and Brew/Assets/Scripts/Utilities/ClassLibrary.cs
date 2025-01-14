using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[Serializable]

public class Letter
{
    [field: SerializeField] public LetterContentSo LetterContent { get; set; }
    [field: SerializeField] public NarrativeBlockOfLetters RelatedNarrativeBlock { get; set; }
    [field: SerializeField] public bool DeliveredOnTime { get; set; }

    public Letter(LetterContentSo content, NarrativeBlockOfLetters nBlock, bool deliveredOnTime)
    {
        LetterContent = content;
        RelatedNarrativeBlock = nBlock;
        DeliveredOnTime = deliveredOnTime;
    }
    public Letter(LetterContentSo content, NarrativeBlockOfLetters nBlock)
    {
        LetterContent = content;
        RelatedNarrativeBlock = nBlock;
    }
}

public class Order
{
    public OrderContentSo OrderContent { get; set; }
    public OrderCodexDisplayBehaviour OrderDisplay { get; set; }
    public int Days { get; set; }
    public NarrativeBlockOfLetters RelatedNarrativeBlock { get; private set; }
    public LetterContentSo RelatedLetter { get; private set; }
    public LetterContentSo RelatedSuccessLetter { get; private set; }
    public LetterContentSo RelatedFailureLetter { get; private set; }

    public Order(Letter LetterToOrder, OrderCodexDisplayBehaviour orderDisplay)
    {
        OrderDisplay = orderDisplay;
        OrderContent = LetterToOrder.LetterContent.OrderContent;
        Days = OrderContent.TimeToFulfill;
        RelatedLetter = LetterToOrder.LetterContent;
        RelatedSuccessLetter = LetterToOrder.LetterContent.RelatedSuccessLetter;
        RelatedFailureLetter = LetterToOrder.LetterContent.RelatedFailureLetter;
        RelatedNarrativeBlock = LetterToOrder.RelatedNarrativeBlock;
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

[Serializable]
public class NarrativeBlockOfLetters
{
    [field: SerializeField] public NarrativeBlockOfLettersContentSo ContentSo { get; set; }
    
    [field: AllowNesting] [field: SerializeField] [field: ReadOnly] public int SelfProgressionIndex { get; set; }
    [field: SerializeField] [field: HideInInspector] public bool[] CompletedLetters { get; set; }
    [field: SerializeField] [field: HideInInspector] public bool[] InactiveLetters { get; set; }

    public NarrativeBlockOfLetters(NarrativeBlockOfLettersContentSo content)
    {
        ContentSo = content;
        
        CompletedLetters = new bool [ContentSo.Content.Length];
        InactiveLetters = new bool [ContentSo.Content.Length];
    }
}

[Serializable]
public class ClientOrderPotions
{
    [field: SerializeField] public ClientSo ClientSo { get; set; }
    [field: SerializeField] public List<PotionValuesSo> Potions { get; set; } = new();
}

[Serializable]
public class WeatherSuccessiveDays
{
    [field: SerializeField] public WeatherStateSo WeatherStateSo { get; set; }
    [field: SerializeField] public int SuccessiveDays { get; set; }
    
    public WeatherSuccessiveDays(WeatherStateSo weatherStateSo, int successiveDays)
    {
        WeatherStateSo = weatherStateSo;
        SuccessiveDays = successiveDays;
    }
}
