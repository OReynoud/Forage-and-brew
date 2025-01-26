using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[ExecuteAlways]
public class PageLoader : MonoBehaviour
{
    public PageBehavior display;
    [ShowIf("currentContent", ContentType.Letter)] public LetterContentSo letterToLoad;
    [ShowIf("currentContent", ContentType.Historic)] public LetterContentSo historicLetterToLoad;
    [ShowIf("currentContent", ContentType.Recipe)] public PotionValuesSo recipeToLoad;
    [ShowIf("currentContent", ContentType.Ingredient)] public IngredientValuesSo ingredientToLoad;
    [ShowIf("currentContent", ContentType.Order)] public LetterContentSo orderToLoad;
    [ReadOnly] private GameObject loadedPageBehavior;

    private List<Sprite> tempIngredientsLow = new();
    private List<Sprite> tempIngredientsHigh = new();

    public Sprite[] allBrewingActionSprites;

    [Button]
    public void LoadPage()
    {
        if (transform.GetChild(0))
            DestroyImmediate(transform.GetChild(0).gameObject);

        var toDisplay = Instantiate(display, transform.position, Quaternion.identity, transform);
        loadedPageBehavior = toDisplay.gameObject;
        switch (toDisplay)
        {
            case HistoricCodexDisplayBehavior historic:
                historic.InitHistoric(historicLetterToLoad, historicLetterToLoad.RelatedSuccessLetter);
                break;
            case LetterMailBoxDisplayBehaviour letter:
                letter.InitLetter(letterToLoad);
                break;
            case OrderCodexDisplayBehaviour order:
                order.InitOrder(orderToLoad.Client, orderToLoad.TextContent, orderToLoad.OrderContent.RequestedPotions,
                    orderToLoad.OrderContent.MoneyReward, orderToLoad.OrderContent.TimeToFulfill, 0);
                break;
            case RecipeCodexDisplay recipe:
                recipe.DisableAll();
                foreach (TemperatureChallengeIngredients t in recipeToLoad.TemperatureChallengeIngredients)
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
                recipe.InitRecipe(tempIngredientsLow.ToArray(),tempIngredientsHigh.ToArray(),recipeToLoad, allBrewingActionSprites);
                tempIngredientsLow.Clear();
                tempIngredientsHigh.Clear();
                recipe.leftPageDissolve.transform.SetParent(loadedPageBehavior.transform);
                recipe.rightPageDissolve.transform.SetParent(loadedPageBehavior.transform);
                recipe.leftPageDissolve.transform.SetAsFirstSibling();
                recipe.rightPageDissolve.transform.SetAsFirstSibling();
                break;
            case IngredientPageDisplay ingredient:
                ingredient.InitIngredient(ingredientToLoad);
                ingredient.dissolveImage.material.SetFloat(Ex.CutoffHeight, 0);
                ingredient.dissolveImage.transform.SetParent(loadedPageBehavior.transform);
                ingredient.dissolveImage.transform.SetAsFirstSibling();
                break;
        }
    }

    public enum ContentType
    {
        Letter,
        Order,
        Recipe,
        Historic,
        Ingredient
    }

    private ContentType currentContent;


    public void OnValidate()
    {
        switch (display)
        {
            case HistoricCodexDisplayBehavior:
                currentContent = ContentType.Historic;
                break;
            case IngredientPageDisplay:
                currentContent = ContentType.Ingredient;
                break;
            case LetterMailBoxDisplayBehaviour:
                currentContent = ContentType.Letter;
                break;
            case OrderCodexDisplayBehaviour:
                currentContent = ContentType.Order;
                break;
            case RecipeCodexDisplay:
                currentContent = ContentType.Recipe;
                break;
        }
    }
}