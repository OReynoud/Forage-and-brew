using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CodexContentManager : Singleton<CodexContentManager>
{
    [Foldout("References")] public OrderCodexDisplayBehaviour[] orderPrefabs;
    [Foldout("References")] public RecipeCodexDisplay recipeDisplayPrefab;
    [Foldout("References")] public HistoricCodexDisplayBehavior historicDisplayPrefab;
    [Foldout("References")] public Image pinImage;
    [Foldout("References")] public Sprite leftEmptyPage;
    [Foldout("References")] public Sprite rightEmptyPage;
    [Foldout("References")] public Sprite leftRecipePage;
    [Foldout("References")] public Sprite rightRecipePage;
    [Foldout("References")] public RectTransform emptyPage;
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
    
    //Historic Management
    [BoxGroup("Recipe display")] [ReadOnly]
    public List<HistoricCodexDisplayBehavior> historicPages = new();

    [Foldout("Debug")] public PotionTag testTag;

    [Foldout("Debug")] public PotionValuesSo testPotion;

    [Foldout("Debug")] private List<Sprite> tempIngredientsList = new();


    private void Start()
    {
        pinImage.enabled = false;
        recipes.Clear();
        historicPages.Clear();
        CharacterInputManager.Instance.OnSelectRecipe.AddListener(SelectCodexPage);
        foreach (var ticket in _orderCodexDisplayBehaviours)
        {
            ticket.gameObject.SetActive(false);
        }


        for (int i = potionList.Potions.Length - 1; i >= 0; i--)
        {
            var newRecipe = Instantiate(recipeDisplayPrefab, Vector3.down * 10000, Quaternion.identity, transform);
            recipes.Insert(0, newRecipe);
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

    public void InsertRecipePages(RectTransform LeftPage, RectTransform RightPage)
    {
        AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[0].index,
            new Book.BookPage(rightRecipePage, RightPage));
        AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[0].index,
            new Book.BookPage(leftRecipePage, LeftPage));

        for (int i = 2; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
        {
            AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
        }
    }

    public void ReceiveNewOrder(ClientSo client, string orderDescription, PotionDemand[] potionsRequested,
        int moneyReward, int timeToComplete, out OrderCodexDisplayBehaviour order)
    {
        if (!emptyOrderPage)
        {
            var pageContainer = Instantiate(emptyPage, transform);

            emptyOrderPage = Instantiate(emptyPage, transform);
            order = Instantiate(orderPrefabs[Random.Range(0, orderPrefabs.Length)], pageContainer);
            pageContainer.anchoredPosition = new Vector2(1500, 0);
            emptyOrderPage.anchoredPosition = new Vector2(1500, 0);

            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,
                new Book.BookPage(rightEmptyPage, emptyOrderPage));
            emptyOrderPage.name = "Empty Order Page" + (AutoFlip.instance.ControledBook.bookMarks[0].index + 1);

            AutoFlip.instance.ControledBook.bookPages.Insert(AutoFlip.instance.ControledBook.bookMarks[1].index,
                new Book.BookPage(leftEmptyPage, pageContainer));
            pageContainer.name = "Order Page " + AutoFlip.instance.ControledBook.bookMarks[0].index;

            emptyOrderPageIndex = AutoFlip.instance.ControledBook.bookMarks[1].index + 1;
            _orderCodexDisplayBehaviours.Add(order);
            order.InitializeOrder(client, orderDescription, potionsRequested, moneyReward, timeToComplete,
                AutoFlip.instance.ControledBook.bookMarks[1].index);

            for (int i = 1; i < AutoFlip.instance.ControledBook.bookMarks.Length; i++)
            {
                AutoFlip.instance.ControledBook.bookMarks[i].index += 2;
            }
        }
        else
        {
            order = Instantiate(orderPrefabs[Random.Range(0, orderPrefabs.Length)], emptyOrderPage);
            _orderCodexDisplayBehaviours.Add(order);
            order.InitializeOrder(client, orderDescription, potionsRequested, moneyReward, timeToComplete,
                AutoFlip.instance.ControledBook.bookMarks[1].index - 1);
            emptyOrderPage = null;
        }

        AutoFlip.instance.ControledBook.UpdateSprites();
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
        }

        AutoFlip.instance.ControledBook.UpdateSprites();
    }

    private RectTransform emptyHistoricPage;

    public void AddHistoricPage(LetterContentSo originLetter, LetterContentSo successLetter)
    {
        if (!emptyHistoricPage)
        {
            var pageContainer = Instantiate(emptyPage, transform);

            emptyHistoricPage = Instantiate(emptyPage, transform);
            var historic = Instantiate(historicDisplayPrefab, pageContainer);
            pageContainer.anchoredPosition = new Vector2(1500, 0);
            emptyHistoricPage.anchoredPosition = new Vector2(1500, 0);
            
            AutoFlip.instance.ControledBook.bookPages.Add( new Book.BookPage(leftEmptyPage, pageContainer));
            pageContainer.name = "Page " + AutoFlip.instance.ControledBook.bookMarks[0].index;

            AutoFlip.instance.ControledBook.bookPages.Add(new Book.BookPage(rightEmptyPage, emptyHistoricPage));
            emptyHistoricPage.name = "Page " + (AutoFlip.instance.ControledBook.bookMarks[0].index + 1);
            
            
            historicPages.Add(historic);
            historic.InitializeHistoricPage(originLetter, successLetter);
        }
        else
        {
            var historic = Instantiate(historicDisplayPrefab, emptyHistoricPage);
            historicPages.Add(historic);
            historic.InitializeHistoricPage(originLetter, successLetter);
            emptyHistoricPage = null;
        }

        AutoFlip.instance.ControledBook.UpdateSprites();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="side"> true = right, false = left</param>

    private RecipeCodexDisplay pinnedRecipe;
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
                    Debug.Log("Selected same recipe, unpinning");
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
            PinnedRecipe.instance.PinRecipe(recipes[recipeIndex].storedPotion, recipes[recipeIndex].potionIngredients);
            Debug.Log("Pinned recipe: " + recipes[recipeIndex].storedPotion.Name);
            pinnedRecipe = recipes[recipeIndex];
            pinnedRecipe.pinIcon.enabled = true;
            pinImage.enabled = true;
        }
        else
        {
            PinnedRecipe.instance.UnpinRecipe();
            pinnedRecipe.pinIcon.enabled = false;
            pinImage.enabled = false;
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