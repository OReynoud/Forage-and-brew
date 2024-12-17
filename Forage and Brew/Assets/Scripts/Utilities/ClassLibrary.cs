using System;
using UnityEngine;

[Serializable]
public class Letter
{
    public LetterContainer associatedLetter;
    public LetterContentSo LetterContent;
    public int days;

    public Letter(LetterContentSo letter, int delay )
    {
        LetterContent = letter;
        days = delay;
    }
}

[Serializable]
public class PotionDemand
{
    public bool isSpecific;
    public PotionValuesSo potion;
    public string keywords;
    public PotionTag validTag;
    public Sprite relatedIcon;
    public bool isSubmitted = false;

    public PotionDemand(bool newIsSpecific, PotionValuesSo newPotion, Sprite newIcon)
    {
        potion = newPotion;
        isSpecific = newIsSpecific;
        relatedIcon = newIcon;
    }
    public PotionDemand(bool newIsSpecific, PotionTag newTag, Sprite newIcon, string newKeywords)
    {
        validTag = newTag;
        isSpecific = newIsSpecific;
        keywords = newKeywords;
        relatedIcon = newIcon;
    }
}
