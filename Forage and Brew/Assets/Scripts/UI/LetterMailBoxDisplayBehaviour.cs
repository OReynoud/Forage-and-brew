using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LetterMailBoxDisplayBehaviour : MonoBehaviour
{
    public TextMeshProUGUI clientNameText;
    
    public TextMeshProUGUI descriptionText;
    
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI timeText;

    private LetterContentSo letterContent;
    public List<PotionDemand> potionsDemanded = new();

    public Image[] potionImages;
    public TextMeshProUGUI[] potionKeywords;
    public int moneyReward;
    public int daysLeftToComplete;

    public bool IsPassed { get; private set; }
    public bool IsMoving { get; private set; }
    public LetterType letterType;
    [FormerlySerializedAs("animIndex")] public float animTime;

    public AnimationClip animClip;

    
    public void InitLetter(LetterContentSo newLetterContent)
    {
        letterContent = newLetterContent;
        clientNameText.text = letterContent.ClientName;
        descriptionText.text = letterContent.TextContent;
        moneyReward = letterContent.OrderContent.MoneyReward;
        moneyText.text = moneyReward.ToString();
        daysLeftToComplete = letterContent.OrderContent.TimeToFulfill;
        timeText.text = daysLeftToComplete.ToString();
        potionsDemanded.Clear();
        potionsDemanded.AddRange(letterContent.OrderContent.RequestedPotions);

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

        letterType = letterContent.LetterType;
    }

    public void AnimateLetter(bool hasToPass)
    {
        IsPassed = hasToPass;
        animTime = MailBoxBehaviour.instance.animCurve.length;
        IsMoving = true;
    }
    
    public void Update()
    {
        if (animTime <= 0f) return;
        
        if (IsPassed)
        {
            animTime -= Time.deltaTime * MailBoxBehaviour.instance.animSpeed;
            animClip.SampleAnimation(gameObject, MailBoxBehaviour.instance.animCurve.Evaluate(Mathf.Clamp(
                MailBoxBehaviour.instance.animCurve.length - animTime, 0f, MailBoxBehaviour.instance.animCurve.length)));

            if (animTime <= 0f)
            {
                IsMoving = false;
            }
        }
    }
}
