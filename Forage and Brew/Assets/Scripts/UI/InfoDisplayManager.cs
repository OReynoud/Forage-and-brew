using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplayManager : Singleton<InfoDisplayManager>
{
    [BoxGroup("Top Right")][SerializeField] private RectTransform weatherUIContainer;
    [BoxGroup("Top Right")][SerializeField] private TextMeshProUGUI daysPassedText;
    [BoxGroup("Top Right")][SerializeField] private GameObject forestDisplay;
    [BoxGroup("Top Right")][SerializeField] private Image forestWeather;
    [BoxGroup("Top Right")][SerializeField] private GameObject swampDisplay;
    [BoxGroup("Top Right")][SerializeField] private Image swampWeather;
    [BoxGroup("Top Right")][SerializeField] private Image moonCyclesDisplay;
    
    [BoxGroup("Top Left")][SerializeField] private RectTransform letterIcon;
    
    [BoxGroup("Bottom Right")][SerializeField] private RectTransform moneyUIContainer;
    [BoxGroup("Bottom Right")][SerializeField] private TextMeshProUGUI moneyText;
    
    [BoxGroup("Bottom Left")][SerializeField] private RectTransform codexIcon;

    [BoxGroup("Behavior")] [SerializeField] private float lerp;
    [BoxGroup("Behavior")] [SerializeField][ReadOnly] private bool canShow;
    
    [BoxGroup("Behavior")] [SerializeField] private Vector2 topRightShownPos;
    [BoxGroup("Behavior")] [SerializeField] private Vector2 topRightHiddenPos;
    [BoxGroup("Behavior")] [SerializeField] private Vector2 topLeftShownPos;
    [BoxGroup("Behavior")] [SerializeField] private Vector2 topLeftHiddenPos;
    [BoxGroup("Behavior")] [SerializeField] private Vector2 bottomRightShownPos;
    [BoxGroup("Behavior")] [SerializeField] private Vector2 bottomRightHiddenPos;
    [BoxGroup("Behavior")] [SerializeField] private Vector2 bottomLeftShownPos;
    [BoxGroup("Behavior")] [SerializeField] private Vector2 bottomLeftHiddenPos;

    
    public void DisplayAll()
    {
        DisplayWeather();
        DisplayDays();
        DisplayMoney();
        DisplayMoonCycles();
    }

    private void Start()
    {
        CharacterInputManager.Instance.OnInputsEnabled.AddListener(UpdateUIVisibility);
        CharacterInputManager.Instance.OnNavigationChange.AddListener(UpdateUIVisibilityReverse);
    }

    private void UpdateUIVisibility(bool arg0)
    {
        canShow = arg0;
    }
    private void UpdateUIVisibilityReverse(bool arg0)
    {
        canShow = !arg0;
    }

    private void Update()
    {
        if (canShow)
        {
            weatherUIContainer.anchoredPosition = Vector2.Lerp(weatherUIContainer.anchoredPosition, topRightShownPos, lerp);
            moneyUIContainer.anchoredPosition = Vector2.Lerp(moneyUIContainer.anchoredPosition, bottomRightShownPos, lerp);
            if (!CharacterInputManager.Instance.showCodex)
            {
                letterIcon.anchoredPosition = Vector2.Lerp(letterIcon.anchoredPosition, topLeftShownPos, lerp);
                codexIcon.anchoredPosition = Vector2.Lerp(codexIcon.anchoredPosition, bottomLeftShownPos, lerp);
            }
            else
            {
                letterIcon.anchoredPosition = Vector2.Lerp(letterIcon.anchoredPosition,topLeftHiddenPos, lerp);
                codexIcon.anchoredPosition = Vector2.Lerp( codexIcon.anchoredPosition,bottomLeftHiddenPos, lerp);
            }
        }
        else
        {
            weatherUIContainer.anchoredPosition = Vector2.Lerp( weatherUIContainer.anchoredPosition, topRightHiddenPos, lerp);
            moneyUIContainer.anchoredPosition = Vector2.Lerp( moneyUIContainer.anchoredPosition, bottomRightHiddenPos, lerp);
            letterIcon.anchoredPosition = Vector2.Lerp(letterIcon.anchoredPosition,topLeftHiddenPos, lerp);
            codexIcon.anchoredPosition = Vector2.Lerp( codexIcon.anchoredPosition,bottomLeftHiddenPos, lerp);
        }
    }


    public void DisplayWeather()
    {
        forestDisplay.SetActive(false);
        swampDisplay.SetActive(false);

        Biome currentBiome = Biome.None;
        
        switch (GameDontDestroyOnLoadManager.Instance.PreviousScene)
        {
            case Scene.House:
            case Scene.Outdoor:
                break;
            case Scene.Biome1:
                currentBiome = Biome.Forest;
                break;
            case Scene.Biome2:
                currentBiome = Biome.Swamp;
                break;
        }

        switch (currentBiome)
        {
            case Biome.None:
                forestDisplay.SetActive(true);
                DisplayBiomeWeather(Biome.Forest, forestWeather);

                swampDisplay.SetActive(true);
                DisplayBiomeWeather(Biome.Swamp, swampWeather);
                break;

            case Biome.Forest:
                forestDisplay.SetActive(true);
                DisplayBiomeWeather(Biome.Forest, forestWeather);
                break;

            case Biome.Swamp:
                swampDisplay.SetActive(true);
                DisplayBiomeWeather(Biome.Swamp, swampWeather);
                break;
        }
    }

    private void DisplayBiomeWeather(Biome biome, Image weatherDisplay)
    {
        weatherDisplay.sprite = WeatherManager.Instance.CurrentWeatherStates[biome].WeatherStateSo.Icon;
        weatherDisplay.color = WeatherManager.Instance.CurrentWeatherStates[biome].WeatherStateSo.Color;
    }
    
    public void DisplayDays()
    {
        daysPassedText.text = "Day " + (GameDontDestroyOnLoadManager.Instance.DayPassed + 1);
    }
    
    public void DisplayMoney()
    {
        moneyText.text = MoneyManager.Instance.MoneyAmount.ToString();
    }
    
    public void DisplayMoonCycles()
    {
        moonCyclesDisplay.sprite = LunarCycleManager.Instance.CurrentLunarCycleState.Icon;
        moonCyclesDisplay.color = LunarCycleManager.Instance.CurrentLunarCycleState.Color;
    }

}
