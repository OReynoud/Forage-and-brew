
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoricCodexDisplayBehavior : PageBehavior
{
    
    public TextMeshProUGUI clientNameText;
    public Image backGround1;
    public Image backGround2;
    public TextMeshProUGUI description1Text;
    public TextMeshProUGUI description2Text;
    public GameObject tampon;

    
    public override void InitHistoric(LetterContentSo originLetter, LetterContentSo successLetter)
    {
        clientNameText.text = originLetter.Client.Name;
        description1Text.text = originLetter.TextContent;
        description2Text.text = successLetter ? successLetter.TextContent : "";
        backGround1.color = backGround2.color = originLetter.Client.AssociatedColor;
    }    
    public void InitHistoric(LetterContentSo originLetter)
    {
        clientNameText.text = originLetter.Client.Name;
        description1Text.text = originLetter.TextContent;
        backGround2.gameObject.SetActive(false);
        backGround1.color = originLetter.Client.AssociatedColor;
        backGround1.rectTransform.offsetMax = new Vector2(-33, -10);
        backGround1.rectTransform.offsetMin = new Vector2(33, 37);
        description2Text.enabled = false;
        backGround1.transform.rotation = Quaternion.identity;
        tampon.SetActive(false);
    }
}
