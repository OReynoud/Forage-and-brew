using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    // Singleton
    public static WeatherManager Instance { get; private set; }
    
    [SerializeField] private WeatherStateSo forestStartingWeatherState;
    
    public Dictionary<Biome, (WeatherStateSo weatherState, int successiveCount)> CurrentWeatherStates { get; } = new();

    [BoxGroup("UI")] [SerializeField] private Image weatherImage;
    [BoxGroup("UI")] [SerializeField] private TextMeshProUGUI daysPassedText;
    [BoxGroup("UI")] [SerializeField] private Sprite[] weatherSprites;
    

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }
    
    private void Start()
    {
        CurrentWeatherStates.Add(Biome.Forest, (forestStartingWeatherState, 1));
        Debug.Log("The weather state for the first day is " + CurrentWeatherStates[Biome.Forest].weatherState.Name);
        DisplayWeather();
    }
    
    
    public void PassToNextWeatherState()
    {
        foreach (KeyValuePair<Biome, (WeatherStateSo weatherState, int successiveCount)> currentWeatherState in CurrentWeatherStates)
        {
            foreach (WeatherStateEndProbabilityBySuccessiveDayNumber weatherStateEndProbability in currentWeatherState.Value.weatherState.EndProbabilities)
            {
                if (weatherStateEndProbability.SuccessiveDayNumber == currentWeatherState.Value.successiveCount)
                {
                    float randomValue = Random.Range(0f, 1000f);
                    float cumulativeProbability = 0f;
                
                    foreach (WeatherStateEndProbability endProbability in weatherStateEndProbability.WeatherStateEndProbabilities)
                    {
                        cumulativeProbability += endProbability.EndProbability;
                    
                        if (randomValue <= cumulativeProbability)
                        {
                            Debug.Log("The weather state for the next day is " + endProbability.WeatherStateSo.Name);
                        
                            if (endProbability.WeatherStateSo == currentWeatherState.Value.weatherState)
                            {
                                CurrentWeatherStates[currentWeatherState.Key] = (endProbability.WeatherStateSo,
                                    CurrentWeatherStates[currentWeatherState.Key].successiveCount + 1);
                                return;
                            }
                        
                            CurrentWeatherStates[currentWeatherState.Key] = (endProbability.WeatherStateSo, 1);
                        
                            return;
                        }
                    }
                }
            }
        }

        DisplayWeather();
    }

    private void DisplayWeather()
    {
        switch (CurrentWeatherStates[Biome.Forest].weatherState.Name)
        {
            case "Sunny":
                weatherImage.sprite = weatherSprites[0];
                break;
            case "Cloudy":
                weatherImage.sprite = weatherSprites[1];
                break;
            case "Rainy":
                weatherImage.sprite = weatherSprites[2];
                break;
        }

        daysPassedText.text = "Day " + GameDontDestroyOnLoadManager.Instance.DayPassed;
    }
}
