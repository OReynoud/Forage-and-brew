using System;
using System.Collections.Generic;
using UnityEngine;

public class CodexContentManager : Singleton<CodexContentManager>
{
    [Serializable]
    public class PotionDemand
    {
        public bool isSpecific;
        public PotionValuesSo potion;
        public string keywords;
        public PotionTag validTag;
        public Sprite relatedIcon;
        public bool isSubmitted = false;

        public PotionDemand(bool Specific, PotionValuesSo Potion, Sprite Icon)
        {
            potion = Potion;
            isSpecific = Specific;
            relatedIcon = Icon;
        }
        public PotionDemand(bool Specific, PotionTag Tag, Sprite Icon, string Keywords)
        {
            validTag = Tag;
            isSpecific = Specific;
            keywords = Keywords;
            relatedIcon = Icon;
        }
    }

    public RectTransform emptyPage;
    public OrderTicket orderPrefab;
    public Sprite leftEmptyPage;
    public Sprite rightEmptyPage;
    public List<OrderTicket> tickets = new List<OrderTicket>();
    private RectTransform emptyOrderPage;

    public Sprite potionIcon;
    public PotionTag testTag;

    public PotionValuesSo testPotion;

    public PotionValuesSo[] allPotions;
    public RecipeContainer[] recipes;

    public Sprite[] allDifficultySprites;
    public Sprite[] allIngredientTypeSprites;
    public Sprite[] allBrewingActionSprites;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<Sprite> tempIngredientsList = new List<Sprite>();
    void Start()
    {
        
        CharacterInputManager.Instance.OnSelectRecipe.AddListener(SelectCodexPage);
        foreach (var ticket in tickets)
        {
            ticket.gameObject.SetActive(false);
        }

        if (recipes.Length != allPotions.Length)
        {
            Debug.LogError("Number of Recipes does not match number of Potions SO, some recipe pages may be broken");
        }

        
        for (int i = 0; i < recipes.Length; i++)
        {
            foreach (var t in allPotions[i].TemperatureChallengeIngredients)
            {
                foreach (var cookedIngredient in t.CookedIngredients)
                {
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (cookedIngredient.IsAType)
                    {
                        tempIngredientsList.Add(allIngredientTypeSprites[(int)cookedIngredient.IngredientType]);
                    }
                    else
                    {
                        tempIngredientsList.Add(cookedIngredient.Ingredient.icon);
                    }
                }
            }

            recipes[i].InitPage(
                tempIngredientsList.ToArray(),
                allPotions[i]);
            tempIngredientsList.Clear();
        }
        //DebugTickets();
    }




    // public void ReceiveNewOrder(string clientName,string orderDescription, PotionDemand[] potionsRequested, float moneyReward, int timeToComplete)
    // {
    //     for (int i = 0; i < tickets.Count; i++)
    //     {
    //         if (tickets[i].hasAnOrder)
    //             continue;
    //         tickets[i].gameObject.SetActive(true);
    //         tickets[i].InitializeOrder(clientName, orderDescription, potionsRequested, moneyReward, timeToComplete);
    //         break;
    //     }
    // }
    public void ReceiveNewOrder(string clientName,string orderDescription, PotionDemand[] potionsRequested, float moneyReward, int timeToComplete)
    {
        if (!emptyOrderPage)
        {
            var pageContainer= Instantiate(emptyPage,transform);
            emptyOrderPage = Instantiate(emptyPage,transform);
            var order = Instantiate(orderPrefab,pageContainer);
            pageContainer.anchoredPosition = new Vector2(1500,0);
            emptyOrderPage.anchoredPosition = new Vector2(1500,0);
            
            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,new Book.BookPage(rightEmptyPage,emptyOrderPage));
            
            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,new Book.BookPage(leftEmptyPage,pageContainer));
            
            tickets.Add(order);
            order.InitializeOrder(clientName,orderDescription,potionsRequested,moneyReward,timeToComplete);
            for (int i = 1; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
            {
                AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
            }
        }
        else
        {
            var order = Instantiate(orderPrefab,emptyOrderPage);
            tickets.Add(order);
            order.InitializeOrder(clientName,orderDescription,potionsRequested,moneyReward,timeToComplete);
            emptyOrderPage = null;
        }
        AutoFlip.instance.ControledBook.UpdateSprites();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="side"> true = right, false = left</param>
    public void SelectCodexPage(bool side)
    {
        if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[0].index &&
            AutoFlip.instance.ControledBook.currentPage < AutoFlip.instance.ControledBook.bookMarks[1].index )
        {
            //Select Order
        }
        else if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[1].index &&
            AutoFlip.instance.ControledBook.currentPage < AutoFlip.instance.ControledBook.bookMarks[2].index )
        {
            var recipeIndex = AutoFlip.instance.ControledBook.currentPage -
                              AutoFlip.instance.ControledBook.bookMarks[1].index;
            recipeIndex = side ? recipeIndex + 1 : recipeIndex;
            Debug.Log(recipeIndex);
            if (PinnedRecipe.instance.pinnedRecipe.Name == recipes[recipeIndex].storedPotion.Name)
            {
                
                Debug.Log("Selected same recipe, unpinning");
                PinnedRecipe.instance.UnpinRecipe();
                return;
            }
            PinnedRecipe.instance.PinRecipe(recipes[recipeIndex].storedPotion,recipes[recipeIndex].potionIngredients);
            Debug.Log("Pinned recipe: " + recipes[recipeIndex].storedPotion.Name);
        }
        else
        {
            PinnedRecipe.instance.UnpinRecipe();
            Debug.Log("Not in recipe pages");
        }
        
    }
    
    
    
    
    
    
    
    public void DebugTickets()
    {
        var temp = new List<PotionDemand>();
        temp.Add(new PotionDemand(true,testPotion,potionIcon));
        tickets[0].gameObject.SetActive(true);
        tickets[0].InitializeOrder("Jean-Eude","Je me suis coupé le doigt, tu peux me passer de la pommade s'il te plait?",temp.ToArray(), 10, 3);
        temp.Clear();
        temp.Add(new PotionDemand(false,testTag,potionIcon,"Something against a fever"));
        tickets[1].gameObject.SetActive(true);
        tickets[1].InitializeOrder("Paul","J'ai de la fièvre, t'as quelque chose pour m'aider?",temp.ToArray(), 15, 3);
        temp.Clear();
        temp.Add(new PotionDemand(true,testPotion,potionIcon));
        temp.Add(new PotionDemand(true,testPotion,potionIcon));
        tickets[2].gameObject.SetActive(true);
        tickets[2].InitializeOrder("Marie","J'ai besoin de comparer la saveur de ces deux jus, peux-tu me les préparer?",temp.ToArray(),25, 3);
    }
}
