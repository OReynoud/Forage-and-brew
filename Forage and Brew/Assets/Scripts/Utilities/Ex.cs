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
    
    public static void EnsureVisibility(this ScrollRect scrollRect, RectTransform child, float padding=0)
    {
        Debug.Assert(child.parent == scrollRect.content,
            "EnsureVisibility assumes that 'child' is directly nested in the content of 'scrollRect'");

        float viewportHeight = scrollRect.viewport.rect.height;
        Vector2 scrollPosition = scrollRect.content.anchoredPosition;

        float elementTop = 0f;
        RectTransform newChild = child;

        do
        {
            elementTop -= newChild.anchoredPosition.y;
            newChild = newChild.parent as RectTransform;
        } while (newChild != scrollRect.content && newChild);
        
        float elementBottom = elementTop - child.rect.height;

        float visibleContentTop = -scrollPosition.y - padding;
        float visibleContentBottom = -scrollPosition.y - viewportHeight + padding;

        float scrollDelta =
            elementTop > visibleContentTop ? visibleContentTop - elementTop :
            elementBottom < visibleContentBottom ? visibleContentBottom - elementBottom :
            0f;

        scrollPosition.y += scrollDelta;
        scrollRect.content.anchoredPosition = scrollPosition;
    }
}
