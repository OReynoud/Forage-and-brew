using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientCounterContainer : MonoBehaviour
{
    private int counter;
    public IngredientValuesSo trackedIngredient;
    public Sprite[] ingredientStateSprites;    
    
    public TextMeshProUGUI ingredientAmountDisplay;
    public Image ingredientBackground;
    public int ingredientUpperLimit = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void AddListener()
    {
        CollectHapticChallengeManager.Instance.UpdateCounters.AddListener(UpdateDisplay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDisplay(IngredientValuesSo ingredientToCheck)
    {
        if (ingredientToCheck != trackedIngredient) return;
        counter = 0;
        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
        {
            if (ingredient == ingredientToCheck)
            {
                counter++;
            }
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients)
        {
            if (ingredient.IngredientValuesSo == ingredientToCheck)
            {
                counter++;
            }
        }

        foreach (var ingredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients)
        {
            if (ingredient.Ingredient == ingredientToCheck)
            {
                counter++;
            }
        }

        ingredientAmountDisplay.text = counter.ToString();
        
        if (counter == 0)
        {
            ingredientBackground.sprite = ingredientStateSprites[2];
            ingredientBackground.color = Color.red;
        }
        else
        {
            if (counter > ingredientUpperLimit)
            {
                ingredientBackground.sprite = ingredientStateSprites[0];
                ingredientBackground.color = Color.blue;
            }
            else
            {
                ingredientBackground.sprite = ingredientStateSprites[1];
                ingredientBackground.color = Color.green;
            }
        }
    }
}
