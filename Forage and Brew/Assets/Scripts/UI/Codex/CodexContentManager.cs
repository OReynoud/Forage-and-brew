using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CodexContentManager : Singleton<CodexContentManager>
{
    [Foldout("References")] public OrderCodexDisplayBehaviour[] orderPrefabs;
    [Foldout("References")] public RecipeCodexDisplay recipeDisplayPrefab;
    [Foldout("References")] public IngredientPageDisplay ingredientDisplayPrefabLeft;
    [Foldout("References")] public IngredientPageDisplay ingredientDisplayPrefabRight;
    [Foldout("References")] public HistoricCodexDisplayBehavior historicDisplayPrefab;
    [Foldout("References")] public BundlePageCodexDisplay bundlesDisplayPrefab;
    [Foldout("References")] public Image pinImage;
    [Space] [Foldout("References")] public Sprite[] leftEmptyPage;
    [Foldout("References")] public Sprite[] rightEmptyPage;
    [Space] [Foldout("References")] public Sprite[] leftRecipePage;
    [Foldout("References")] public Sprite[] rightRecipePage;
    [Space] [Foldout("References")] public Sprite[] leftIngredientPage;
    [Foldout("References")] public Sprite[] rightIngredientPage;
    [Space] [Foldout("References")] public RectTransform emptyPage;
    [Foldout("References")] public Sprite[] allBrewingActionSprites;

    //Orders Management
    private readonly List<OrderCodexDisplayBehaviour> _orderCodexDisplayBehaviours = new();
    private RectTransform emptyOrderPage;
    private int emptyOrderPageIndex;

    //Recipe Management
    [BoxGroup("Recipe display")] [SerializeField]
    private PotionListSo potionList;

    [BoxGroup("Recipe display")] [ReadOnly]
    public List<RecipeCodexDisplay> recipes = new();

    //Ingredients Management
    [BoxGroup("Ingredient display")] [SerializeField]
    public IngredientListSo ingredientList;

    [BoxGroup("Ingredient display")] [ReadOnly]
    public List<IngredientPageDisplay> ingredientPages = new();

    [BoxGroup("Ingredient display")] public float ingredientDissolveDelay = 0.4f;

    //Historic Management
    [BoxGroup("Historic display")] [ReadOnly]
    public List<HistoricCodexDisplayBehavior> historicPages = new();

    [BoxGroup("Bundles display")] [ReadOnly]
    public List<BundlePageCodexDisplay> bundlesPages = new();


    [Foldout("Debug")] private List<Sprite> tempIngredientsLow = new();
    [Foldout("Debug")] private List<Sprite> tempIngredientsHigh = new();

    public List<int> pageIndexesToCheck = new();
    private int pageChoser;

    public bool isDiscoveringNewIngredient { get; set; }

    private void Start()
    {
        StartCoroutine(StartingRoutine());
    }

    IEnumerator StartingRoutine()
    {
        pinImage.enabled = false;
        recipes.Clear();
        historicPages.Clear();
        CharacterInputManager.Instance.OnSelectRecipe.AddListener(SelectCodexPage);
        GameDontDestroyOnLoadManager.Instance.OnNewRecipeReceived.AddListener(CreateNewRecipePage);
        foreach (var ticket in _orderCodexDisplayBehaviours)
        {
            ticket.gameObject.SetActive(false);
        }

        yield return new WaitWhile(() => !OrderManager.Instance.IsInitialized);
        if (GameDontDestroyOnLoadManager.Instance.loadOrders)
        {
            foreach (var orderSo in GameDontDestroyOnLoadManager.Instance.OrdersToLoad)
            {
                OrderManager.Instance.CreateNewOrder(new Letter(orderSo, null));
            }
        }

        if (GameDontDestroyOnLoadManager.Instance.loadAllRecipes)
        {
            GameDontDestroyOnLoadManager.Instance.UnlockedRecipes.AddRange(potionList.Potions);
        }

        foreach (var recipes in GameDontDestroyOnLoadManager.Instance.UnlockedRecipes)
        {
            CreateNewRecipePage(recipes);
        }

        foreach (var display in recipes)
        {
            display.RemoveDissolve();
        }

        pageIndexesToCheck.Clear();


        if (GameDontDestroyOnLoadManager.Instance.loadAllIngredients)
        {
            GameDontDestroyOnLoadManager.Instance.UnlockedIngredients.Clear();
            GameDontDestroyOnLoadManager.Instance.UnlockedIngredients.AddRange(ingredientList.IngredientValues);
            AutoFlip.instance.ControledBook.DisplayNewIngredientFromSave();
        }

        if (GameDontDestroyOnLoadManager.Instance.loadHistoric)
        {
            GameDontDestroyOnLoadManager.Instance.UnlockedRecipes.Clear();
            GameDontDestroyOnLoadManager.Instance.UnlockedRecipes.AddRange(potionList.Potions);
            foreach (var historic in GameDontDestroyOnLoadManager.Instance.HistoricToLoad)
            {
                AddHistoricPage(historic, historic.RelatedSuccessLetter);
            }
        }

        AutoFlip.instance.ControledBook.UpdatePageNumbers();
    }

    private void CreateNewRecipePage(PotionValuesSo newRecipeValues)
    {
        var newRecipe = Instantiate(recipeDisplayPrefab, Vector3.down * 10000, Quaternion.identity, transform);
        int recipeIndex = potionList.Potions.IndexOf(newRecipeValues);
        if (recipes.Count == 0)
        {
            recipeIndex = 0;
        }
        else
        {
            for (var index = 0; index < recipes.Count; index++)
            {
                var recipe = recipes[index];
                if (potionList.Potions.IndexOf(recipe.storedPotion) > recipeIndex)
                {
                    recipeIndex = index;
                    break;
                }
            }

            if (recipeIndex > recipes.Count)
            {
                recipeIndex = recipes.Count;
            }
        }

        recipes.Insert(recipeIndex, newRecipe);
        recipeIndex *= 2;
        recipeIndex += AutoFlip.instance.ControledBook.bookMarks[1].index;


        pageChoser = Random.Range(0, rightRecipePage.Length);
        foreach (TemperatureChallengeIngredients t in newRecipeValues.TemperatureChallengeIngredients)
        {
            foreach (CookedIngredientForm cookedIngredient in t.CookedIngredients)
            {
                if (cookedIngredient.IsAType)
                {
                    tempIngredientsLow.Add(cookedIngredient.IngredientType.IconLow);
                    tempIngredientsHigh.Add(cookedIngredient.IngredientType.IconHigh);
                }
                else
                {
                    tempIngredientsLow.Add(cookedIngredient.Ingredient.iconLow);
                    tempIngredientsHigh.Add(cookedIngredient.Ingredient.iconHigh);
                }
            }
        }

        newRecipe.InitRecipe(tempIngredientsLow.ToArray(), tempIngredientsHigh.ToArray(), newRecipeValues,
            allBrewingActionSprites,
            new[] { leftRecipePage[pageChoser], rightRecipePage[pageChoser] });
        tempIngredientsLow.Clear();
        tempIngredientsHigh.Clear();
        //Debug.Log(recipeIndex);
        InsertRecipePages(recipeIndex, newRecipe);
    }

    void InsertRecipePages(int index, RecipeCodexDisplay recipeDisplay)
    {
        AutoFlip.instance.ControledBook.bookPages.Insert(index,
            new Book.BookPage(rightRecipePage[pageChoser], recipeDisplay.rightPage, recipeDisplay));
        AutoFlip.instance.ControledBook.bookPages.Insert(index,
            new Book.BookPage(leftRecipePage[pageChoser], recipeDisplay.leftPage, recipeDisplay));

        // for (var i = 0; i < pageIndexesToCheck.Count; i++)
        // {
        //     pageIndexesToCheck[i] = (pageIndexesToCheck[i].Item1 + 2, pageIndexesToCheck[i].Item2);
        // }

        //pageIndexesToCheck.Add();

        for (int i = 2; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
        {
            AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
        }

        if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[2].index)
        {
            AutoFlip.instance.ControledBook.currentPage += 2;
        }

        AutoFlip.instance.ControledBook.UpdatePageNumbers();
    }

    public void ReceiveNewOrder(ClientSo client, string orderDescription, PotionDemand[] potionsRequested,
        int moneyReward, out OrderCodexDisplayBehaviour order)
    {
        pageChoser = Random.Range(0, rightEmptyPage.Length);

        if (!emptyOrderPage)
        {
            //Debug.Log(AutoFlip.instance.ControledBook.bookMarks[1].index);
            var pageContainer = Instantiate(emptyPage, transform);

            emptyOrderPage = Instantiate(emptyPage, transform);
            emptyOrderPage.gameObject.SetActive(false);
            order = Instantiate(orderPrefabs[Random.Range(0, orderPrefabs.Length)], pageContainer);
            pageContainer.anchoredPosition = new Vector2(1500, 0);
            emptyOrderPage.anchoredPosition = new Vector2(1500, 0);

            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,
                new Book.BookPage(rightEmptyPage[pageChoser], emptyOrderPage,
                    emptyOrderPage.GetComponent<PageBehavior>()));
            emptyOrderPage.name = "Empty Order Page" + (AutoFlip.instance.ControledBook.bookMarks[1].index + 1);

            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,
                new Book.BookPage(leftEmptyPage[pageChoser], pageContainer, order));
            pageContainer.name = "Order Page " + AutoFlip.instance.ControledBook.bookMarks[1].index;

            emptyOrderPageIndex = AutoFlip.instance.ControledBook.bookMarks[1].index + 1;
            _orderCodexDisplayBehaviours.Add(order);
            order.InitOrder(client, orderDescription, potionsRequested, moneyReward,
                AutoFlip.instance.ControledBook.bookMarks[1].index);

            pageIndexesToCheck.Add(AutoFlip.instance.ControledBook.bookMarks[1].index);

            for (int i = 1; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
            {
                AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
            }

            if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[1].index)
            {
                AutoFlip.instance.ControledBook.currentPage += 2;
            }
        }
        else
        {
            order = Instantiate(orderPrefabs[Random.Range(0, orderPrefabs.Length)], emptyOrderPage);
            _orderCodexDisplayBehaviours.Add(order);
            Book.BookPage bookPage =
                AutoFlip.instance.ControledBook.bookPages.Find(x => x.UIComponent == emptyOrderPage);
            int index = AutoFlip.instance.ControledBook.bookPages.IndexOf(bookPage);
            AutoFlip.instance.ControledBook.bookPages.RemoveAt(index);
            bookPage.pageBehavior = order;
            AutoFlip.instance.ControledBook.bookPages.Insert(index, bookPage);


            order.InitOrder(client, orderDescription, potionsRequested, moneyReward,
                AutoFlip.instance.ControledBook.bookMarks[1].index - 1);
            emptyOrderPage = null;
        }

        AutoFlip.instance.ControledBook.UpdateSprites();
        AutoFlip.instance.ControledBook.UpdatePageNumbers();
    }

    public void TerminateOrder(int index)
    {
        index += AutoFlip.instance.ControledBook.bookMarks[0].index;
        if (!emptyOrderPage)
        {
            emptyOrderPage = AutoFlip.instance.ControledBook.bookPages[emptyOrderPageIndex].UIComponent;
            emptyOrderPageIndex = index;
            Destroy(emptyOrderPage.GetChild(0).gameObject);
            emptyOrderPage.name = "Empty Order Page " + emptyOrderPageIndex;
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


            for (int i = 1; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
            {
                AutoFlip.instance.ControledBook.bookMarks[i].index -= 2;
            }

            if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[1].index)
            {
                AutoFlip.instance.ControledBook.currentPage -= 2;
            }
        }

        AutoFlip.instance.ControledBook.UpdateSprites();
        AutoFlip.instance.ControledBook.UpdatePageNumbers();
    }

    private RectTransform emptyHistoricPage;

    public void AddHistoricPage(LetterContentSo originLetter, LetterContentSo successLetter)
    {
        pageChoser = Random.Range(0, rightEmptyPage.Length);

        if (!emptyHistoricPage)
        {
            var pageContainer = Instantiate(emptyPage, transform);

            emptyHistoricPage = Instantiate(emptyPage, transform);
            var historic = Instantiate(historicDisplayPrefab, pageContainer);
            var dummy = historic;
            pageContainer.anchoredPosition = new Vector2(1500, 0);
            emptyHistoricPage.anchoredPosition = new Vector2(1500, 0);

            AutoFlip.instance.ControledBook.bookPages.Add(new Book.BookPage(rightEmptyPage[pageChoser], pageContainer,
                historic));
            pageContainer.name = "Page " + AutoFlip.instance.ControledBook.bookMarks[3].index;

            AutoFlip.instance.ControledBook.bookPages.Add(new Book.BookPage(leftEmptyPage[pageChoser],
                emptyHistoricPage, dummy));
            emptyHistoricPage.name = "Page " + (AutoFlip.instance.ControledBook.bookMarks[3].index + 1);

            if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[3].index)
            {
                AutoFlip.instance.ControledBook.currentPage += 2;
            }

            historicPages.Add(historic);
            if (successLetter)
            {
                historic.InitHistoric(originLetter, successLetter);
            }
            else
            {
                historic.InitHistoric(originLetter);
            }
        }
        else
        {
            var historic = Instantiate(historicDisplayPrefab, emptyHistoricPage);
            var bookPage = AutoFlip.instance.ControledBook.bookPages.Find(x => x.UIComponent == emptyHistoricPage);
            bookPage.pageBehavior = historic;
            historicPages.Add(historic);
            if (successLetter)
            {
                historic.InitHistoric(originLetter, successLetter);
            }
            else
            {
                historic.InitHistoric(originLetter);
            }

            emptyHistoricPage = null;
        }

        AutoFlip.instance.ControledBook.UpdateSprites();
        AutoFlip.instance.ControledBook.UpdatePageNumbers();
    }

    private RectTransform emptyIngredientPage;

    public int AddIngredientPage(IngredientValuesSo ingredient)
    {
        pageChoser = Random.Range(0, rightIngredientPage.Length);

        int ingredientIndex = ingredientList.IngredientValues.IndexOf(ingredient);
        //Debug.Log("Raw index: " + ingredientIndex);
        if (ingredientPages.Count == 0)
        {
            //Debug.Log("First discovered ingredient");
            ingredientIndex = AutoFlip.instance.ControledBook.bookMarks[2].index;
        }
        else if (ingredientIndex >= ingredientList.IngredientValues.IndexOf(ingredientPages[^1].associatedIngredient))
        {
            //Debug.Log("Highest index yet");
            ingredientIndex = AutoFlip.instance.ControledBook.bookMarks[2].index + ingredientPages.Count;
        }
        else
        {
            if (ingredientPages.Count == 1)
            {
                ingredientIndex = AutoFlip.instance.ControledBook.bookMarks[2].index;
            }
            else
            {
                for (int i = 0; i < ingredientPages.Count; i++)
                {
                    if (ingredientIndex <
                        ingredientList.IngredientValues.IndexOf(ingredientPages[i].associatedIngredient))
                    {
                        ingredientIndex = AutoFlip.instance.ControledBook.bookMarks[2].index + i;
                        break;
                    }
                }
            }
            //Debug.Log("Index can be fitted in book: " + ingredientIndex);
        }


        if (!emptyIngredientPage)
        {
            var pageContainer = Instantiate(emptyPage, transform);

            emptyIngredientPage = Instantiate(emptyPage, transform);
            var ingredientPage = Instantiate(ingredientDisplayPrefabLeft, pageContainer);
            pageContainer.anchoredPosition = new Vector2(1450, 0);
            emptyIngredientPage.anchoredPosition = new Vector2(1450, 0);

            AutoFlip.instance.ControledBook.bookPages.Insert(
                AutoFlip.instance.ControledBook.bookMarks[2].index + ingredientPages.Count,
                new Book.BookPage(rightIngredientPage[pageChoser], emptyIngredientPage,
                    emptyIngredientPage.GetComponent<PageBehavior>()));


            AutoFlip.instance.ControledBook.bookPages.Insert(ingredientIndex,
                new Book.BookPage(leftIngredientPage[pageChoser], pageContainer, ingredientPage));
            pageContainer.name = ingredient.Name;


            ingredientPages.Add(ingredientPage);
            ingredientPage.InitIngredient(ingredient);
            emptyIngredientPage.name = "Empty ingredient page " +
                                       (AutoFlip.instance.ControledBook.bookMarks[2].index + ingredientPages.Count);
            for (int i = 3; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
            {
                AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
            }

            if (AutoFlip.instance.ControledBook.currentPage >= ingredientIndex + 2)
            {
                AutoFlip.instance.ControledBook.currentPage += 2;
            }
        }
        else
        {
            var ingredientPage = Instantiate(ingredientDisplayPrefabRight, emptyIngredientPage);
            Book.BookPage bookPage =
                AutoFlip.instance.ControledBook.bookPages.Find(x => x.UIComponent == emptyIngredientPage);

            AutoFlip.instance.ControledBook.bookPages.Remove(bookPage);

            bookPage.pageBehavior = ingredientPage;

            AutoFlip.instance.ControledBook.bookPages.Insert(ingredientIndex, bookPage);
            ingredientPages.Add(ingredientPage);
            ingredientPage.InitIngredient(ingredient);
            emptyIngredientPage.name = ingredient.Name;
            emptyIngredientPage = null;
        }

        //Debug.Log("Placed " + ingredient.Name + " at index " + ingredientIndex);
        AutoFlip.instance.ControledBook.UpdatePageNumbers();
        return ingredientIndex;
    }

    private RectTransform emptyBundlesPage;

    private void AddBundlesPage(PotionEnsembleSo bundleToDisplay)
    {
        pageChoser = Random.Range(0, rightEmptyPage.Length);

        Debug.Log("New bundle pages");
        var pageContainer = Instantiate(emptyPage, transform);

        emptyBundlesPage = Instantiate(emptyPage, transform);
        var bundles1 = Instantiate(bundlesDisplayPrefab, pageContainer);
        var bundles2 = Instantiate(bundlesDisplayPrefab, emptyBundlesPage);
        pageContainer.anchoredPosition = new Vector2(1500, 0);
        emptyBundlesPage.anchoredPosition = new Vector2(1500, 0);
        
        AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[0].index,new Book.BookPage(rightEmptyPage[pageChoser], emptyBundlesPage,
            bundles2));
        emptyBundlesPage.name = "Bundle, Page " + (AutoFlip.instance.ControledBook.bookMarks[0].index + 1);

        AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[0].index,new Book.BookPage(leftEmptyPage[pageChoser], pageContainer,
            bundles1));
        pageContainer.name = "Bundle, Page " + AutoFlip.instance.ControledBook.bookMarks[0].index;
        
        if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[0].index)
        {
            AutoFlip.instance.ControledBook.currentPage += 2;
        }
        for (int i = 0; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
        {
            AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
        }

        bundlesPages.Add(bundles1);
        bundlesPages.Add(bundles2);


        AutoFlip.instance.ControledBook.UpdateSprites();
        AutoFlip.instance.ControledBook.UpdatePageNumbers();
        AddNewBundleToCodex(bundleToDisplay);
    }

    public void AddNewBundleToCodex(PotionEnsembleSo bundleToDisplay)
    {
        if (bundlesPages.Count == 0)
        {
            AddBundlesPage(bundleToDisplay);
            return;
        }

        foreach (var bundlesPage in bundlesPages)
        {
            foreach (var bundle in bundlesPage.allContainers)
            {
                if (bundle.isInitialized)continue;
                bundlesPage.InitNewBundle(bundleToDisplay);
                return;
            }
        }
        
        AddBundlesPage(bundleToDisplay);
    }

    public void UpdateCodexBundle(PotionEnsembleSo bundleToUpdate, int potionIndexToUpdate)
    {
        foreach (var bundlesPage in bundlesPages)
        {
            foreach (var bundle in bundlesPage.allContainers)
            {
                if (!bundle.isInitialized)return;
                if (bundle.displayedBundle == bundleToUpdate)
                {
                    bundle.UpdateCheckmarks(potionIndexToUpdate);
                    return;
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="side"> true = right, false = left</param>

    public RecipeCodexDisplay pinnedRecipe { get; set; }

    public void SelectCodexPage(bool side)
    {
        if (!CharacterInputManager.Instance.showCodex) return;

        if (AutoFlip.instance.ControledBook.currentPage >= AutoFlip.instance.ControledBook.bookMarks[1].index &&
            AutoFlip.instance.ControledBook.currentPage < AutoFlip.instance.ControledBook.bookMarks[2].index)
        {
            int recipeIndex = Mathf.FloorToInt((AutoFlip.instance.ControledBook.currentPage -
                                                AutoFlip.instance.ControledBook.bookMarks[1].index) * 0.5f);
            if (PinnedRecipe.instance.pinnedRecipe)
            {
                if (PinnedRecipe.instance.pinnedRecipe.Name == recipes[recipeIndex].storedPotion.Name)
                {
                    //Debug.Log("Selected same recipe, unpinning");
                    PinnedRecipe.instance.UnpinRecipe();
                    pinnedRecipe.pinIcon.enabled = false;
                    pinImage.enabled = false;
                    return;
                }
            }

            if (pinnedRecipe)
            {
                pinnedRecipe.pinIcon.enabled = false;
            }

            PinnedRecipe.instance.PinRecipe(recipes[recipeIndex].storedPotion,
                recipes[recipeIndex].potionIngredientsHigh);
            //Debug.Log("Pinned recipe: " + recipes[recipeIndex].storedPotion.Name);
            pinnedRecipe = recipes[recipeIndex];
            pinnedRecipe.pinIcon.enabled = true;
            pinImage.enabled = true;
        }
        else
        {
            if (!pinnedRecipe)
                return;
            //Debug.Log("Selected nothing, unpinning");
            pinnedRecipe.pinIcon.enabled = false;
            pinImage.enabled = false;
            PinnedRecipe.instance.UnpinRecipe();
        }
    }


    // public void DebugTickets()
    // {
    //     var temp = new List<PotionDemand>();
    //     temp.Add(new PotionDemand(true, testPotion));
    //
    //     ReceiveNewOrder("Jean-Eude", "Je me suis coupé le doigt, tu peux me passer de la pommade s'il te plait?",
    //         temp.ToArray(), 10, 3);
    //     temp.Clear();
    //
    //     temp.Add(new PotionDemand(false, testTag, "Something against a fever"));
    //     ReceiveNewOrder("Paul", "J'ai de la fièvre, t'as quelque chose pour m'aider?", temp.ToArray(), 15, 3);
    //     temp.Clear();
    //
    //     temp.Add(new PotionDemand(true, testPotion));
    //     temp.Add(new PotionDemand(true, testPotion));
    //     ReceiveNewOrder("Marie", "J'ai besoin de comparer la saveur de ces deux jus, peux-tu me les préparer?",
    //         temp.ToArray(), 25, 3);
    // }
}