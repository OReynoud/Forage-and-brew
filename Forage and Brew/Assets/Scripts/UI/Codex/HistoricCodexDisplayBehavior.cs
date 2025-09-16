
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
    [Range(0, 1)] public float historicPagesRatio = 0.5f; 

    
    public override void InitHistoric(LetterContentSo originLetter, LetterContentSo successLetter)
    {
        clientNameText.text = originLetter.Client.Name;
        description1Text.text = originLetter.TextContent;
        description2Text.text = successLetter ? successLetter.TextContent : "";
        backGround1.color = backGround2.color = originLetter.Client.AssociatedColor;
        OriginLetter = originLetter;
    }    
    public void InitHistoric(LetterContentSo originLetter)
    {
        clientNameText.text = originLetter.Client.Name;
        description1Text.text = originLetter.TextContent;
        backGround2.gameObject.SetActive(false);
        backGround1.color = originLetter.Client.AssociatedColor;
        backGround1.rectTransform.offsetMax = new Vector2(-53, -30);
        backGround1.rectTransform.offsetMin = new Vector2(53, 47);
        description2Text.enabled = false;
        backGround1.transform.rotation = Quaternion.identity;
        tampon.SetActive(false);
        OriginLetter = originLetter;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            var oui = backGround1.GetComponent<RectTransform>();
            oui.offsetMin = new Vector2(oui.offsetMin.x,
            Mathf.Lerp(0,1000,historicPagesRatio));
            //oui.sizeDelta = new Vector2(oui.sizeDelta.x, )
            // oui.rect.center = Vector2.zero;
            //     oui.rect.yMax);
            // oui.;
            // Debug.Log(oui.rect.yMin);
        }
    }

    public void OnDrawGizmos()
    {
        var oui = backGround1.GetComponent<RectTransform>();
        Handles.Label(oui.anchoredPosition, "pos");
        Debug.Log(oui.offsetMin.y);
        Debug.Log(oui.sizeDelta);
        Handles.Label(oui.rect.max, "Max");
    }
}
