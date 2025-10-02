using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Ex
{
    public static readonly int CutoffHeight = Shader.PropertyToID("_Cutoff_Height");
    public static readonly string MusicVolume = "musicVolume";
    public static readonly string SfxVolume = "sfxVolume";
    public static int CheckForSameElementsSprite(int index, int similes, Sprite[] potionIngredients)
    {
        if (index + similes + 1 >= potionIngredients.Length)
            return similes;


        if (potionIngredients[index] !=
            potionIngredients[index + similes + 1])
            return similes;

        //Debug.Log("Similar element detected");
        similes++;
        return CheckForSameElementsSprite(index, similes, potionIngredients);
    }

    public static int CheckForSameElementsIngredientSo(int index, int similes, List<CookedIngredientForm> list)
    {
        if (index + similes + 1 < list.Count)
        {
            if (list[index].Ingredient ==
                list[index + similes + 1].Ingredient
                &&
                list[index].IsAType == 
                list[index + similes + 1].IsAType)
            {
                similes++;
                return CheckForSameElementsIngredientSo(index, similes, list);
            }

            return similes;
        }
        return similes;
    }


    public static Sprite HandleWritingIngredientType(CookedIngredientForm cookedIngredient, bool useLow)
    {
        if (cookedIngredient.IsAType)
        {
            return useLow ? cookedIngredient.IngredientType.IconLow : cookedIngredient.IngredientType.IconHigh;
        }
        
        return useLow ? cookedIngredient.Ingredient.iconLow : cookedIngredient.Ingredient.iconHigh;
        
    }

    public static void EnsureVisibilityVertical(this ScrollRect scrollRect, RectTransform child, float padding)
    {
        float viewportHeight = scrollRect.viewport.rect.height;
        if (-child.anchoredPosition.y - padding < scrollRect.content.anchoredPosition.y)
        {
            // Element is above the visible area, scroll up
            float scrollPositionY = -child.anchoredPosition.y - padding;
            scrollPositionY = Mathf.Max(scrollPositionY, 0f);
            Vector2 scrollPosition = new(scrollRect.content.anchoredPosition.x, scrollPositionY);
            scrollRect.content.anchoredPosition = scrollPosition;
        }
        else if (-child.anchoredPosition.y + child.rect.height + padding >
                 scrollRect.content.anchoredPosition.y + viewportHeight)
        {
            // Element is below the visible area, scroll down
            float scrollPositionY = -child.anchoredPosition.y + child.rect.height + padding - viewportHeight;
            scrollPositionY = Mathf.Min(scrollPositionY, scrollRect.content.rect.height - viewportHeight);
            Vector2 scrollPosition = new(scrollRect.content.anchoredPosition.x, scrollPositionY);
            scrollRect.content.anchoredPosition = scrollPosition;
        }
    }
}
