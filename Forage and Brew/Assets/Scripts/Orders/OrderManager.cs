using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [field: AllowNesting] [field: SerializeField] public List<Order> CurrentOrders { get; } = new();
    public bool isInitialized { get; set; }


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        isInitialized = false;
    }

    private void Start()
    {
        InitializeCurrentOrders();
        
        CreateOrdersFromSave();
    }
    

    private void InitializeCurrentOrders()
    {
        if (GameDontDestroyOnLoadManager.Instance.IsFirstGameSession)
        {
            for (int i = 0; i < PotionCrateManager.Instance.PotionCrates.Count; i++)
            {
                CurrentOrders.Add(null);
                GameDontDestroyOnLoadManager.Instance.OrderPotions.Add(null);
                PotionCrateManager.Instance.PotionCrates[i].DisableCrate();
            }
        }

        isInitialized = true;
    }
    
    public void CreateNewOrder(Letter letter)
    {
        bool triggerAutoPin = CurrentOrders.Count == 0;
        CodexContentManager.instance.ReceiveNewOrder(
            letter.LetterContent.Client,
            letter.LetterContent.TextContent,
            letter.LetterContent.OrderContent.RequestedPotions,
            letter.LetterContent.OrderContent.MoneyReward,
            letter.LetterContent.OrderContent.TimeToFulfill, out OrderCodexDisplayBehaviour order);

        int newOrderIndex = CurrentOrders.FindIndex(x => x == null);
        //Debug.Log(CurrentOrders.Count);
        //Debug.Log(newOrderIndex);
        CurrentOrders[newOrderIndex] = new Order(letter, order);
        
        if (triggerAutoPin)
        {
            if (CurrentOrders[0].OrderContent.RequestedPotions[0].IsSpecific)
            {
                AutoFlip.instance.recipeToPin = CurrentOrders[0].OrderContent.RequestedPotions[0].Potion;
            }
        }
        
        PotionCrateManager.Instance.ReactivateRightPotionCrates();
    }
    
    public void CreateOrdersFromSave()
    {
        foreach (Order o in CurrentOrders)
        {
            if (o == null) continue;
            
            CodexContentManager.instance.ReceiveNewOrder(
                o.RelatedLetter.Client,
                o.RelatedLetter.TextContent,
                o.RelatedLetter.OrderContent.RequestedPotions,
                o.RelatedLetter.OrderContent.MoneyReward,
                o.RelatedLetter.OrderContent.TimeToFulfill, out OrderCodexDisplayBehaviour order);
        }

        CodexContentManager.instance.pageIndexesToCheck.Clear();
    }

    public void CheckOrdersToValidate()
    {
        foreach (PotionCrateBehaviour potionCrate in PotionCrateManager.Instance.PotionCrates)
        {
            if (!potionCrate.IsFulfilled) continue;
            
            int orderIndex = CurrentOrders.FindIndex(x => x != null && x.OrderContent == potionCrate.OrderContentSo);
            
            if (orderIndex < 0)
            {
                Debug.LogError("Order not found for the potion crate: " + potionCrate.name);
                continue;
            }

            //Debug.Log("Order is " + (isOrderCorrect ? "correct" : "incorrect"));
            int index = Array.IndexOf(
                CurrentOrders[orderIndex].RelatedNarrativeBlock.ContentSo.Content,
                CurrentOrders[orderIndex].RelatedLetter);

            GameDontDestroyOnLoadManager.Instance. ThanksAndErrorLetters.Add(new Letter(
                CurrentOrders[orderIndex].RelatedLetter,
                CurrentOrders[orderIndex].RelatedNarrativeBlock, CurrentOrders[orderIndex].OrderDisplay.daysLeftToComplete >=  0));
                
            CurrentOrders[orderIndex].RelatedNarrativeBlock.CompletedLetters[index] = true;
            CurrentOrders[orderIndex].RelatedNarrativeBlock.SelfProgressionIndex++;
                
            if (CurrentOrders[orderIndex].RelatedNarrativeBlock.SelfProgressionIndex >=
                CurrentOrders[orderIndex].RelatedNarrativeBlock.CompletedLetters.Length &&
                CurrentOrders[orderIndex].RelatedNarrativeBlock.ContentSo.CanAdvanceQuestProgressionIndex)
            {
                GameDontDestroyOnLoadManager.Instance.QuestProgressionIndex++;
            }
            
            potionCrate.DisableCrate();
            CurrentOrders[orderIndex] = null;
            CodexContentManager.instance.TerminateOrder(orderIndex);
            GameDontDestroyOnLoadManager.Instance.OrderPotions[orderIndex] = null;
        }
    }
}