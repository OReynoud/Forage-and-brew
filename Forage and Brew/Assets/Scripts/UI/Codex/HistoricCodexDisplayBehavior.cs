using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class HistoricCodexDisplayBehavior : PageBehavior
{
    public TextMeshProUGUI clientNameText;
    public Image backGround1;
    public Image backGround2;
    public TextMeshProUGUI description1Text;
    public TextMeshProUGUI description2Text;
    public GameObject tampon;
    public LetterContentSo OriginLetter;


    public float backGroundSpacing;
    public float backGroundRim;
    [Range(0, 1)] public float historicPagesRatio = 0.5f;


    public override void InitHistoric(LetterContentSo originLetter, LetterContentSo successLetter)
    {
        clientNameText.text = originLetter.Client.Name;
        description1Text.text = originLetter.TextContent;
        description2Text.text = successLetter ? successLetter.TextContent : "";
        backGround1.color = backGround2.color = originLetter.Client.AssociatedColor;
        OriginLetter = originLetter;
        historicPagesRatio = 0.5f + (float)(description1Text.text.Length - description2Text.text.Length) / (description1Text.text.Length + description2Text.text.Length);
        AdjustBackgrounds();
    }

    public void InitHistoric(LetterContentSo originLetter)
    {
        clientNameText.text = originLetter.Client.Name;
        description1Text.text = originLetter.TextContent;
        backGround2.gameObject.SetActive(false);
        backGround1.color = originLetter.Client.AssociatedColor;
        //backGround1.rectTransform.offsetMax = new Vector2(-53, -30);
        //backGround1.rectTransform.offsetMin = new Vector2(53, 47);
        description2Text.enabled = false;
        backGround1.transform.rotation = Quaternion.identity;
        tampon.SetActive(false);
        OriginLetter = originLetter;
        historicPagesRatio = 0.8f;
        AdjustBackgrounds();
    }

    private void OnValidate()
    {
        AdjustBackgrounds();

        //oui.sizeDelta = new Vector2(oui.sizeDelta.x, )
        // oui.rect.center = Vector2.zero;
        //     oui.rect.yMax);
        // oui.;
        // Debug.Log(oui.rect.yMin);
    }

    private void AdjustBackgrounds()
    {
        var backGround1Rect = backGround1.GetComponent<RectTransform>();
        backGround1Rect.offsetMin = new Vector2(backGround1Rect.offsetMin.x,
            Mathf.Lerp(0, -1000, historicPagesRatio) - backGroundSpacing);
        backGround1Rect.offsetMax = new Vector2(backGround1Rect.offsetMax.x,
            -backGroundRim);

        var backGround2Rect = backGround2.GetComponent<RectTransform>();
        backGround2Rect.offsetMax = new Vector2(backGround2Rect.offsetMax.x,
            Mathf.Lerp(1000, 0, historicPagesRatio) + backGroundSpacing);
        backGround2Rect.offsetMin = new Vector2(backGround2Rect.offsetMin.x,
            backGroundRim);
    }
}