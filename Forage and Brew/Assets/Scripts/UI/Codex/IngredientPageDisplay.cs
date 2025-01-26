using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IngredientPageDisplay : PageBehavior
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image ingredientImage;
    public Image[] biomeImages;
    public Image typeImage;
    public Image[] weatherImages;
    public Image[] cyclesImages;
    public Color greyedColor;
    public WeatherStateSo[] weathers;
    public LunarCycleStateSo[] cycles;
    
    
    
    public Image dissolveImage;
    public AnimationCurve dissolveCurve;

    public IngredientValuesSo associatedIngredient;

    public float dissolveTimer;

    public float animationTime;

    public bool doDissolve;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void StartDissolve()
    {
        animationTime = dissolveCurve.keys[^1].time;
        doDissolve = true;
        dissolveTimer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (!doDissolve) return;
        
        dissolveTimer += Time.deltaTime;
            
        dissolveImage.material.SetFloat(Ex.CutoffHeight, dissolveCurve.Evaluate(dissolveTimer));

        if (dissolveTimer > animationTime)
        {
            CharacterInputManager.Instance.EnableCodexInputs();
            CharacterInputManager.Instance.EnableCodexExitInput();
            CharacterInputManager.Instance.EnableMoveInputs();
            enabled = false;
        }
    }

    public void InitIngredient(IngredientValuesSo ingredientToDisplay)
    {
        associatedIngredient = ingredientToDisplay;
        nameText.text = associatedIngredient.Name;
        descriptionText.text = associatedIngredient.Description;
        ingredientImage.sprite = associatedIngredient.iconHigh;
        foreach (var image in biomeImages)
        {
            image.enabled = false;
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

        typeImage.sprite = associatedIngredient.Type.IconHigh;
    }
}
