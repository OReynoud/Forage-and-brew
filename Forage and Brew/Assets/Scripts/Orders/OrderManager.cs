using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [field: AllowNesting] [field: SerializeField] public List<Order> CurrentOrders { get; private set; } = new();
    public bool IsInitialized { get; private set; }


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

        IsInitialized = false;
    }

    private void Start()
    {
        InitializeCurrentOrders();
        
        CreateOrdersFromSave();
    }
    

    private void InitializeCurrentOrders()
    {
        if (GameDontDestroyOnLoadManager.Instance.IsFirstGameSession && PotionCrateManager.Instance != null)
        {
            for (int i = 0; i < PotionCrateManager.Instance.PotionCrates.Count; i++)
            {
                CurrentOrders.Add(null);
                GameDontDestroyOnLoadManager.Instance.OrderPotions.Add(null);
                PotionCrateManager.Instance.PotionCrates[i].DisableCrate();
            }
        }

        IsInitialized = true;
    }
    
    public void CreateNewOrder(Letter letter)
    {
        bool triggerAutoPin = CurrentOrders.Count != 0;
        CodexContentManager.instance.ReceiveNewOrder(
            letter.LetterContent.Client,
            letter.LetterContent.TextContent,
            letter.LetterContent.OrderContent.RequestedPotions,
            letter.LetterContent.OrderContent.MoneyReward,
            out OrderCodexDisplayBehaviour order);
        int newOrderIndex = CurrentOrders.FindIndex(x => x == null);
        if (newOrderIndex == -1)
        {
            newOrderIndex = CurrentOrders.FindIndex(x => x.OrderDisplay == null);
        }
        //Debug.Log(CurrentOrders.Count);
        //Debug.Log(newOrderIndex);
        
        CurrentOrders[newOrderIndex] = new Order(letter, order);
        PotionCrateManager.Instance.ReactivateRightPotionCrates();
        if (triggerAutoPin)
        {
            for (int x = 0; x < CurrentOrders.Count; x++)
            {
                if (CurrentOrders[x] == null) continue;
                if (CurrentOrders[x].OrderDisplay == null) continue;
                for (int y = 0; y < CurrentOrders[x].OrderContent.RequestedPotions.Length; y++)
                {
                    if (CurrentOrders[x].OrderContent.RequestedPotions[y].IsSpecific)
                    {
                        AutoFlip.instance.recipeToPin = CurrentOrders[x].OrderContent.RequestedPotions[y].Potion;
                        return;
                    }
                }
            }

        }
        
    }

    [Button("Unit Test",EButtonEnableMode.Always)]
    public void UnitTest()
    {
        for (int x = 0; x < CurrentOrders.Count; x++)
        {
            if (CurrentOrders[x] == null) continue;
            if (CurrentOrders[x].OrderDisplay == null) continue;
            for (int y = 0; y < CurrentOrders[x].OrderContent.RequestedPotions.Length; y++)
            {
                if (CurrentOrders[x].OrderContent.RequestedPotions[y].IsSpecific)
                {
                    AutoFlip.instance.recipeToPin = CurrentOrders[x].OrderContent.RequestedPotions[y].Potion;
                    return;
                }
            }
        }
    }
    public void CreateOrdersFromSave()
    {
        foreach (Order o in CurrentOrders)
        {
            if (o == null) continue;
            if (o.RelatedLetter == null) continue;
            CodexContentManager.instance.ReceiveNewOrder(
                o.RelatedLetter.Client,
                o.RelatedLetter.TextContent,
                o.RelatedLetter.OrderContent.RequestedPotions,
                o.RelatedLetter.OrderContent.MoneyReward
                , out OrderCodexDisplayBehaviour order);
            
            CurrentOrders[CodexContentManager.instance._orderCodexDisplayBehaviours.Count - 1].OrderDisplay = order;
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
            int index = CurrentOrders[orderIndex].RelatedNarrativeBlock.ContentSo.Content.IndexOf(CurrentOrders[orderIndex].RelatedLetter);

            GameDontDestroyOnLoadManager.Instance. ThanksAndErrorLetters.Add(new Letter(
                CurrentOrders[orderIndex].RelatedLetter,
                CurrentOrders[orderIndex].RelatedNarrativeBlock));
                
            CurrentOrders[orderIndex].RelatedNarrativeBlock.CompletedLetters[index] = true;
            CurrentOrders[orderIndex].RelatedNarrativeBlock.SelfProgressionIndex++;
            Debug.Log(CurrentOrders[orderIndex].RelatedNarrativeBlock.SelfProgressionIndex);
            if (CurrentOrders[orderIndex].RelatedLetter.CanAdvanceQuestProgressionIndex)
            {
                GameDontDestroyOnLoadManager.Instance.QuestProgressionIndex++;
                if (GameDontDestroyOnLoadManager.Instance.QuestProgressionIndex > 
                    GameDontDestroyOnLoadManager.Instance.QuestProgressionIndexWatchers[GameDontDestroyOnLoadManager.Instance.FillerQuestProgression].RequiredIndex)
                {
                    GameDontDestroyOnLoadManager.Instance.FillerQuestProgression++;
                }
            }
            
            potionCrate.DisableCrate();
            CurrentOrders[orderIndex] = null;
            CodexContentManager.instance.TerminateOrder(orderIndex);
            GameDontDestroyOnLoadManager.Instance.OrderPotions[orderIndex] = null;
        }
    }
}