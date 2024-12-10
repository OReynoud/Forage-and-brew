using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinnedRecipe : MonoBehaviour
{
    public PotionValuesSo pinnedRecipe;
    
    public TextMeshProUGUI title;
    public Sprite arrowSprite;
    private int writingIndex;
    
    //Recipe ingredients
    public Image[] potionIngredients;
    public TextMeshProUGUI[] potionIngredientQuantity;
    
    
    
    //Recipe steps
    public TextMeshProUGUI[] stepText;
    public Image[] ingredientStepImage;
    public Image[] mainArrow;
    public Image[] secondaryArrow;
    public Image[] mainActionImage;
    public Image[] secondaryActionImage;

    private void Start()
    {
        if (pinnedRecipe)
            PinRecipe(pinnedRecipe);
    }

    void PinRecipe(PotionValuesSo PotionToPin)
    {
        pinnedRecipe = PotionToPin;
        for (int i = 0; i < mainActionImage.Length; i++)
        {
            stepText[i].text = " ";
            ingredientStepImage[i].sprite = null;
            mainActionImage[i].sprite = null;
            secondaryActionImage[i].sprite = null;
            
            stepText[i].enabled = false;
            mainActionImage[i].enabled = false;
            ingredientStepImage[i].enabled = false;
            mainArrow[i].enabled = false;
            secondaryArrow[i].enabled = false;
            secondaryActionImage[i].enabled = false;
        }
        if (GameDontDestroyOnLoadManager.Instance.PreviousScene == Scene.House)
        {
            ShowRecipeSteps();
        }
        else
        {
            ShowRecipeIngredients();
        }
    }

    void ShowRecipeSteps()
    {
        foreach (var t in pinnedRecipe.TemperatureChallengeIngredients)
        {
            foreach (var cookedIngredient in t.CookedIngredients)
            {

                stepText[writingIndex].enabled = true;
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
                        secondaryActionImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[^1];
                        
                        break;
                }



                writingIndex++;
            }
            stepText[writingIndex].enabled = true;
            
            ingredientStepImage[writingIndex].enabled = true;
            ingredientStepImage[writingIndex].sprite = CodexContentManager.instance.allBrewingActionSprites[^1];

            
            switch (t.Temperature)
            {
                case Temperature.None:
                    break;
                case Temperature.LowHeat:            
                    mainArrow[writingIndex].enabled = true;
                    
                    mainActionImage[writingIndex].enabled = true;
                    mainActionImage[writingIndex].sprite = null;
                    mainActionImage[writingIndex].color = Color.cyan;
                    break;
                case Temperature.MediumHeat:            
                    mainArrow[writingIndex].enabled = true;    
                    
                    mainActionImage[writingIndex].enabled = true;
                    mainActionImage[writingIndex].sprite = null;
                    mainActionImage[writingIndex].color = Color.yellow;
                    break;
                case Temperature.HighHeat:            
                    mainArrow[writingIndex].enabled = true;
                                        
                    mainActionImage[writingIndex].enabled = true;
                    mainActionImage[writingIndex].sprite = null;
                    mainActionImage[writingIndex].color = Color.red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            writingIndex++;
        }
    }

    void ShowRecipeIngredients()
    {
        
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
