using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class PageBehavior : MonoBehaviour
{
    public virtual void InitOrder(ClientSo client,string description, PotionDemand[] Potions, int Reward, int TTC, int index) { }
    public virtual void InitLetter(LetterContentSo newLetterContent) {}
    public virtual void InitIngredient(IngredientValuesSo ingredientToDisplay) {}
    public virtual void InitHistoric(LetterContentSo originLetter, LetterContentSo successLetter) {}
    public virtual void InitRecipe(Sprite[] PotionIngredientsLow, Sprite[] PotionIngredientsHigh, PotionValuesSo PotionSteps, Sprite[] AllBrewingActionSprites) {}
}
