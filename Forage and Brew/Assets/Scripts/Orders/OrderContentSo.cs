using UnityEngine;

[CreateAssetMenu(fileName = "D_OrderContent", menuName = "Orders/OrderContentSo")]
public class OrderContentSo : ScriptableObject
{
    [field: SerializeField] public PotionDemand[] RequestedPotions { get; private set; }
    [field: SerializeField] public int MoneyReward { get; private set; }
    
    
    public void SetData(PotionDemand[] requestedPotions, int moneyReward)
    {
        RequestedPotions = requestedPotions;
        MoneyReward = moneyReward;
    }
}
