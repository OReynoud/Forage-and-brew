using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinnedRecipe : MonoBehaviour
{
    public PotionValuesSo pinnedRecipe;

    public TextMeshProUGUI title;
    public Sprite arrowSprite;
    public Sprite[] potionIngredients;
    private int writingIndex;

    //Recipe ingredients
    public Image[] potionIngredientsImage;
    public TextMeshProUGUI[] potionIngredientQuantity;


    //Recipe steps
    public TextMeshProUGUI[] stepText;
    public Image[] ingredientStepImage;
    public Image[] mainArrow;
    public Image[] secondaryArrow;
    public Image[] mainActionImage;
    public Image[] secondaryActionImage;

    public CanvasGroup ingredientsList;
    public CanvasGroup recipeStepsList;

    private void Start()
    {
        PinRecipe(pinnedRecipe, potionIngredients);
        //if (pinnedRecipe)
    }

    void PinRecipe(PotionValuesSo PotionToPin, Sprite[] PotionIngredients)
    {
        ingredientsList.alpha = 0;
        recipeStepsList.alpha = 0;

        writingIndex = 0;
        pinnedRecipe = PotionToPin;
        potionIngredients = PotionIngredients;
        for (int i = 0; i < mainActionImage.Length; i++)
        {
            stepText[i].text = " ";
            ingredientStepImage[i].sprite = null;
            mainActionImage[i].sprite = null;
            secondaryActionImage[i].sprite = null;

            stepText[i].enabled = false;
            ingredientStepImage[i].enabled = false;
            mainActionImage[i].enabled = false;
            secondaryActionImage[i].enabled = false;

            mainArrow[i].enabled = false;
            secondaryArrow[i].enabled = false;
        }

        recipeStepsList.alpha = 1;
        ShowRecipeSteps();

        // if (GameDontDestroyOnLoadManager.Instance.PreviousScene == Scene.House)
        // {
        // }
        // else
        // {
        //     ingredientsList.alpha = 1;
        //     ShowRecipeIngredients();
        // }
    }

    void ShowRecipeSteps()
    {
        foreach (var t in stepText)
        {
            t.transform.parent.gameObject.SetActive(false);
        }
        
        foreach (var t in pinnedRecipe.TemperatureChallengeIngredients)
        {
            for (var i = 0; i < t.CookedIngredients.Count; i++)
            {
                
                stepText[i].transform.parent.gameObject.SetActive(true);
                stepText[i].enabled = true;
                
                int numberOfIngredients = CheckForSameElementsIngredientSo(i, 0, t.CookedIngredients);
                
                stepText[i].text = (1 + numberOfIngredients).ToString();
                
                var cookedIngredient = t.CookedIngredients[i];
                
                
                ingredientStepImage[i].enabled = true;
                mainArrow[i].enabled = true;

                switch (cookedIngredient.CookedForm)
                {
                    case null:
                
                        HandleWritingIngredientType(cookedIngredient);
                
                
                        mainActionImage[i].enabled = true;
                        mainActionImage[i].sprite = CodexContentManager.instance.allBrewingActionSprites[^1];
                
                        break;
                    case ChoppingHapticChallengeListSo:
                
                        HandleWritingIngredientType(cookedIngredient);
                
                
                        mainActionImage[i].enabled = true;
                        mainActionImage[i].sprite = CodexContentManager.instance.allBrewingActionSprites[1];
                
                        secondaryArrow[i].enabled = true;
                
                        secondaryActionImage[i].enabled = true;
                        secondaryActionImage[i].sprite =
                            CodexContentManager.instance.allBrewingActionSprites[^1];
                
                        break;
                }


                i += numberOfIngredients;
                Debug.Log(i);
            }
        //
        //     stepText[writingIndex].transform.parent.gameObject.SetActive(true);
        //
        //     ingredientStepImage[writingIndex].enabled = true;
        //     ingredientStepImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[^1];
        //
        //
        //     switch (t.Temperature)
        //     {
        //         case Temperature.None:
        //             break;
        //         case Temperature.LowHeat:
        //             mainArrow[writingIndex].enabled = true;
        //
        //             mainActionImage[writingIndex].enabled = true;
        //             mainActionImage[writingIndex].sprite = null;
        //             mainActionImage[writingIndex].color = Color.cyan;
        //             break;
        //         case Temperature.MediumHeat:
        //             mainArrow[writingIndex].enabled = true;
        //
        //             mainActionImage[writingIndex].enabled = true;
        //             mainActionImage[writingIndex].sprite = null;
        //             mainActionImage[writingIndex].color = Color.yellow;
        //             break;
        //         case Temperature.HighHeat:
        //             mainArrow[writingIndex].enabled = true;
        //
        //             mainActionImage[writingIndex].enabled = true;
        //             mainActionImage[writingIndex].sprite = null;
        //             mainActionImage[writingIndex].color = Color.red;
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        //
        //     writingIndex++;
        }
    }

    void ShowRecipeIngredients()
    {
        foreach (var ingredient in potionIngredientsImage)
        {
            ingredient.transform.gameObject.SetActive(false);
        }

        foreach (var text in potionIngredientQuantity)
        {
            text.transform.gameObject.SetActive(false);
        }

        for (int i = 0; i < potionIngredients.Length; i++)
        {
            potionIngredientsImage[i].transform.gameObject.SetActive(true);
            potionIngredientsImage[i].sprite = potionIngredients[i];

            potionIngredientQuantity[i].transform.gameObject.SetActive(true);
            if (i + 1 < potionIngredients.Length)
            {
                int numberOfIngredients = CheckForSameElementsSprite(i, 0);

                potionIngredientQuantity[i].text = (1 + numberOfIngredients).ToString();

                i += numberOfIngredients;
            }
        }
    }

    int CheckForSameElementsSprite(int index, int similes)
    {
        if (index + similes + 1 >= potionIngredients.Length)
            return similes;


        if (potionIngredients[index] !=
            potionIngredients[index + similes + 1])
            return similes;

        //Debug.Log("Similar element detected");
        similes++;
        return CheckForSameElementsSprite(index, similes);
    }

    int CheckForSameElementsIngredientSo(int index, int similes, List<CookedIngredientForm> list)
    {
        if (index + similes + 1 < list.Count)
        {
            if (list[index].Ingredient ==
                list[index + similes + 1].Ingredient)
            {
                similes++;
                //Debug.Log("Similar element detected");
                return CheckForSameElementsSprite(index, similes);
            }

            //Debug.Log("Different element detected" );
            return similes;
        }


        //Debug.Log("Reached end of list" );
        return similes;
    }


    private void HandleWritingIngredientType(CookedIngredientForm cookedIngredient)
    {
        if (cookedIngredient.IsAType)
        {
            ingredientStepImage[writingIndex].sprite =
                CodexContentManager.instance.allIngredientTypeSprites[
                    (int)cookedIngredient.IngredientType];
        }
        else
        {
            ingredientStepImage[writingIndex].sprite = cookedIngredient.Ingredient.icon;
        }
    }
}