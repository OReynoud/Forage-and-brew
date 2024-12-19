using System;
using System.Collections.Generic;
using System.Globalization;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCodexDisplay : MonoBehaviour
{
    [BoxGroup("Refs")] public RectTransform leftPage;
    [BoxGroup("Refs")] public RectTransform rightPage;
    [BoxGroup("Potion Description")] public PotionValuesSo storedPotion;
    [BoxGroup("Potion Description")] public TextMeshProUGUI potionName;
    [BoxGroup("Potion Description")] public TextMeshProUGUI potionFlavorText;
    [BoxGroup("Potion Description")] public TextMeshProUGUI potionPrice;
    [BoxGroup("Potion Description")] public GameObject[] potionDifficulty;
    [BoxGroup("Potion Description")] public Image potionIcon;
    
    
    //Ingredients List
    [BoxGroup("Potion Description")] public Image[] potionIngredientImage;
    [BoxGroup("Potion Description")] public TextMeshProUGUI[] potionIngredientNumber;
    
    //BrewingSteps
    private int writingIndex;
    [BoxGroup("Brewing Steps")] public TextMeshProUGUI[] stepText;
    [BoxGroup("Brewing Steps")] public Image[] ingredientStepImage;
    [BoxGroup("Brewing Steps")] public Image[] mainActionImage;
    [BoxGroup("Brewing Steps")] public Image[] singleActionImage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i < potionIngredientImage.Length; i++)
        {
            potionIngredientImage[i].enabled = false;
            potionIngredientNumber[i].enabled = false;
            potionIngredientImage[i].transform.parent.gameObject.SetActive(false);
        }
        for (int i = 0; i < stepText.Length; i++)
        {
            stepText[i].gameObject.SetActive(false);
            ingredientStepImage[i].gameObject.SetActive(false);
            mainActionImage[i].gameObject.SetActive(false);
            singleActionImage[i].gameObject.SetActive(false);
            stepText[i].transform.parent.gameObject.SetActive(false);
        }

        foreach (var VARIABLE in potionDifficulty)
        {
            VARIABLE.SetActive(false);
        }
    }


    private string writingText;
    public Sprite[] potionIngredients { get; set; }

    private int ingredientsIndex = 0;
    public void InitPage(Sprite[] PotionIngredients, PotionValuesSo PotionSteps)
    {
        storedPotion = PotionSteps;
        potionName.text = PotionSteps.Name;
        potionFlavorText.text = PotionSteps.Description;
        potionPrice.text = PotionSteps.SalePrice.ToString(CultureInfo.InvariantCulture);
        for (int i = 0; i < PotionSteps.Difficulty; i++)
        {
            potionDifficulty[i].SetActive(true);
        }
        potionIcon.sprite = PotionSteps.icon;
        potionIngredients = PotionIngredients;

        
        //Ingredients List
        for (int i = 0; i < potionIngredientImage.Length; i++)
        {
            potionIngredientImage[i].transform.parent.gameObject.SetActive(true);
            potionIngredientImage[i].enabled = true;
            potionIngredientNumber[i].enabled = true;

            
            potionIngredientImage[i].sprite = potionIngredients[ingredientsIndex];

            if (i + 1 < potionIngredients.Length)
            {
                int numberOfIngredients = CheckForSameElementsSprite(ingredientsIndex, 0);

                potionIngredientNumber[i].text = (1 + numberOfIngredients).ToString();

                ingredientsIndex += numberOfIngredients;
            }
            else
            {
                
                potionIngredientNumber[i].text = "1";
            }

            ingredientsIndex++;
            if (ingredientsIndex >= potionIngredients.Length - 1)
                break;
            
        }
        
        
        //Recipe Steps
        foreach (var t in PotionSteps.TemperatureChallengeIngredients)
        {
            for (var i = 0; i < t.CookedIngredients.Count; i++)
            {
                stepText[writingIndex].transform.parent.gameObject.SetActive(true);
                stepText[writingIndex].gameObject.SetActive(true);

                int numberOfIngredients = CheckForSameElementsIngredientSo(i, 0, t.CookedIngredients);
                stepText[writingIndex].text = (1 + numberOfIngredients).ToString();

                var cookedIngredient = t.CookedIngredients[i];


                ingredientStepImage[writingIndex].gameObject.SetActive(true);

                switch (cookedIngredient.CookedForm)
                {
                    case null:

                        HandleWritingIngredientType(cookedIngredient);


                        mainActionImage[writingIndex].gameObject.SetActive(true);
                        mainActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[^1];

                        break;
                    case ChoppingHapticChallengeListSo:

                        HandleWritingIngredientType(cookedIngredient);


                        mainActionImage[writingIndex].gameObject.SetActive(true);
                        mainActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[1];
                        break;
                }


                i += numberOfIngredients;
                writingIndex++;
            }

            stepText[writingIndex].transform.parent.gameObject.SetActive(true);

            singleActionImage[writingIndex].gameObject.SetActive(true);


            switch (t.Temperature)
            {
                case Temperature.None:
                    writingIndex--;
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
        singleActionImage[writingIndex].transform.parent.gameObject.SetActive(true);

        singleActionImage[writingIndex].gameObject.SetActive(true);
        singleActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[0];
        
        
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
    
}
