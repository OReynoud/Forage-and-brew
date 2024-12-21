using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

    private Biome currentBiome;

    public void DisplayAll()
    {
        Debug.Log("DisplayAll");
        DisplayWeather();
        DisplayDays();
        DisplayMoney();
        DisplayMoonCycles();
    }



    private void DisplayWeather()
    {
        forestDisplay.SetActive(false);
        swampDisplay.SetActive(false);

        currentBiome = Biome.None;
        switch (GameDontDestroyOnLoadManager.Instance.PreviousScene)
        {
            case Scene.House:
                break;
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

    void DisplayBiomeWeather(Biome biome, Image weatherDisplay)
    {
        switch (WeatherManager.Instance.CurrentWeatherStates[biome].weatherState.Name)
        {
            case "Sunny":
                weatherDisplay.sprite = weatherSprites[0];
                break;
            case "Cloudy":
                weatherDisplay.sprite = weatherSprites[1];
                break;
            case "Rainy":
                weatherDisplay.sprite = weatherSprites[2];
                break;
        }
    }
    private void DisplayDays()
    {
        daysPassedText.text = "Day " + GameDontDestroyOnLoadManager.Instance.DayPassed;
    }
    private void DisplayMoney()
    {
        moneyText.text = GameDontDestroyOnLoadManager.Instance.MoneyAmount.ToString();
    }

    private int index;
    private Biome biome;
    private void DisplayMoonCycles()
    {
        moonCyclesDisplay.sprite = moonCycleSprites[LunarCycleManager.Instance.CurrentLunarCycleState.Index];
    }

}
