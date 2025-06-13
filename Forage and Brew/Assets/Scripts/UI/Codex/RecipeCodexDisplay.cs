using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCodexDisplay : PageBehavior
{
    [BoxGroup("Refs")] public RectTransform leftPage;
    [BoxGroup("Refs")] public RectTransform rightPage;
    [BoxGroup("Refs")] public TextMeshProUGUI secondPageNumberText;
    [BoxGroup("Refs")] public Image pinIcon;
    [BoxGroup("Refs")] public Image leftPageDissolve;
    [BoxGroup("Refs")] public Image rightPageDissolve;
    [BoxGroup("Refs")] public AnimationCurve animCurveDissolve;
    [BoxGroup("Refs")] public Sprite ingredientBackground;
    [BoxGroup("Refs")] public Sprite ingredientTypeBackground;
    [BoxGroup("Potion Description")] public PotionValuesSo storedPotion;
    [BoxGroup("Potion Description")] public TextMeshProUGUI potionName;
    [BoxGroup("Potion Description")] public TextMeshProUGUI potionFlavorText;
    [BoxGroup("Potion Description")] public TextMeshProUGUI potionPrice;
    [BoxGroup("Potion Description")] public GameObject[] potionDifficulty;
    [BoxGroup("Potion Description")] public Image potionIcon;
    [BoxGroup("Potion Description")] public Image liquidIcon;


    //Ingredients List
    [BoxGroup("Recipe Ingredients")] public RecipeIngredientDisplayContainer[] ingredientDisplayContainers;

    //BrewingSteps
    private int writingIndex;
    [BoxGroup("Brewing Steps")] public TextMeshProUGUI[] stepText;
    [BoxGroup("Brewing Steps")] public Image[] ingredientStepImage;
    [BoxGroup("Brewing Steps")] public Image[] mainActionImage;
    [BoxGroup("Brewing Steps")] public Image[] singleActionImage;

    public int SecondPageNumber;

    private bool doDissolve;
    private float dissolveTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DisableAll();
    }

    public void DisableAll()
    {
        for (int i = 0; i < ingredientDisplayContainers.Length; i++)
        {
            ingredientDisplayContainers[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < stepText.Length; i++)
        {
            stepText[i].gameObject.SetActive(false);
            ingredientStepImage[i].gameObject.SetActive(false);
            mainActionImage[i].gameObject.SetActive(false);
            singleActionImage[i].gameObject.SetActive(false);
            stepText[i].transform.parent.gameObject.SetActive(false);
        }


        foreach (var VARIABLE in potionDifficulty)
        {
            VARIABLE.SetActive(false);
        }
    }


    public void StartDissolve()
    {
        Material mat = Instantiate(leftPageDissolve.material);
        leftPageDissolve.material.SetFloat(Ex.CutoffHeight, 0);
        leftPageDissolve.material = mat;
        leftPageDissolve.sprite =
            AutoFlip.instance.ControledBook.bookPages.Find(x => x.pageBehavior == this).pageSprite;

        Material mat2 = Instantiate(rightPageDissolve.material);
        rightPageDissolve.material.SetFloat(Ex.CutoffHeight, 0);
        rightPageDissolve.material = mat2;
        rightPageDissolve.sprite =
            AutoFlip.instance.ControledBook.bookPages.Find(x => x.pageBehavior == this).pageSprite;

        doDissolve = true;


        AutoFlip.instance.ControledBook.discoveryAudio.Play();
    }

    private void Update()
    {
        if (!doDissolve) return;

        dissolveTimer += Time.deltaTime;

        leftPageDissolve.material.SetFloat(Ex.CutoffHeight, animCurveDissolve.Evaluate(dissolveTimer));
        rightPageDissolve.material.SetFloat(Ex.CutoffHeight, animCurveDissolve.Evaluate(dissolveTimer));

        //Debug.Log(rightPageDissolve.material.GetFloat(Ex.CutoffHeight));
        if (dissolveTimer > animCurveDissolve.keys[^1].time)
        {
            CharacterInputManager.Instance.EnableCodexInputs();
            CharacterInputManager.Instance.EnableCodexExitInput();
            CharacterInputManager.Instance.EnableMoveInputs();
            doDissolve = false;
            AutoFlip.instance.isDissolving = false;
        }
    }


    private string writingText;
    public Sprite[] potionIngredientsLow { get; set; }
    public Sprite[] potionIngredientsHigh { get; set; }

    private int ingredientsIndex = 0;

    private List<IngredientValuesSo> tempIngredient = new();
    private List<IngredientTypeSo> tempIngredientType = new();

    public override void InitRecipe(Sprite[] PotionIngredientsLow, Sprite[] PotionIngredientsHigh,
        PotionValuesSo PotionSteps, Sprite[] AllBrewingActionSprites)
    {
        storedPotion = PotionSteps;
        potionName.text = PotionSteps.Name;
        potionFlavorText.text = PotionSteps.Description;
        potionPrice.text = PotionSteps.SalePrice.ToString(CultureInfo.InvariantCulture);
        for (int i = 0; i < PotionSteps.PotionDifficulty.Difficulty; i++)
        {
            potionDifficulty[i].SetActive(true);
        }

        potionIcon.sprite = PotionSteps.PotionDifficulty.PotionSprite;
        liquidIcon.sprite = PotionSteps.PotionDifficulty.LiquidSprite;
        liquidIcon.color = PotionSteps.SpriteLiquidColor;
        potionIngredientsLow = PotionIngredientsLow;
        potionIngredientsHigh = PotionIngredientsHigh;


        //Ingredients List
        if (potionIngredientsLow.Length > 0)
        {
            for (int i = 0; i < ingredientDisplayContainers.Length; i++)
            {
                ingredientDisplayContainers[i].gameObject.SetActive(true);
                int ingredientAmount;

                if (i + 1 < potionIngredientsLow.Length)
                {
                    int numberOfIngredients = Ex.CheckForSameElementsSprite(ingredientsIndex, 0, potionIngredientsLow);


                    ingredientAmount = (1 + numberOfIngredients);

                    ingredientsIndex += numberOfIngredients;
                }
                else
                {
                    ingredientAmount = 1;
                }

                CookedIngredientForm cookedIngredient = default;

                for (var index = 0; index < PotionSteps.TemperatureChallengeIngredients.Length; index++)
                {
                    TemperatureChallengeIngredients t = PotionSteps.TemperatureChallengeIngredients[index];
                    
                    foreach (var cookedForm in t.CookedIngredients)
                    {
                        if (cookedForm.Ingredient.iconLow == potionIngredientsLow[i])
                        {
                            cookedIngredient = cookedForm;
                            index = PotionSteps.TemperatureChallengeIngredients.Length;
                            break;
                        }
                    }
                }

                if (cookedIngredient.IsAType)
                {
                    ingredientDisplayContainers[i].InitializeSelf(ingredientAmount,
                        cookedIngredient.IngredientType, ingredientTypeBackground);
                }
                else
                {
                    ingredientDisplayContainers[i].InitializeSelf(ingredientAmount, cookedIngredient.Ingredient,
                        GameDontDestroyOnLoadManager.Instance.UnlockedIngredients.Contains(cookedIngredient
                            .Ingredient), ingredientBackground);
                }

                ingredientsIndex++;
                if (ingredientsIndex > potionIngredientsLow.Length - 1)
                    break;
            }
        }

        //Recipe Steps
        foreach (var t in PotionSteps.TemperatureChallengeIngredients)
        {
            for (var i = 0; i < t.CookedIngredients.Count; i++)
            {
                stepText[writingIndex].transform.parent.gameObject.SetActive(true);
                stepText[writingIndex].gameObject.SetActive(true);

                int numberOfIngredients = Ex.CheckForSameElementsIngredientSo(i, 0, t.CookedIngredients);

                stepText[writingIndex].text = (1 + numberOfIngredients).ToString();

                var cookedIngredient = t.CookedIngredients[i];


                ingredientStepImage[writingIndex].gameObject.SetActive(true);

                ingredientStepImage[writingIndex].sprite = Ex.HandleWritingIngredientType(cookedIngredient, true);

                if (cookedIngredient.IsAType)
                {
                    //backGround[writingIndex].sprite = ingredientTypeBackground;
                }
                else
                {
                    //backGround[writingIndex].sprite = ingredientBackground;
                }

                switch (cookedIngredient.CookedForm)
                {
                    case null:
                        mainActionImage[writingIndex].gameObject.SetActive(true);
                        mainActionImage[writingIndex].sprite = AllBrewingActionSprites[^1];

                        break;
                    case ChoppingHapticChallengeListSo:
                        mainActionImage[writingIndex].gameObject.SetActive(true);
                        mainActionImage[writingIndex].sprite = AllBrewingActionSprites[1];
                        break;
                    case GrindingHapticChallengeSo:
                        mainActionImage[writingIndex].gameObject.SetActive(true);
                        mainActionImage[writingIndex].sprite = AllBrewingActionSprites[2];
                        break;
                }


                i += numberOfIngredients;
                writingIndex++;
            }

            stepText[writingIndex].transform.parent.gameObject.SetActive(true);

            singleActionImage[writingIndex].gameObject.SetActive(true);


            switch (t.Temperature)
            {
                case Temperature.None:
                    writingIndex--;
                    break;
                case Temperature.LowHeat:
                    singleActionImage[writingIndex].sprite = AllBrewingActionSprites[3];
                    break;
                case Temperature.MediumHeat:
                    singleActionImage[writingIndex].sprite = AllBrewingActionSprites[4];
                    break;
                case Temperature.HighHeat:
                    singleActionImage[writingIndex].sprite = AllBrewingActionSprites[5];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            writingIndex++;
        }

        singleActionImage[writingIndex].transform.parent.gameObject.SetActive(true);

        singleActionImage[writingIndex].gameObject.SetActive(true);
        singleActionImage[writingIndex].sprite = AllBrewingActionSprites[0];
    }

    public void RemoveDissolve()
    {
        leftPageDissolve.material.SetFloat(Ex.CutoffHeight, 1);
        rightPageDissolve.material.SetFloat(Ex.CutoffHeight, 1);
    }

    public override void PlacePageNumberText()
    {
        pageNumberText.text = PageNumber.ToString();
        secondPageNumberText.text = SecondPageNumber.ToString();
        anchoredPosition = secondPageNumberText.rectTransform.anchoredPosition;
        anchoredPosition = new Vector2(Mathf.Abs(anchoredPosition.x) * -1,
            anchoredPosition.y);
        secondPageNumberText.rectTransform.anchoredPosition = anchoredPosition;
    }
}