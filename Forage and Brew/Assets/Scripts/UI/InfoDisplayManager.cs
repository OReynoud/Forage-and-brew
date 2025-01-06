using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplayManager : Singleton<InfoDisplayManager>
{
    [SerializeField] private TextMeshProUGUI daysPassedText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject forestDisplay;
    [SerializeField] private Image forestWeather;
    [SerializeField] private GameObject swampDisplay;
    [SerializeField] private Image swampWeather;
    [SerializeField] private Image moonCyclesDisplay;
    
    [SerializeField] private Sprite[] weatherSprites;
    [SerializeField] private Sprite[] moonCycleSprites;

    
    public void DisplayAll()
    {
        DisplayWeather();
        DisplayDays();
        DisplayMoney();
        DisplayMoonCycles();
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
        weatherDisplay.sprite = WeatherManager.Instance.CurrentWeatherStates[biome].weatherState.Icon;
    }
    
    public void DisplayDays()
    {
        daysPassedText.text = "Day " + GameDontDestroyOnLoadManager.Instance.DayPassed;
    }
    
    public void DisplayMoney()
    {
        moneyText.text = MoneyManager.Instance.MoneyAmount.ToString();
    }
    
    public void DisplayMoonCycles()
    {
        moonCyclesDisplay.sprite = LunarCycleManager.Instance.CurrentLunarCycleState.Icon;
    }

}
