using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinnedRecipe : Singleton<PinnedRecipe>
{
    private RectTransform ownTransform; //Behavior logic
    private bool isPinned; //Behavior logic
    private bool canShow; //Behavior logic
    private int writingIndex; // Display logic
    private Sprite[] potionIngredients; // Display logic


    [field: ReadOnly] public PotionValuesSo pinnedRecipe;
    [BoxGroup("References")] public TextMeshProUGUI title;
    [BoxGroup("References")] public CanvasGroup ingredientsCanvas;
    [BoxGroup("References")] public CanvasGroup recipeStepsCanvas;

    [BoxGroup("Behavior")] public Vector3 restingPos;
    [BoxGroup("Behavior")] public Vector3 pinnedPos;
    [BoxGroup("Behavior")] public float lerp;
    [BoxGroup("Behavior")] public Color negativeColor;
    [BoxGroup("Behavior")] public Color positiveColor;


    //Recipe ingredients
    [BoxGroup("Recipe Ingredients")] public Image[] potionIngredientsImage;
    [BoxGroup("Recipe Ingredients")] public TextMeshProUGUI[] potionIngredientQuantity;
    [BoxGroup("Recipe Ingredients")] public TextMeshProUGUI[] potionIngredientCounter;


    //Recipe steps
    [BoxGroup("Recipe Steps")] public TextMeshProUGUI[] stepText;
    [BoxGroup("Recipe Steps")] public Image[] ingredientStepImage;
    [BoxGroup("Recipe Steps")] public Image[] mainActionImage;
    [BoxGroup("Recipe Steps")] public Image[] singleActionImage;
    private List<Sprite> tempCollectedIngredientsList = new List<Sprite>();


    public void Start()
    {
        ownTransform = GetComponent<RectTransform>();
        CharacterInputManager.Instance.OnInputsEnabled.AddListener(ChangePos);
        for (int i = 0; i < potionIngredientsImage.Length; i++)
        {
            potionIngredientsImage[i].preserveAspect = true;
        }

        if (pinnedRecipe)
            PinRecipe(pinnedRecipe, potionIngredients);
    }

    private void ChangePos(bool arg0)
    {
        canShow = arg0;
    }

    private void Update()
    {
        if (CharacterInputManager.Instance.showCodex || !canShow)
        {
            ownTransform.anchoredPosition = Vector2.Lerp(
                ownTransform.anchoredPosition, restingPos, lerp);
        }
        else
        {
            ownTransform.anchoredPosition = Vector2.Lerp(
                ownTransform.anchoredPosition,
                isPinned ? pinnedPos : restingPos,
                lerp);
        }
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

        title.text = pinnedRecipe.Name;

        tempCollectedIngredientsList.Clear();
        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
        {
            tempCollectedIngredientsList.Add(ingredient.icon);
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients)
        {
            tempCollectedIngredientsList.Add(ingredient.IngredientValuesSo.icon);
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients)
        {
            tempCollectedIngredientsList.Add(ingredient.ingredient.icon);
        }

        if (GameDontDestroyOnLoadManager.Instance.PreviousScene == Scene.House)
        {
            recipeStepsCanvas.alpha = 1;
            ShowRecipeSteps();
        }
        else
        {
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


            switch (t.Temperature)
            {
                case Temperature.None:
                    break;
                case Temperature.LowHeat:
                    stepText[writingIndex].transform.parent.gameObject.SetActive(true);
                    singleActionImage[writingIndex].enabled = true;

                    singleActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[3];
                    break;
                case Temperature.MediumHeat:
                    stepText[writingIndex].transform.parent.gameObject.SetActive(true);
                    singleActionImage[writingIndex].enabled = true;

                    singleActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[4];
                    break;
                case Temperature.HighHeat:
                    stepText[writingIndex].transform.parent.gameObject.SetActive(true);
                    singleActionImage[writingIndex].enabled = true;

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

            var i1 = i;
            var temp = tempCollectedIngredientsList.Where(x => x == potionIngredients[i1]);

            var enumerable = temp.ToArray();
            Debug.Log(enumerable.Length);
            potionIngredientCounter[i].text = enumerable.Length + "";
            if (i + 1 < potionIngredients.Length)
            {
                int numberOfIngredients = Ex.CheckForSameElementsSprite(i, 0, potionIngredients);


                potionIngredientCounter[i].text = enumerable.Length + "";

                if (enumerable.Length >= 1 + numberOfIngredients)
                {
                    potionIngredientCounter[i].color = positiveColor;
                }
                else
                {
                    potionIngredientCounter[i].color = negativeColor;
                }


                potionIngredientQuantity[i].text = (1 + numberOfIngredients).ToString();

                i += numberOfIngredients;
            }


            if (enumerable.Length >= 1)
            {
                potionIngredientCounter[i].color = positiveColor;
            }
            else
            {
                potionIngredientCounter[i].color = negativeColor;
            }
        }
    }

    public void UpdateIngredientCounter()
    {
        if (!isPinned && GameDontDestroyOnLoadManager.Instance.PreviousScene != Scene.House) return;

        tempCollectedIngredientsList.Clear();
        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
        {
            tempCollectedIngredientsList.Add(ingredient.icon);
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients)
        {
            tempCollectedIngredientsList.Add(ingredient.IngredientValuesSo.icon);
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients)
        {
            tempCollectedIngredientsList.Add(ingredient.ingredient.icon);
        }

        for (int i = 0; i < potionIngredients.Length; i++)
        {
            var i1 = i;
            var temp = tempCollectedIngredientsList.Where(x => x == potionIngredients[i1]);

            var enumerable = temp.ToArray();
            potionIngredientCounter[i].text = enumerable.Length + "";
            
            if (i + 1 < potionIngredients.Length)
            {
                int numberOfIngredients = Ex.CheckForSameElementsSprite(i, 0, potionIngredients);


                if (enumerable.Length >= 1 + numberOfIngredients)
                {
                    potionIngredientCounter[i].color = positiveColor;
                }
                else
                {
                    potionIngredientCounter[i].color = negativeColor;
                }

                i += numberOfIngredients;
            }

            if (enumerable.Length >= 1)
            {
                potionIngredientCounter[i].color = positiveColor;
            }
            else
            {
                potionIngredientCounter[i].color = negativeColor;
            }
        }
    }
}