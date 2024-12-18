using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    public List<Order> CurrentOrders { get; private set; } = new();
    public List<int> OrderToValidateIndices { get; private set; } = new();
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    
    
    public void CreateNewOrder(LetterContentSo letterContent)
    {
        CurrentOrders.Add(new Order(letterContent.OrderContent));
        
        CodexContentManager.instance.ReceiveNewOrder(                
            letterContent.ClientName,
            letterContent.TextContent,
            letterContent.OrderContent.RequestedPotions,
            letterContent.OrderContent.MoneyReward,
            letterContent.OrderContent.TimeToFulfill);
        
        GameDontDestroyOnLoadManager.Instance.OrderPotions.Add(new List<PotionValuesSo>());
        for (int i = 0; i < letterContent.OrderContent.RequestedPotions.Length; i++)
        {
            GameDontDestroyOnLoadManager.Instance.OrderPotions[^1].Add(null);
        }
    }
    
    public void CheckOrdersToValidate()
    {
        
    }
}
