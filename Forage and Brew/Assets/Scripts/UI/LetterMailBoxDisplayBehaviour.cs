using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterMailBoxDisplayBehaviour : PageBehavior
{
    public TextMeshProUGUI clientNameText;
    public Image letterBackground;
    public Image bills;
    public GameObject moneyIcon;
    
    public TextMeshProUGUI descriptionText;
    
    public TextMeshProUGUI moneyTextOrder;
    public TextMeshProUGUI moneyTextThanks;

    private LetterContentSo letterContent;
    public List<PotionDemand> potionsDemanded = new();

    public LayoutGroup mainContentLayoutGroup;
    public GridLayoutGroup potionsLayoutGroup;
    public Image[] liquidImages;
    public Image[] potionImages;
    public TextMeshProUGUI[] potionNames;
    public TextMeshProUGUI[] potionKeywords;
    public int moneyReward;

    public bool IsPassed;
    public bool IsMoving;
    public LetterType letterType;
    public float animTime;

    public AnimationClip animClip;

    
    public override void InitLetter(LetterContentSo newLetterContent)
    {
        letterContent = newLetterContent;
        letterType = letterContent.LetterType;
        
        clientNameText.text = letterContent.Client.Name;
        descriptionText.text = letterContent.TextContent;
        letterBackground.color = letterContent.Client.AssociatedColor;

        foreach (var potionImage in potionImages)
        {
            potionImage.transform.parent.gameObject.SetActive(false);
        }

        foreach (var keyword in potionKeywords)
        {
            keyword.transform.parent.gameObject.SetActive(false);
        }
        
        if (letterType != LetterType.Orders)
        {
            moneyTextOrder.transform.parent.gameObject.SetActive(false);
            return;
        }

        bills.gameObject.SetActive(letterType is LetterType.Thanks or LetterType.Gift);
        moneyIcon.SetActive(letterType is LetterType.Orders);
        moneyReward = letterContent.OrderContent.MoneyReward;
        moneyTextOrder.text = moneyReward.ToString();
        moneyTextThanks.text = moneyReward.ToString();
        potionsDemanded.Clear();
        potionsDemanded.AddRange(letterContent.OrderContent.RequestedPotions);

        if (potionsDemanded.Count == 4)
        {
            potionsLayoutGroup.constraintCount = 2;
        }
        
        for (int i = 0; i < potionsDemanded.Count; i++)
        {
            if (potionsDemanded[i].IsSpecific)
            {
                potionImages[i].transform.parent.gameObject.SetActive(true);
                liquidImages[i].sprite = potionsDemanded[i].Potion.PotionDifficulty.LiquidSprite;
                liquidImages[i].color = potionsDemanded[i].Potion.SpriteLiquidColor;
                potionImages[i].sprite = potionsDemanded[i].Potion.PotionDifficulty.PotionSprite;
                potionNames[i].text = potionsDemanded[i].Potion.Name;
            }
            else
            {
                potionKeywords[i].transform.parent.gameObject.SetActive(true);
                potionKeywords[i].text = potionsDemanded[i].Keywords;
            }
        }

        letterType = letterContent.LetterType;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainContentLayoutGroup.transform as RectTransform);
    }

    public void AnimateLetter(bool hasToPass)
    {
        IsPassed = hasToPass;
        animTime = 1;
        IsMoving = true;
    }
    
    public void Update()
    {
        if (animTime <= 0f) return;
        
        if (IsPassed)
        {
            animTime -= Time.deltaTime * MailBoxBehaviour.instance.animSpeed;
            animClip.SampleAnimation(gameObject, MailBoxBehaviour.instance.animCurve.Evaluate(Mathf.Clamp(
                1 - animTime, 0f, 1)));

            if (animTime <= 0f)
            {
                IsMoving = false;
            }
        }
    }
}
