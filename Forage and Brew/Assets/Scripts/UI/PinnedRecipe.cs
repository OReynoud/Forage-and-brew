using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinnedRecipe : Singleton<PinnedRecipe>
{
    public PotionValuesSo pinnedRecipe;

    private RectTransform ownTransform;
    public TextMeshProUGUI title;
    private bool isPinned;
    public Vector3 restingPos;
    public Vector3 pinnedPos;
    public float lerp;
    
    public Sprite arrowSprite;
    public Sprite[] potionIngredients;
    private int writingIndex;

    //Recipe ingredients
    public Image[] potionIngredientsImage;
    public TextMeshProUGUI[] potionIngredientQuantity;


    //Recipe steps
    public TextMeshProUGUI[] stepText;
    public Image[] ingredientStepImage;
    public Image[] mainActionImage;
    public Image[] singleActionImage;

    public CanvasGroup ingredientsList;
    public CanvasGroup recipeStepsList;

    public void Start()
    {
        ownTransform = GetComponent<RectTransform>();
        for (int i = 0; i < potionIngredientsImage.Length; i++)
        {
            potionIngredientsImage[i].preserveAspect = true;
        }
        if (pinnedRecipe)
            PinRecipe(pinnedRecipe, potionIngredients);
    }

    private void Update()
    {
        if (CharacterInputManager.Instance.showCodex) 
            return;
        
        ownTransform.anchoredPosition = Vector2.Lerp(
            ownTransform.anchoredPosition, 
            isPinned ? pinnedPos : restingPos, 
            lerp);
    }

    public void UnpinRecipe()
    {
        isPinned = false;
        pinnedRecipe = null;
    }
    public void PinRecipe(PotionValuesSo PotionToPin, Sprite[] PotionIngredients)
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
            singleActionImage[i].sprite = null;
            
            stepText[i].enabled = false;
            ingredientStepImage[i].enabled = false;
            mainActionImage[i].enabled = false;
            singleActionImage[i].enabled = false;
        }


        if (GameDontDestroyOnLoadManager.Instance.PreviousScene == Scene.House)
        {
            title.text = pinnedRecipe.Name;
            recipeStepsList.alpha = 1;
            ShowRecipeSteps();
        }
        else
        {
            title.text = pinnedRecipe.Name;
            ingredientsList.alpha = 1;
            ShowRecipeIngredients();
        }

        isPinned = true;
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
                stepText[writingIndex].transform.parent.gameObject.SetActive(true);
                stepText[writingIndex].enabled = true;

                int numberOfIngredients = CheckForSameElementsIngredientSo(i, 0, t.CookedIngredients);
                stepText[writingIndex].text = (1 + numberOfIngredients).ToString();

                var cookedIngredient = t.CookedIngredients[i];


                ingredientStepImage[writingIndex].enabled = true;

                switch (cookedIngredient.CookedForm)
                {
                    case null:

                        HandleWritingIngredientType(cookedIngredient);


                        mainActionImage[writingIndex].enabled = true;
                        mainActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[^1];

                        break;
                    case ChoppingHapticChallengeListSo:

                        HandleWritingIngredientType(cookedIngredient);


                        mainActionImage[writingIndex].enabled = true;
                        mainActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[1];
                        break;
                }


                i += numberOfIngredients;
                writingIndex++;
            }

            stepText[writingIndex].transform.parent.gameObject.SetActive(true);

            singleActionImage[writingIndex].enabled = true;


            switch (t.Temperature)
            {
                case Temperature.None:
                    break;
                case Temperature.LowHeat:
                    singleActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[3];
                    break;
                case Temperature.MediumHeat:
                    singleActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[4];
                    break;
                case Temperature.HighHeat:
                    singleActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[5];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            writingIndex++;
        }
        stepText[writingIndex].transform.parent.gameObject.SetActive(true);

        singleActionImage[writingIndex].enabled = true;
        singleActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[0];
    }

    void ShowRecipeIngredients()
    {
        foreach (var ingredient in potionIngredientsImage)
        {
            ingredient.transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < potionIngredients.Length; i++)
        {
            potionIngredientsImage[i].transform.parent.gameObject.SetActive(true);
            potionIngredientsImage[i].sprite = potionIngredients[i];

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
                return CheckForSameElementsIngredientSo(index, similes, list);
            }

            return similes;
        }
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