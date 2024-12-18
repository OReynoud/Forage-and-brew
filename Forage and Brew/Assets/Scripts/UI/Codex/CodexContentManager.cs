using System.Collections.Generic;
using UnityEngine;

public class CodexContentManager : Singleton<CodexContentManager>
{
    public RectTransform emptyPage;
    public OrderCodexDisplayBehaviour orderPrefab;
    public Sprite leftEmptyPage;
    public Sprite rightEmptyPage;
    private readonly List<OrderCodexDisplayBehaviour> _orderCodexDisplayBehaviours = new();
    private RectTransform emptyOrderPage;
    private int emptyOrderPageIndex;

    public PotionTag testTag;

    public PotionValuesSo testPotion;

    public PotionValuesSo[] allPotions;
    public RecipeContainer[] recipes;

    public Sprite[] allDifficultySprites;
    public Sprite[] allIngredientTypeSprites;
    public Sprite[] allBrewingActionSprites;

    private List<Sprite> tempIngredientsList = new();


    private void Start()
    {
        CharacterInputManager.Instance.OnSelectRecipe.AddListener(SelectCodexPage);
        foreach (var ticket in _orderCodexDisplayBehaviours)
        {
            ticket.gameObject.SetActive(false);
        }

        if (recipes.Length != allPotions.Length)
        {
            Debug.LogError("Number of Recipes does not match number of Potions SO, some recipe pages may be broken");
        }
        
        for (int i = 0; i < recipes.Length; i++)
        {
            foreach (TemperatureChallengeIngredients t in allPotions[i].TemperatureChallengeIngredients)
            {
                foreach (CookedIngredientForm cookedIngredient in t.CookedIngredients)
                {
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

            recipes[i].InitPage(tempIngredientsList.ToArray(), allPotions[i]);
            tempIngredientsList.Clear();
        }
        
        // DebugTickets();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TerminateOrder(1);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TerminateOrder(2);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TerminateOrder(3);
        }
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
    
    public void ReceiveNewOrder(string clientName,string orderDescription, PotionDemand[] potionsRequested, int moneyReward, int timeToComplete)
    {
        if (!emptyOrderPage)
        {
            var pageContainer= Instantiate(emptyPage,transform);
            
            emptyOrderPage = Instantiate(emptyPage,transform);
            var order = Instantiate(orderPrefab,pageContainer);
            pageContainer.anchoredPosition = new Vector2(1500,0);
            emptyOrderPage.anchoredPosition = new Vector2(1500,0);
            
            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,new Book.BookPage(rightEmptyPage,emptyOrderPage));
            emptyOrderPage.name = "Page " + (AutoFlip.instance.ControledBook.bookMarks[1].index + 1);
            
            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,new Book.BookPage(leftEmptyPage,pageContainer));
            pageContainer.name = "Page " + AutoFlip.instance.ControledBook.bookMarks[1].index;

            emptyOrderPageIndex = AutoFlip.instance.ControledBook.bookMarks[1].index + 1;
            _orderCodexDisplayBehaviours.Add(order);
            order.InitializeOrder(clientName,orderDescription,potionsRequested,moneyReward,timeToComplete,AutoFlip.instance.ControledBook.bookMarks[1].index);
            
            for (int i = 1; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
            {
                AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
            }
        }
        else
        {
            var order = Instantiate(orderPrefab,emptyOrderPage);
            _orderCodexDisplayBehaviours.Add(order);
            order.InitializeOrder(clientName,orderDescription,potionsRequested,moneyReward,timeToComplete,AutoFlip.instance.ControledBook.bookMarks[1].index - 1);
            emptyOrderPage = null;
            //Debug.Log(AutoFlip.instance.ControledBook.bookPages[AutoFlip.instance.ControledBook.bookMarks[1].index - 1].UIComponent);
        }
        
        AutoFlip.instance.ControledBook.UpdateSprites();
    }

    public void TerminateOrder(int index)
    {
        if (!emptyOrderPage)
        {
            emptyOrderPage = AutoFlip.instance.ControledBook.bookPages[index].UIComponent;
            emptyOrderPageIndex = index;
            Destroy(emptyOrderPage.GetChild(0).gameObject);
        }
        else
        {
            emptyOrderPage = null;
            
            Debug.Log("Removed page " + AutoFlip.instance.ControledBook.bookPages[index].UIComponent.name);
            Destroy(AutoFlip.instance.ControledBook.bookPages[index].UIComponent.gameObject);
            AutoFlip.instance.ControledBook.bookPages.RemoveAt(index);
            
            if (emptyOrderPageIndex > index)
                emptyOrderPageIndex--;
            
            Debug.Log("Removed page " + AutoFlip.instance.ControledBook.bookPages[emptyOrderPageIndex].UIComponent.name);
            Destroy(AutoFlip.instance.ControledBook.bookPages[emptyOrderPageIndex].UIComponent.gameObject);
            AutoFlip.instance.ControledBook.bookPages.RemoveAt(emptyOrderPageIndex);
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
            // Select Order
            int orderIndex = AutoFlip.instance.ControledBook.currentPage -
                             AutoFlip.instance.ControledBook.bookMarks[0].index;
            orderIndex = side ? orderIndex + 1 : orderIndex;
            
            OrderManager.Instance.OrderToValidateIndices.Add(orderIndex);
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
                
                //Debug.Log("Selected same recipe, unpinning");
                PinnedRecipe.instance.UnpinRecipe();
                return;
            }
            PinnedRecipe.instance.PinRecipe(recipes[recipeIndex].storedPotion,recipes[recipeIndex].potionIngredients);
            //Debug.Log("Pinned recipe: " + recipes[recipeIndex].storedPotion.Name);
        }
        else
        {
            PinnedRecipe.instance.UnpinRecipe();
            //Debug.Log("Not in recipe pages");
        }
        
    }
    
    
    public void DebugTickets()
    {
        var temp = new List<PotionDemand>();
        temp.Add(new PotionDemand(true,testPotion));
        
        ReceiveNewOrder("Jean-Eude","Je me suis coupé le doigt, tu peux me passer de la pommade s'il te plait?",temp.ToArray(), 10, 3);
        temp.Clear();
        
        temp.Add(new PotionDemand(false,testTag,"Something against a fever"));
        ReceiveNewOrder("Paul","J'ai de la fièvre, t'as quelque chose pour m'aider?",temp.ToArray(), 15, 3);
        temp.Clear();
        
        temp.Add(new PotionDemand(true,testPotion));
        temp.Add(new PotionDemand(true,testPotion));
        ReceiveNewOrder("Marie","J'ai besoin de comparer la saveur de ces deux jus, peux-tu me les préparer?",temp.ToArray(),25, 3);
    }
}
