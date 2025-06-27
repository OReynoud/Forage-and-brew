using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeIngredientDisplayContainer : MonoBehaviour
{
    public Sprite[] ingredientStateSprites;
    public Color[] ingredientStateColors = new []{Color.blue, Color.green, Color.red, Color.black, };
    public IngredientValuesSo storedIngredient;
    public IngredientTypeSo storedIngredientType;

    public TextMeshProUGUI numberRequiredText;

    public Image ingredientBackground;
    public Image ingredientRequiredSprite;
    public Image ingredientStateHighlight;

    public int ingredientUpperLimit = 2;
    

    public void AddListener()
    {
        CollectHapticChallengeManager.Instance.UpdateCounters.AddListener(UpdateSelf);
    }

    // Update is called once per frame
    public void InitializeSelf(int ingredientAmount, IngredientValuesSo ingredient, bool ingredientDiscovered,
        Sprite ingredientBackgroundSprite)
    {
        storedIngredient = ingredient;
        ingredientBackground.sprite = ingredientBackgroundSprite;
        ingredientRequiredSprite.enabled = true;
        ingredientRequiredSprite.sprite = storedIngredient.iconLow;
        if (ingredientAmount == 0)
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[2];
            ingredientStateHighlight.color = ingredientStateColors[2];
        }
        else
        {
            if (ingredientAmount > ingredientUpperLimit)
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[0];
                ingredientStateHighlight.color = ingredientStateColors[0];
            }
            else
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[1];
                ingredientStateHighlight.color = ingredientStateColors[1];
            }
        }

        numberRequiredText.text = ingredientAmount.ToString();
    }

    public void InitializeSelf(int ingredientAmount, IngredientTypeSo ingredient, Sprite ingredientBackgroundSprite)
    {
        storedIngredientType = ingredient;
        ingredientBackground.sprite = ingredientBackgroundSprite;
        ingredientRequiredSprite.sprite = storedIngredientType.IconLow;
        
        if (ingredientAmount == 0)
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[2];
            ingredientStateHighlight.color = ingredientStateColors[2];
        }
        else
        {
            if (ingredientAmount > ingredientUpperLimit)
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[0];
                ingredientStateHighlight.color = ingredientStateColors[0];
            }
            else
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[1];
                ingredientStateHighlight.color = ingredientStateColors[1];
            }
        }

        numberRequiredText.text = ingredientAmount.ToString();
    }

    void UpdateSelf(IngredientValuesSo ingredientToUpdate)
    {
        if (ingredientToUpdate != storedIngredient) return;
        int counter = 0;

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
        {
            if (ingredient == ingredientToUpdate)
            {
                counter++;
            }
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients)
        {
            if (ingredient.IngredientValuesSo == ingredientToUpdate)
            {
                counter++;
            }
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients)
        {
            if (ingredient.Ingredient == ingredientToUpdate)
            {
                counter++;
            }
        }
        numberRequiredText.text = counter.ToString();
        
        ingredientRequiredSprite.enabled = true;
        ingredientRequiredSprite.sprite = storedIngredient.iconLow;
        if (counter == 0)
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[2];
            ingredientStateHighlight.color = ingredientStateColors[2];
        }
        else
        {
            if (counter > ingredientUpperLimit)
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[0];
                ingredientStateHighlight.color = ingredientStateColors[0];
            }
            else
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[1];
                ingredientStateHighlight.color = ingredientStateColors[1];
            }
        }
    }
}