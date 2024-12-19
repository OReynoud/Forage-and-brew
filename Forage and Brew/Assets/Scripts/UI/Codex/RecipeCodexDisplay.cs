using System;
using System.Collections.Generic;
using System.Globalization;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCodexDisplay : MonoBehaviour
{
    [BoxGroup("Potion Description")] public PotionValuesSo storedPotion;
    [BoxGroup("Potion Description")] public TextMeshProUGUI potionName;
    [BoxGroup("Potion Description")] public TextMeshProUGUI potionFlavorText;
    [BoxGroup("Potion Description")] public TextMeshProUGUI potionPrice;
    [BoxGroup("Potion Description")] public Image potionDifficulty;
    [BoxGroup("Potion Description")] public Image potionIcon;
    [BoxGroup("Potion Description")] public Image[] potionIngredientsImage;
    
    //BrewingSteps
    private int writingIndex;
    [BoxGroup("Brewing Steps")] public TextMeshProUGUI[] stepText;
    [BoxGroup("Brewing Steps")] public Image[] ingredientStepImage;
    [BoxGroup("Brewing Steps")] public Image[] mainActionImage;
    [BoxGroup("Brewing Steps")] public Image[] secondaryActionImage;
    [BoxGroup("Brewing Steps")] public Image[] mainArrow;
    [BoxGroup("Brewing Steps")] public Image[] secondaryArrow;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string writingText;
    public Sprite[] potionIngredients;

    public void InitPage(Sprite[] PotionIngredients, PotionValuesSo PotionSteps)
    {
        storedPotion = PotionSteps;
        potionName.text = PotionSteps.Name;
        potionFlavorText.text = PotionSteps.Description;
        potionPrice.text = PotionSteps.SalePrice.ToString(CultureInfo.InvariantCulture);
        potionDifficulty.sprite = CodexContentManager.instance.allDifficultySprites[PotionSteps.Difficulty - 1];
        potionIcon.sprite = PotionSteps.icon;
        potionIngredients = PotionIngredients;
        
        foreach (var ingredient in potionIngredientsImage)
        {
            ingredient.transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < PotionIngredients.Length; i++)
        {
            potionIngredientsImage[i].transform.parent.gameObject.SetActive(true);
            potionIngredientsImage[i].sprite = PotionIngredients[i];
        }

        foreach (var t in stepText)
        {
            t.transform.parent.gameObject.SetActive(false);
        }
        
        foreach (var t in PotionSteps.TemperatureChallengeIngredients)
        {
            for (var i = 0; i < t.CookedIngredients.Count; i++)
            {
                stepText[writingIndex].transform.parent.gameObject.SetActive(true);
                stepText[writingIndex].enabled = true;

                int numberOfIngredients = CheckForSameElementsIngredientSo(i, 0, t.CookedIngredients);
                stepText[writingIndex].text = (1 + numberOfIngredients).ToString();

                var cookedIngredient = t.CookedIngredients[i];


                ingredientStepImage[writingIndex].enabled = true;
                mainArrow[writingIndex].enabled = true;

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

                        secondaryArrow[writingIndex].enabled = true;

                        secondaryActionImage[writingIndex].enabled = true;
                        secondaryActionImage[writingIndex].sprite =
                            CodexContentManager.instance.allBrewingActionSprites[^1];

                        break;
                }


                i += numberOfIngredients;
                writingIndex++;
                //writingIndex += numberOfIngredients;
            }

            stepText[writingIndex].transform.parent.gameObject.SetActive(true);

            ingredientStepImage[writingIndex].enabled = true;


            switch (t.Temperature)
            {
                case Temperature.None:
                    break;
                case Temperature.LowHeat:
                    ingredientStepImage[writingIndex].sprite = null;
                    ingredientStepImage[writingIndex].color = Color.cyan;
                    break;
                case Temperature.MediumHeat:
                    ingredientStepImage[writingIndex].sprite = null;
                    ingredientStepImage[writingIndex].color = Color.yellow;
                    break;
                case Temperature.HighHeat:
                    ingredientStepImage[writingIndex].sprite = null;
                    ingredientStepImage[writingIndex].color = Color.red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            writingIndex++;
        }
        stepText[writingIndex].transform.parent.gameObject.SetActive(true);

        ingredientStepImage[writingIndex].enabled = true;
        ingredientStepImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[0];
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
    
}
