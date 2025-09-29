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
    
    public static void EnsureVisibilityVerticalGridLayout(this ScrollRect scrollRect, GridLayoutGroup gridLayout, RectTransform child)
    {
        gridLayout.GetColumnAndRowCount(out int _, out int rowCount);
        float scrollPositionY = gridLayout.padding.vertical + gridLayout.spacing.y * rowCount + gridLayout.cellSize.y * rowCount;
        Vector2 scrollPosition = new Vector2(scrollRect.content.anchoredPosition.x, scrollPositionY);

        float elementTop = 0f;
        RectTransform newChild = child;

        do
        {
            elementTop -= newChild.anchoredPosition.y;
            newChild = newChild.parent as RectTransform;
        } while (newChild != scrollRect.content && newChild);
        
        float elementBottom = elementTop - child.rect.height;

        // float visibleContentTop = -scrollPosition.y - padding;
        // float visibleContentBottom = -scrollPosition.y - viewportHeight + padding;

        // float scrollDelta =
        //     elementTop > visibleContentTop ? visibleContentTop - elementTop :
        //     elementBottom < visibleContentBottom ? visibleContentBottom - elementBottom :
        //     0f;

        // scrollPosition.y += scrollDelta;
        scrollRect.content.anchoredPosition = scrollPosition;
    }
    
    public static void GetColumnAndRowCount(this GridLayoutGroup glg, out int column, out int row)
    {
        column = 0;
        row = 0;

        if (glg.transform.childCount == 0)
            return;

        //Column and row are now 1
        column = 1;
        row = 1;

        //Get the first child GameObject of the GridLayoutGroup
        RectTransform firstChildObj = glg.transform.
            GetChild(0).GetComponent<RectTransform>();

        Vector2 firstChildPos = firstChildObj.anchoredPosition;
        bool stopCountingRow = false;

        //Loop through the rest of the child object
        for (int i = 1; i < glg.transform.childCount; i++)
        {
            //Get the next child
            RectTransform currentChildObj = glg.transform.
                GetChild(i).GetComponent<RectTransform>();

            Vector2 currentChildPos = currentChildObj.anchoredPosition;

            //if first child.x == otherchild.x, it is a column, ele it's a row
            if (firstChildPos.x == currentChildPos.x)
            {
                column++;
                //Stop couting row once we find column
                stopCountingRow = true;
            }
            else
            {
                if (!stopCountingRow)
                    row++;
            }
        }
    }
}
