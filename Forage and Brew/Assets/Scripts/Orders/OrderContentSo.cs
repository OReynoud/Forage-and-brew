using UnityEngine;

[CreateAssetMenu(fileName = "D_OrderContent", menuName = "Orders/OrderContentSo")]
public class OrderContentSo : ScriptableObject
{
    [field: SerializeField] public PotionDemand[] RequestedPotions { get; private set; }
    [field: SerializeField] public int TimeToFulfill { get; private set; }
    [field: SerializeField] public int MoneyReward { get; private set; }
    [field: SerializeField] public int ErrorMoneyReward { get; private set; }
    [field: SerializeField] [field:Range(0f, 100f)] public float LateMoneyPenaltyPercentage { get; private set; }
}
