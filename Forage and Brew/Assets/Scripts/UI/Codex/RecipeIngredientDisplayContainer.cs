using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeIngredientDisplayContainer : MonoBehaviour
{
    public Sprite[] ingredientStateSprites;
    public IngredientValuesSo storedIngredient;
    public IngredientTypeSo storedIngredientType;

    public TextMeshProUGUI numberRequiredText;

    public Image ingredientBackground;
    public Image ingredientRequiredSprite;
    public Image ingredientStateHighlight;

    public int ingredientUpperLimit = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


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
        if (ingredientDiscovered)
        {
            ingredientRequiredSprite.enabled = true;
            ingredientRequiredSprite.sprite = storedIngredient.iconLow;
            if (ingredientAmount == 0)
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[2];
                ingredientStateHighlight.color = Color.red;
            }
            else
            {
                if (ingredientAmount > ingredientUpperLimit)
                {
                    ingredientStateHighlight.sprite = ingredientStateSprites[0];
                    ingredientStateHighlight.color = Color.blue;
                }
                else
                {
                    ingredientStateHighlight.sprite = ingredientStateSprites[1];
                    ingredientStateHighlight.color = Color.green;
                }
            }
        }
        else
        {
            ingredientRequiredSprite.enabled = false;
            ingredientStateHighlight.color = Color.black;
            ingredientStateHighlight.sprite = ingredientStateSprites[^1];
        }

        numberRequiredText.text = ingredientAmount.ToString();
    }

    public void InitializeSelf(int ingredientAmount, IngredientTypeSo ingredient, Sprite ingredientBackgroundSprite)
    {
        storedIngredientType = ingredient;
        ingredientBackground.sprite = ingredientBackgroundSprite;

        ingredientRequiredSprite.sprite = storedIngredient.iconLow;
        if (ingredientAmount == 0)
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[2];
            ingredientStateHighlight.color = Color.red;
        }
        else
        {
            if (ingredientAmount > ingredientUpperLimit)
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[0];
                ingredientStateHighlight.color = Color.blue;
            }
            else
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[1];
                ingredientStateHighlight.color = Color.green;
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
            counter++;
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients)
        {
            counter++;
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients)
        {
            counter++;
        }

        numberRequiredText.text = counter.ToString();
        
        ingredientRequiredSprite.enabled = true;
        ingredientRequiredSprite.sprite = storedIngredient.iconLow;
        if (counter == 0)
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[2];
            ingredientStateHighlight.color = Color.red;
        }
        else
        {
            if (counter > ingredientUpperLimit)
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[0];
                ingredientStateHighlight.color = Color.blue;
            }
            else
            {
                ingredientStateHighlight.sprite = ingredientStateSprites[1];
                ingredientStateHighlight.color = Color.green;
            }
        }
    }
}