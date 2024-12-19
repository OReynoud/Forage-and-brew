using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "D_LetterContent", menuName = "Letters/LetterContentSo")]
public class LetterContentSo : ScriptableObject
{
    [field: SerializeField] public string ClientName { get; private set; }
    [field: SerializeField] [field: ResizableTextArea] public string TextContent { get; private set; }
    [field: SerializeField] public LetterType LetterType { get; private set; }
    // [field: SerializeField] public OrdersQuestLineTags QuestLine { get; private set; }
    [field: Expandable] [field: ShowIf("LetterType", LetterType.Orders)] [field: SerializeField] public OrderContentSo OrderContent { get; private set; }
    
}
