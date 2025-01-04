using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    public List<Order> CurrentOrders { get; } = new();
    [field: SerializeField] [field: ReadOnly] public List<int> OrderToValidateIndices { get; set; } = new();


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


    public void CreateNewOrder(Letter letter)
    {
        CurrentOrders.Add(new Order(letter));

        CodexContentManager.instance.ReceiveNewOrder(
            letter.LetterContent.ClientName,
            letter.LetterContent.TextContent,
            letter.LetterContent.OrderContent.RequestedPotions,
            letter.LetterContent.OrderContent.MoneyReward,
            letter.LetterContent.OrderContent.TimeToFulfill);

        GameDontDestroyOnLoadManager.Instance.OrderPotions.Add(new List<PotionValuesSo>());
        for (int i = 0; i < letter.LetterContent.OrderContent.RequestedPotions.Length; i++)
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
        Debug.Log("Validating Orders");
        foreach (int orderToValidateIndex in OrderToValidateIndices.ToList())
        {
            bool isOrderCorrect = true;
            List<PotionValuesSo> currentOrderPotions =
                GameDontDestroyOnLoadManager.Instance.OrderPotions[orderToValidateIndex].ToList();

            foreach (PotionDemand potionDemand in CurrentOrders[orderToValidateIndex].OrderContent.RequestedPotions
                         .Where(x => x.IsSpecific))
            {
                if (!currentOrderPotions.Contains(potionDemand.Potion))
                {
                    isOrderCorrect = false;
                    break;
                }

                currentOrderPotions.Remove(potionDemand.Potion);
            }

            //if (!isOrderCorrect) continue;

            List<PotionDemand> notSpecificRequestedPotions = CurrentOrders[orderToValidateIndex].OrderContent
                .RequestedPotions.Where(x => !x.IsSpecific).ToList();

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
                        currentOrderPotions.Remove(
                            currentOrderPotions.First(x => (x.tags & potionDemand.ValidTag) != 0));
                    }
                }

                //if (!isOrderCorrect) continue;
            }

            //Debug.Log("Order is " + (isOrderCorrect ? "correct" : "incorrect"));
            int index = Array.IndexOf(
                CurrentOrders[orderToValidateIndex].RelatedNarrativeBlock.ContentSo.Content,
                CurrentOrders[orderToValidateIndex].RelatedLetter);

            if (isOrderCorrect)
            {
                MoneyManager.Instance.AddMoney(CurrentOrders[orderToValidateIndex].OrderContent.MoneyReward);
                GameDontDestroyOnLoadManager.Instance.ThanksAndErrorLetters.Add(new Letter(
                    CurrentOrders[orderToValidateIndex].RelatedLetter,
                    CurrentOrders[orderToValidateIndex].RelatedNarrativeBlock));
                
                CurrentOrders[orderToValidateIndex].RelatedNarrativeBlock.CompletedLetters[index] = true;
                CurrentOrders[orderToValidateIndex].RelatedNarrativeBlock.SelfProgressionIndex++;
                
                if (CurrentOrders[orderToValidateIndex].RelatedNarrativeBlock.SelfProgressionIndex >=
                    CurrentOrders[orderToValidateIndex].RelatedNarrativeBlock.CompletedLetters.Length &&
                    CurrentOrders[orderToValidateIndex].RelatedNarrativeBlock.ContentSo.CanAdvanceQuestProgressionIndex)
                {
                    GameDontDestroyOnLoadManager.Instance.QuestProgressionIndex++;
                }
            }
            else
            {
                MoneyManager.Instance.AddMoney(CurrentOrders[orderToValidateIndex].OrderContent.ErrorMoneyReward);
                GameDontDestroyOnLoadManager.Instance.ThanksAndErrorLetters.Add(new Letter(
                    CurrentOrders[orderToValidateIndex].RelatedLetter,
                    CurrentOrders[orderToValidateIndex].RelatedNarrativeBlock));
            }

            CurrentOrders.RemoveAt(orderToValidateIndex);
            CodexContentManager.instance.TerminateOrder(orderToValidateIndex);
            GameDontDestroyOnLoadManager.Instance.OrderPotions.RemoveAt(orderToValidateIndex);
        }

        OrderToValidateIndices.Clear();
    }
}