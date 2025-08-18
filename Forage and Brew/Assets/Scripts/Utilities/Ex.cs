using System.Collections.Generic;
using UnityEngine;

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
    
    // public static int CheckForSameElementsIngredientSo(int index, int similes, List<CookedIngredientForm> list)
    // {
    //     int currentIndex = index + similes + 1;
    //
    //     // Check if the current index is within the bounds of the list
    //     while (currentIndex < list.Count)
    //     {
    //         // Check if the current element matches the element at the starting index
    //         if (list[index].Ingredient == list[currentIndex].Ingredient &&
    //             list[index].IsAType == list[currentIndex].IsAType)
    //         {
    //             similes++;
    //             currentIndex++;
    //         }
    //         else
    //         {
    //             break;
    //         }
    //     }
    //
    //     return similes;
    // }


    public static Sprite HandleWritingIngredientType(CookedIngredientForm cookedIngredient, bool useLow)
    {
        if (cookedIngredient.IsAType)
        {
            return useLow ? cookedIngredient.IngredientType.IconLow : cookedIngredient.IngredientType.IconHigh;
        }
        
        return useLow ? cookedIngredient.Ingredient.iconLow : cookedIngredient.Ingredient.iconHigh;
        
    }
}
