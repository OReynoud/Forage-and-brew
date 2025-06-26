using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientPageDisplay : PageBehavior
{
    public IngredientCounterContainer ingredientCounter;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image ingredientImage;
    public Image[] biomeImages;
    public Image typeImage;
    public Image[] weatherImages;
    public Image[] cyclesImages;
    public TextMeshProUGUI[] ingredientLocations;
    public Color greyedColor;
    public WeatherStateSo[] weathers;
    public LunarCycleStateSo[] cycles;
    
    public Image disolveImage;
    public AnimationCurve dissolveCurve;

    public IngredientValuesSo associatedIngredient;

    public float dissolveTimer;

    public float animationTime;

    public bool doDissolve;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    IEnumerator StartDissolve()
    {
        yield return new WaitForSeconds(0.3f);
        Material matInstance = Instantiate(disolveImage.material);
        disolveImage.material = matInstance;
        disolveImage.material.SetFloat("_Cutoff_Height", 0);
        
        animationTime = dissolveCurve.keys[^1].time;
        doDissolve = true;
        dissolveTimer = 0;
        AutoFlip.instance.ControledBook.discoveryAudio.Play();
        
        disolveImage.sprite = AutoFlip.instance.ControledBook.bookPages.Find(x => x.pageBehavior == this).pageSprite;
    }
    // Update is called once per frame
    void Update()
    {
        if (!doDissolve) return;
        
        dissolveTimer += Time.deltaTime;
            
        disolveImage.material.SetFloat(Ex.CutoffHeight, dissolveCurve.Evaluate(dissolveTimer));

        if (dissolveTimer > animationTime)
        {
            CharacterInputManager.Instance.EnableCodexInputs();
            CharacterInputManager.Instance.EnableCodexExitInput();
            CharacterInputManager.Instance.EnableMoveInputs();
            enabled = false;
        }
    }

    private string storedText;
    public override void InitIngredient(IngredientValuesSo ingredientToDisplay)
    {
        Debug.Log("Init Ingredient");
        associatedIngredient = ingredientToDisplay;
        nameText.text = associatedIngredient.Name;
        descriptionText.text = associatedIngredient.Description;
        ingredientImage.sprite = associatedIngredient.iconHigh;
        foreach (var image in biomeImages)
        {
            image.enabled = false;
        }

        foreach (var textLocation in ingredientLocations)
        {
            textLocation.enabled = false;
        }

        int textCounter = 0;
        int maxValue = Enum.GetValues(typeof(SpawnLocation)).Cast<int>().Max();
        
        for (int mask = maxValue; mask > 0; mask >>= 1)
        {
            SpawnLocation currentSpawnLocation = (SpawnLocation)mask;
            
            if ((currentSpawnLocation & ingredientToDisplay.SpawnLocations) != 0)
            {
                string spawnLocationText = currentSpawnLocation.ToString();
                StringBuilder spawnLocationTextBuilder = new(spawnLocationText);
                MatchCollection caps = Regex.Matches(spawnLocationText, "[A-Z]");
                
                for (int i = caps.Count - 1; i >= 1; i--)
                {
                    spawnLocationTextBuilder[caps[i].Index] = (char)(caps[i].Value[0] + 32); // Convert to lowercase
                    spawnLocationTextBuilder.Insert(caps[i].Index, ' '); // Insert space before the lowercase letter
                }
                
                spawnLocationTextBuilder.Insert(0, "- ");
                
                ingredientLocations[textCounter].text = spawnLocationTextBuilder.ToString();
                ingredientLocations[textCounter].enabled = true;
                textCounter++;
            }
        }
        
        if ((associatedIngredient.Biomes & Biome.Forest) == Biome.Forest)
            biomeImages[0].enabled = true;
        if ((associatedIngredient.Biomes & Biome.Swamp) == Biome.Swamp)
            biomeImages[1].enabled = true;
        for (int i = 0; i < weatherImages.Length; i++)
        {
            weatherImages[i].color = associatedIngredient.WeatherStates.Contains(weathers[i]) ? weathers[i].Color : greyedColor;
        }
        for (int i = 0; i < cyclesImages.Length; i++)
        {
            cyclesImages[i].color = associatedIngredient.LunarCycleStates.Contains(cycles[i]) ? cycles[i].Color : greyedColor;
        }

        typeImage.sprite = associatedIngredient.Type.IconLow;
        
        ingredientCounter.trackedIngredient = ingredientToDisplay;
        ingredientCounter.UpdateDisplay(ingredientToDisplay);

        StartCoroutine(StartDissolve());

    }
}
