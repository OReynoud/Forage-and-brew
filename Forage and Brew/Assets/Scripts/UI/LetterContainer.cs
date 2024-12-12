using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterContainer : MonoBehaviour
{
    public TextMeshProUGUI clientNameText;

    public Image clientPortraitImage;
    
    public TextMeshProUGUI descriptionText;
    
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI timeText;

    public List<CodexContentManager.PotionDemand> potionsDemanded = new List<CodexContentManager.PotionDemand>();

    public Image[] potionImages;
    public TextMeshProUGUI[] potionKeywords;
    public float moneyReward;
    public int daysLeftToComplete;


    public bool isMoved;
    public LetterType letterType;
    public float animIndex;

    public AnimationClip animClip;

    public void InitLetter(string clientName, string letterDescription, CodexContentManager.PotionDemand[] Potions, float money, int daysToComplete, LetterType typeOfLetter)
    {
        clientNameText.text = clientName;
        descriptionText.text = letterDescription;
        moneyText.text = money.ToString(CultureInfo.InvariantCulture);
        timeText.text = daysToComplete.ToString();
        potionsDemanded.Clear();
        potionsDemanded.AddRange(Potions);
        moneyReward = money;
        daysLeftToComplete = daysToComplete;

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

        letterType = typeOfLetter;
    }

    public void AnimateLetter(bool toMove)
    {
        isMoved = toMove;
        
        animIndex = 0;

    }
    public void Update()
    {
        if (isMoved)
        {
            animIndex += Time.deltaTime * MailBox.instance.animSpeed;
            animClip.SampleAnimation(gameObject,MailBox.instance.animCurve.Evaluate(animIndex));
        }
    }
}
