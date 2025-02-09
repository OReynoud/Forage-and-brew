using System;
using System.Globalization;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCodexDisplay : PageBehavior
{
    [BoxGroup("Refs")] public RectTransform leftPage;
    [BoxGroup("Refs")] public RectTransform rightPage;
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
    [BoxGroup("Recipe Ingredients")] public Image[] backGround;
    [BoxGroup("Potion Description")] public Image[] potionIngredientImage;
    [BoxGroup("Potion Description")] public TextMeshProUGUI[] potionIngredientNumber;

    //BrewingSteps
    private int writingIndex;
    [BoxGroup("Brewing Steps")] public TextMeshProUGUI[] stepText;
    [BoxGroup("Brewing Steps")] public Image[] ingredientStepImage;
    [BoxGroup("Brewing Steps")] public Image[] mainActionImage;
    [BoxGroup("Brewing Steps")] public Image[] singleActionImage;

    private bool doDissolve;
    private float dissolveTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DisableAll();
    }

    public void DisableAll()
    {
        for (int i = 0; i < potionIngredientImage.Length; i++)
        {
            potionIngredientImage[i].enabled = false;
            potionIngredientNumber[i].enabled = false;
            potionIngredientImage[i].transform.parent.gameObject.SetActive(false);
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

    private void Start()
    {
    }

    public void StartDissolve()
    {
        Material mat = Instantiate(leftPageDissolve.material);
        leftPageDissolve.material.SetFloat(Ex.CutoffHeight, 0);
        leftPageDissolve.material = mat;

        Material mat2 = Instantiate(rightPageDissolve.material);
        rightPageDissolve.material.SetFloat(Ex.CutoffHeight, 0);
        rightPageDissolve.material = mat2;

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

    public override void InitRecipe(Sprite[] PotionIngredientsLow, Sprite[] PotionIngredientsHigh, PotionValuesSo PotionSteps, Sprite[] AllBrewingActionSprites)
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
            for (int i = 0; i < potionIngredientImage.Length; i++)
            {
                potionIngredientImage[i].transform.parent.gameObject.SetActive(true);
                potionIngredientImage[i].enabled = true;
                potionIngredientNumber[i].enabled = true;


                potionIngredientImage[i].sprite = potionIngredientsLow[ingredientsIndex];

                if (i + 1 < potionIngredientsLow.Length)
                {
                    int numberOfIngredients = Ex.CheckForSameElementsSprite(ingredientsIndex, 0, potionIngredientsLow);


                    potionIngredientNumber[i].text = (1 + numberOfIngredients).ToString();

                    ingredientsIndex += numberOfIngredients;
                }
                else
                {
                    potionIngredientNumber[i].text = "1";
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
                    backGround[writingIndex].sprite = ingredientTypeBackground;
                }
                else
                {
                    backGround[writingIndex].sprite = ingredientBackground;
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
}