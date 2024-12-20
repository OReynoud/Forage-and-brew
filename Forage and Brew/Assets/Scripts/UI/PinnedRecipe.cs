using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinnedRecipe : Singleton<PinnedRecipe>
{
    private RectTransform ownTransform; //Behavior logic
    private bool isPinned; //Behavior logic
    private int writingIndex; // Display logic
    private Sprite[] potionIngredients; // Display logic


    [field: ReadOnly] public PotionValuesSo pinnedRecipe;
    [BoxGroup("References")] public TextMeshProUGUI title;
    [BoxGroup("References")] public CanvasGroup ingredientsCanvas;
    [BoxGroup("References")] public CanvasGroup recipeStepsCanvas;

    [BoxGroup("Behavior")] public Vector3 restingPos;
    [BoxGroup("Behavior")] public Vector3 pinnedPos;
    [BoxGroup("Behavior")] public float lerp;


    //Recipe ingredients
    [BoxGroup("Recipe Ingredients")] public Image[] potionIngredientsImage;
    [BoxGroup("Recipe Ingredients")] public TextMeshProUGUI[] potionIngredientQuantity;


    //Recipe steps
    [BoxGroup("Recipe Steps")] public TextMeshProUGUI[] stepText;
    [BoxGroup("Recipe Steps")] public Image[] ingredientStepImage;
    [BoxGroup("Recipe Steps")] public Image[] mainActionImage;
    [BoxGroup("Recipe Steps")] public Image[] singleActionImage;


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
        ingredientsCanvas.alpha = 0;
        recipeStepsCanvas.alpha = 0;

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
            recipeStepsCanvas.alpha = 1;
            ShowRecipeSteps();
        }
        else
        {
            title.text = pinnedRecipe.Name;
            ingredientsCanvas.alpha = 1;
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

                int numberOfIngredients = Ex.CheckForSameElementsIngredientSo(i, 0, t.CookedIngredients);
                stepText[writingIndex].text = (1 + numberOfIngredients).ToString();

                var cookedIngredient = t.CookedIngredients[i];


                ingredientStepImage[writingIndex].enabled = true;
                ingredientStepImage[writingIndex].sprite = Ex.HandleWritingIngredientType(cookedIngredient);

                switch (cookedIngredient.CookedForm)
                {
                    case null:
                        mainActionImage[writingIndex].enabled = true;
                        mainActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[^1];

                        break;
                    case ChoppingHapticChallengeListSo:
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
                int numberOfIngredients = Ex.CheckForSameElementsSprite(i, 0, potionIngredients);

                potionIngredientQuantity[i].text = (1 + numberOfIngredients).ToString();

                i += numberOfIngredients;
            }
        }
    }
}