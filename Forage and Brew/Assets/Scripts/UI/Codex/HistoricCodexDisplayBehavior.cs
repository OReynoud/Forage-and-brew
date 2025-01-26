
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

    
    public override void InitHistoric(LetterContentSo originLetter, LetterContentSo successLetter)
    {
        clientNameText.text = originLetter.Client.Name;
        description1Text.text = originLetter.TextContent;
        description2Text.text = successLetter.TextContent;
        backGround1.color = backGround2.color = originLetter.Client.AssociatedColor;
    }
}
