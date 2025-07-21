using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeIngredientDisplayContainer : MonoBehaviour
{
    public Sprite[] ingredientStateSprites;
    public Color[] ingredientStateColors = new[] { Color.blue, Color.green, Color.red, Color.black, };
    public IngredientValuesSo storedIngredient;
    public IngredientTypeSo storedIngredientType;

    public TextMeshProUGUI currentNumberText;
    public TextMeshProUGUI numberRequiredText;

    public Image ingredientBackground;
    public Image ingredientRequiredSprite;
    public Image ingredientStateHighlight;

    public int ingredientLimit;


    public void AddListener()
    {
        CollectHapticChallengeManager.Instance.UpdateCounters.AddListener(UpdateSelf);
    }

    // Update is called once per frame
    public void InitializeSelf(int requiredIngredientAmount, IngredientValuesSo ingredientToStore,
        Sprite ingredientBackgroundSprite)
    {
        storedIngredient = ingredientToStore;
        ingredientBackground.sprite = ingredientBackgroundSprite;
        ingredientRequiredSprite.enabled = true;
        ingredientRequiredSprite.sprite = storedIngredient.iconLow;
        ingredientLimit = requiredIngredientAmount;


        int counter = 0;

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
        {
            if (ingredient == storedIngredient)
            {
                counter++;
            }
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients)
        {
            if (ingredient.IngredientValuesSo == storedIngredient)
            {
                counter++;
            }
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients)
        {
            if (ingredient.Ingredient == storedIngredient)
            {
                counter++;
            }
        }

        if (counter > requiredIngredientAmount)
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[0];
            ingredientStateHighlight.color = ingredientStateColors[0];
            currentNumberText.color = ingredientStateColors[0];
        }
        else
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[1];
            ingredientStateHighlight.color = ingredientStateColors[1];
            currentNumberText.color = ingredientStateColors[1];
        }

        currentNumberText.text = counter.ToString();
        numberRequiredText.text = requiredIngredientAmount.ToString();
    }

    public void InitializeSelf(int requiredIngredientAmount, IngredientTypeSo ingredientToStore,
        Sprite ingredientBackgroundSprite)
    {
        storedIngredientType = ingredientToStore;
        ingredientBackground.sprite = ingredientBackgroundSprite;
        ingredientRequiredSprite.sprite = storedIngredientType.IconLow;
        ingredientLimit = requiredIngredientAmount;

        int counter = 0;

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
        {
            if (ingredient == storedIngredient)
            {
                counter++;
            }
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients)
        {
            if (ingredient.IngredientValuesSo == storedIngredient)
            {
                counter++;
            }
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients)
        {
            if (ingredient.Ingredient == storedIngredient)
            {
                counter++;
            }
        }

        if (counter >= requiredIngredientAmount)
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[0];
            ingredientStateHighlight.color = ingredientStateColors[0];
            currentNumberText.color = ingredientStateColors[0];
        }
        else
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[1];
            ingredientStateHighlight.color = ingredientStateColors[1];
            currentNumberText.color = ingredientStateColors[1];
        }


        currentNumberText.text = counter.ToString();
        numberRequiredText.text = requiredIngredientAmount.ToString();
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

        if (counter >= ingredientLimit)
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[0];
            ingredientStateHighlight.color = ingredientStateColors[0];
            currentNumberText.color = ingredientStateColors[0];
        }
        else
        {
            ingredientStateHighlight.sprite = ingredientStateSprites[1];
            ingredientStateHighlight.color = ingredientStateColors[1];
            currentNumberText.color = ingredientStateColors[1];
        }


        currentNumberText.text = counter.ToString();
    }
}