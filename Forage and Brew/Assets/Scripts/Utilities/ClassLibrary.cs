using System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]

public class Letter
{
    [field: SerializeField] public LetterContentSo LetterContent { get; set; }
    [field: SerializeField] public NarrativeBlockOfLetters RelatedNarrativeBlock { get; private set; }

    public Letter(LetterContentSo Content, NarrativeBlockOfLetters nBlock)
    {
        LetterContent = Content;
        RelatedNarrativeBlock = nBlock;
    }
    
}
public class Order
{
    [field: SerializeField] public OrderContentSo OrderContent { get; set; }
    [field: SerializeField] public int Days { get; set; }
    [field: SerializeField] public NarrativeBlockOfLetters RelatedNarrativeBlock { get; private set; }
    [field: SerializeField] public LetterContentSo RelatedLetter { get; private set; }
    [field: SerializeField] public LetterContentSo RelatedSuccessLetter { get; private set; }
    [field: SerializeField] public LetterContentSo RelatedFailureLetter { get; private set; }

    public Order(Letter LetterToOrder)
    {
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
    public bool[] CompletedLetters { get; set; }
    public bool[] InactiveLetters { get; set; }

    public NarrativeBlockOfLetters(NarrativeBlockOfLettersContentSo Content)
    {
        ContentSo = Content;
        
        CompletedLetters = new bool [ContentSo.Content.Length];
        InactiveLetters = new bool [ContentSo.Content.Length];
    }
    
}
