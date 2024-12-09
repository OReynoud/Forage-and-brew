using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeContainer : MonoBehaviour
{
    public TextMeshProUGUI potionName;
    public TextMeshProUGUI potionFlavorText;
    public TextMeshProUGUI potionPrice;
    public Image potionDifficulty;
    public Image potionIcon;
    public Image[] potionIngredients;
    
    //BrewingSteps
    private int writingIndex;
    public Image[] mainActionImage;
    public TextMeshProUGUI[] mainActionText;
    public Image[] ingredientStepImage;
    public TextMeshProUGUI[] secondaryActionText;
    public Image[] secondaryActionImage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i < mainActionImage.Length; i++)
        {
            mainActionImage[i].sprite = null;
            ingredientStepImage[i].sprite = null;
            secondaryActionImage[i].sprite = null;
            mainActionText[i].text = " ";
            secondaryActionText[i].text = " ";
            mainActionImage[i].enabled = false;
            ingredientStepImage[i].enabled = false;
            secondaryActionImage[i].enabled = false;
            mainActionText[i].enabled = false;
            secondaryActionText[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string writingText;
    public void InitPage(Sprite[] PotionIngredients, PotionValuesSo PotionSteps)
    {
        potionName.text = PotionSteps.Name;
        potionFlavorText.text = PotionSteps.Description;
        potionPrice.text = PotionSteps.SalePrice.ToString(CultureInfo.InvariantCulture);
        potionDifficulty.sprite = CodexContentManager.instance.allDifficultySprites[PotionSteps.Difficulty - 1];
        potionIcon.sprite = PotionSteps.icon;
        foreach (var ingredient in potionIngredients)
        {
            ingredient.transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < PotionIngredients.Length; i++)
        {
            potionIngredients[i].transform.parent.gameObject.SetActive(true);
            potionIngredients[i].sprite = PotionIngredients[i];
        }
        foreach (var t in PotionSteps.TemperatureChallengeIngredients)
        {
            foreach (var cookedIngredient in t.CookedIngredients)
            {

                switch (cookedIngredient.CookedForm)
                {
                    case null:
                        BeginWriteBrewingStep(CodexContentManager.instance.allBrewingActionSprites.Length-1,"Add ");
                        
                        ingredientStepImage[writingIndex].enabled = true;
                        HandleWritingIngredientType(cookedIngredient);
                        
                        secondaryActionText[writingIndex].enabled = true;
                        secondaryActionText[writingIndex].text = "to cauldron";
                        break;
                    case ChoppingHapticChallengeListSo:
                        BeginWriteBrewingStep(1,"Mince ");
                        
                        ingredientStepImage[writingIndex].enabled = true;
                        HandleWritingIngredientType(cookedIngredient);
                        
                        secondaryActionText[writingIndex].enabled = true;
                        secondaryActionText[writingIndex].text = "then add to cauldron";
                        
                        secondaryActionImage[writingIndex].enabled = true;
                        secondaryActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[^1];
                        break;
                }



                writingIndex++;
            }
            BeginWriteBrewingStep(0,"Stir ");
            switch (t.Temperature)
            {
                case Temperature.None:
                    break;
                case Temperature.LowHeat:
                    mainActionText[writingIndex].text += "at LOW heat";
                    break;
                case Temperature.MediumHeat:
                    mainActionText[writingIndex].text += "at MEDIUM heat";
                    break;
                case Temperature.HighHeat:
                    mainActionText[writingIndex].text += "at HIGH heat";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            writingIndex++;
        }
    }

    private void HandleWritingIngredientType(CookedIngredientForm cookedIngredient)
    {
        if (cookedIngredient.IsAType)
        {
            mainActionText[writingIndex].text += "type of ";
            ingredientStepImage[writingIndex].sprite =
                CodexContentManager.instance.allIngredientTypeSprites[
                    (int)cookedIngredient.IngredientType];
        }
        else
        {
            ingredientStepImage[writingIndex].sprite = cookedIngredient.Ingredient.icon;
        }
    }

    void BeginWriteBrewingStep(int actionSpriteIndex, string text)
    {
        mainActionImage[writingIndex].enabled = true;
        mainActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[actionSpriteIndex];
        
        mainActionText[writingIndex].enabled = true;
        mainActionText[writingIndex].text = text;
    }
    
}
