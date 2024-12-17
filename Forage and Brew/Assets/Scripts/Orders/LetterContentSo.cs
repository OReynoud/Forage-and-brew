using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "LetterContentSO", menuName = "Scriptable Objects/LetterContentSO")]
public class LetterContentSo : ScriptableObject
{
    [field: SerializeField] public string ClientName { get; private set; }
    [field: SerializeField] [field: ResizableTextArea] public string Description { get; private set; }
    [field: SerializeField] public PotionDemand[] RequestedPotions { get; private set; }
    [field: SerializeField] public LetterType LetterType { get; private set; }
    [field: SerializeField] public OrdersQuestLineTags Questline { get; private set; }
    [field: ShowIf("LetterType", LetterType.Orders)] [field: SerializeField] public float MoneyReward { get; private set; }
    [field: ShowIf("LetterType", LetterType.Orders)] [field: SerializeField] public int TimeToFulfill { get; private set; }
    [field: ShowIf("LetterType", LetterType.Orders)] [field: SerializeField] [field:Range(0,100)]public float moneyPenalty { get; private set; }
    [field: ShowIf("LetterType", LetterType.Orders)] [field: SerializeField] public float compensation { get; private set; }
    
}
