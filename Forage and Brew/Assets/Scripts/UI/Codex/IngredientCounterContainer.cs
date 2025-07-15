using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientCounterContainer : MonoBehaviour
{
    private int counter;
    public IngredientValuesSo trackedIngredient;
    
    public TextMeshProUGUI ingredientAmountDisplay;
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
        
    }
}
