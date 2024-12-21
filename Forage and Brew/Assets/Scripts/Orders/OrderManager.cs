using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    public List<Order> CurrentOrders { get; } = new();
    public List<int> OrderToValidateIndices { get; } = new();
    

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
    
    public bool TryAddOrderToValidate(int orderIndex)
    {
        if (OrderToValidateIndices.Contains(orderIndex)) return false;

        if (GameDontDestroyOnLoadManager.Instance.OrderPotions[orderIndex].Any(x => x == null)) return false;
        
        OrderToValidateIndices.Add(orderIndex);
        
        return true;
    }
    
    public void CheckOrdersToValidate()
    {
        foreach (int orderToValidateIndex in OrderToValidateIndices.ToList())
        {
            bool isOrderCorrect = true;
            List<PotionValuesSo> currentOrderPotions = GameDontDestroyOnLoadManager.Instance.OrderPotions[orderToValidateIndex].ToList();
            
            foreach (PotionDemand potionDemand in CurrentOrders[orderToValidateIndex].OrderContent.RequestedPotions.Where(x => x.IsSpecific))
            {
                if (!currentOrderPotions.Contains(potionDemand.Potion))
                {
                    isOrderCorrect = false;
                    break;
                }
                    
                currentOrderPotions.Remove(potionDemand.Potion);
            }
            
            if (!isOrderCorrect) continue;
            
            List<PotionDemand> notSpecificRequestedPotions = CurrentOrders[orderToValidateIndex].OrderContent.RequestedPotions.Where(x => !x.IsSpecific).ToList();

            if (notSpecificRequestedPotions.Count > 0)
            {
                // Sort by ascending order of currentOrderPotions valid tag count
                notSpecificRequestedPotions.Sort((x, y) => 
                    currentOrderPotions.Count(potion => (potion.tags & x.ValidTag) != 0).CompareTo(
                        currentOrderPotions.Count(potion => (potion.tags & y.ValidTag) != 0)));

                if (currentOrderPotions.All(potion => (potion.tags & notSpecificRequestedPotions[0].ValidTag) == 0))
                {
                    isOrderCorrect = false;
                }
                else
                {
                    foreach (PotionDemand potionDemand in notSpecificRequestedPotions)
                    {
                        currentOrderPotions.Remove(currentOrderPotions.First(x => (x.tags & potionDemand.ValidTag) != 0));
                    }
                }
            
                if (!isOrderCorrect) continue;
            }
            
            MoneyManager.Instance.AddMoney(CurrentOrders[orderToValidateIndex].OrderContent.MoneyReward);
            CurrentOrders.RemoveAt(orderToValidateIndex);
            CodexContentManager.instance.TerminateOrder(orderToValidateIndex);
            GameDontDestroyOnLoadManager.Instance.OrderPotions.RemoveAt(orderToValidateIndex);
        }
        
        OrderToValidateIndices.Clear();
    }
}
