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
    public Image[] potionSteps;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitPage(string PotionName, string PotionFlavorText, float PotionPrice, Sprite PotionDifficulty, Sprite PotionIcon, Sprite[] PotionIngredients)
    {
        potionName.text = PotionName;
        potionFlavorText.text = PotionFlavorText;
        potionPrice.text = PotionPrice.ToString(CultureInfo.InvariantCulture);
        potionDifficulty.sprite = PotionDifficulty;
        potionIcon.sprite = PotionIcon;
        foreach (var ingredient in potionIngredients)
        {
            ingredient.transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < PotionIngredients.Length; i++)
        {
            potionIngredients[i].transform.parent.gameObject.SetActive(true);
            potionIngredients[i].sprite = PotionIngredients[i];
        }
    }
}
