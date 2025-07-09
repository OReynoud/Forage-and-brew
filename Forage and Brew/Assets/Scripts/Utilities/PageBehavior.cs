using TMPro;
using UnityEngine;

public class PageBehavior : MonoBehaviour
{
    public int PageNumber;
    public TextMeshProUGUI pageNumberText;
    public virtual void InitOrder(ClientSo client,string description, PotionDemand[] Potions, int Reward, int index) { }
    public virtual void InitLetter(LetterContentSo newLetterContent) {}
    public virtual void InitIngredient(IngredientValuesSo ingredientToDisplay) {}
    public virtual void InitHistoric(LetterContentSo originLetter, LetterContentSo successLetter) {}
    public virtual void InitRecipe(Sprite[] PotionIngredientsLow, Sprite[] PotionIngredientsHigh, PotionValuesSo PotionSteps, Sprite[] AllBrewingActionSprites, Sprite[] PagesToUse) {}
    public virtual void InitBundlesPage(PotionEnsembleSo bundleToDisplay) {}

    protected Vector2 anchoredPosition;
    public virtual void PlacePageNumberText()
    {
        pageNumberText.text = PageNumber.ToString();
        anchoredPosition = pageNumberText.rectTransform.anchoredPosition;
        if (PageNumber % 2 == 1)
        {
            anchoredPosition = new Vector2(Mathf.Abs(anchoredPosition.x),
                anchoredPosition.y);
        }
        else
        {
            anchoredPosition = new Vector2(Mathf.Abs(anchoredPosition.x) * -1,
                anchoredPosition.y);
        }
        pageNumberText.rectTransform.anchoredPosition = anchoredPosition;
    }
}
