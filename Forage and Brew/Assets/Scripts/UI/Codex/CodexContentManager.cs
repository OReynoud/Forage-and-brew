using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class CodexContentManager : Singleton<CodexContentManager>
{
    [Foldout("References")] public OrderCodexDisplayBehaviour[] orderPrefabs;
    [Foldout("References")] public Sprite leftEmptyPage;
    [Foldout("References")] public Sprite rightEmptyPage;
    [Foldout("References")] public Sprite leftRecipePage;
    [Foldout("References")] public Sprite rightRecipePage;
    [Foldout("References")] public RectTransform emptyPage;
    [Foldout("References")] public Sprite[] allBrewingActionSprites;
    [Foldout("References")] public RecipeCodexDisplay recipeDisplayPrefab;
    
    //Orders Management
    private readonly List<OrderCodexDisplayBehaviour> _orderCodexDisplayBehaviours = new();
    private RectTransform emptyOrderPage;
    private int emptyOrderPageIndex;
    
    [BoxGroup("Recipe display")] [SerializeField] private PotionListSo potionList;
    [BoxGroup("Recipe display")] [ReadOnly] public List<RecipeCodexDisplay> recipes = new ();
    
    [Foldout("Debug")] public PotionTag testTag;

    [Foldout("Debug")] public PotionValuesSo testPotion;
    
    [Foldout("Debug")] private List<Sprite> tempIngredientsList = new();
    
    
    private void Start()
    {
        CharacterInputManager.Instance.OnSelectRecipe.AddListener(SelectCodexPage);
        foreach (var ticket in _orderCodexDisplayBehaviours)
        {
            ticket.gameObject.SetActive(false);
        }
        

        for (int i = potionList.Potions.Length - 1; i >= 0; i--)
        {
            var newRecipe = Instantiate(recipeDisplayPrefab, Vector3.down * 10000, Quaternion.identity,transform);
            recipes.Add(newRecipe);
            foreach (TemperatureChallengeIngredients t in potionList.Potions[i].TemperatureChallengeIngredients)
            {
                foreach (CookedIngredientForm cookedIngredient in t.CookedIngredients)
                {
                    if (cookedIngredient.IsAType)
                    {
                        tempIngredientsList.Add(cookedIngredient.IngredientType.Icon);
                    }
                    else
                    {
                        tempIngredientsList.Add(cookedIngredient.Ingredient.icon);
                    }
                }
            }

            newRecipe.InitPage(tempIngredientsList.ToArray(), potionList.Potions[i]);
            tempIngredientsList.Clear();
            InsertRecipePages(newRecipe.leftPage, newRecipe.rightPage);
        }

        // DebugTickets();
    }

    private void Update()
    {

    }



    public void InsertRecipePages(RectTransform LeftPage, RectTransform RightPage)
    {
        AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,
            new Book.BookPage(rightRecipePage, RightPage));
        AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,
            new Book.BookPage(leftRecipePage, LeftPage));

        for (int i = 2; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
        {
            AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
        }
    }

    public void ReceiveNewOrder(string clientName, string orderDescription, PotionDemand[] potionsRequested,
        int moneyReward, int timeToComplete)
    {
        if (!emptyOrderPage)
        {
            var pageContainer = Instantiate(emptyPage, transform);

            emptyOrderPage = Instantiate(emptyPage, transform);
            var order = Instantiate(orderPrefabs[Random.Range(0, orderPrefabs.Length)], pageContainer);
            pageContainer.anchoredPosition = new Vector2(1500, 0);
            emptyOrderPage.anchoredPosition = new Vector2(1500, 0);

            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,
                new Book.BookPage(rightEmptyPage, emptyOrderPage));
            emptyOrderPage.name = "Page " + (AutoFlip.instance.ControledBook.bookMarks[0].index + 1);

            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,
                new Book.BookPage(leftEmptyPage, pageContainer));
            pageContainer.name = "Page " + AutoFlip.instance.ControledBook.bookMarks[0].index;

            emptyOrderPageIndex = AutoFlip.instance.ControledBook.bookMarks[1].index + 1;
            _orderCodexDisplayBehaviours.Add(order);
            order.InitializeOrder(clientName, orderDescription, potionsRequested, moneyReward, timeToComplete,
                AutoFlip.instance.ControledBook.bookMarks[1].index);

            for (int i = 1; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
            {
                AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
            }
        }
        else
        {
            var order = Instantiate(orderPrefabs[Random.Range(0, orderPrefabs.Length)], emptyOrderPage);
            _orderCodexDisplayBehaviours.Add(order);
            order.InitializeOrder(clientName, orderDescription, potionsRequested, moneyReward, timeToComplete,
                AutoFlip.instance.ControledBook.bookMarks[1].index - 1);
            emptyOrderPage = null;
            //Debug.Log(AutoFlip.instance.ControledBook.bookPages[AutoFlip.instance.ControledBook.bookMarks[1].index - 1].UIComponent);
        }

        AutoFlip.instance.ControledBook.UpdateSprites();
    }

    public void TerminateOrder(int index)
    {
        index++;
        
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

            Debug.Log("Removed page " +
                      AutoFlip.instance.ControledBook.bookPages[emptyOrderPageIndex].UIComponent.name);
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
        Debug.Log("Order Select Input");
        if (!CharacterInputManager.Instance.showCodex) return;
        
        if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[0].index &&
            AutoFlip.instance.ControledBook.currentPage < AutoFlip.instance.ControledBook.bookMarks[1].index)
        {
            if (emptyOrderPage && side && AutoFlip.instance.ControledBook.currentPage == emptyOrderPageIndex) return;
            // Select Order
            int orderIndex = AutoFlip.instance.ControledBook.currentPage -
                             AutoFlip.instance.ControledBook.bookMarks[0].index;
            orderIndex = side ? orderIndex : orderIndex - 1;

            OrderManager.Instance.TryAddOrderToValidate(orderIndex);
        }
        else if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[1].index &&
                 AutoFlip.instance.ControledBook.currentPage < AutoFlip.instance.ControledBook.bookMarks[2].index)
        {
            int recipeIndex = Mathf.FloorToInt((AutoFlip.instance.ControledBook.currentPage -
                                                AutoFlip.instance.ControledBook.bookMarks[1].index) * 0.5f);
            Debug.Log(recipeIndex);
            if (PinnedRecipe.instance.pinnedRecipe)
            {
                if (PinnedRecipe.instance.pinnedRecipe.Name == recipes[recipeIndex].storedPotion.Name)
                {
                    Debug.Log("Selected same recipe, unpinning");
                    PinnedRecipe.instance.UnpinRecipe();
                    return;
                }
            }

            PinnedRecipe.instance.PinRecipe(recipes[recipeIndex].storedPotion, recipes[recipeIndex].potionIngredients);
            Debug.Log("Pinned recipe: " + recipes[recipeIndex].storedPotion.Name);
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
        temp.Add(new PotionDemand(true, testPotion));

        ReceiveNewOrder("Jean-Eude", "Je me suis coupé le doigt, tu peux me passer de la pommade s'il te plait?",
            temp.ToArray(), 10, 3);
        temp.Clear();

        temp.Add(new PotionDemand(false, testTag, "Something against a fever"));
        ReceiveNewOrder("Paul", "J'ai de la fièvre, t'as quelque chose pour m'aider?", temp.ToArray(), 15, 3);
        temp.Clear();

        temp.Add(new PotionDemand(true, testPotion));
        temp.Add(new PotionDemand(true, testPotion));
        ReceiveNewOrder("Marie", "J'ai besoin de comparer la saveur de ces deux jus, peux-tu me les préparer?",
            temp.ToArray(), 25, 3);
    }
}