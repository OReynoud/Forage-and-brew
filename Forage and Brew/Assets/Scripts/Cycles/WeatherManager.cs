using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    // Singleton
    public static WeatherManager Instance { get; private set; }
    
    [SerializeField] private WeatherStateSo forestStartingWeatherState;
    [SerializeField] private WeatherStateSo swampStartingWeatherState;
    
    public Dictionary<Biome, (WeatherStateSo weatherState, int successiveCount)> CurrentWeatherStates { get; } = new();

    

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
        CurrentWeatherStates.Add(Biome.Swamp, (swampStartingWeatherState, 1));
        Debug.Log("The weather state for the first day is " + CurrentWeatherStates[Biome.Forest].weatherState.Name);
        InfoDisplayManager.instance.DisplayWeather();
    }
    
    
    public void PassToNextWeatherState()
    {
        foreach (KeyValuePair<Biome, (WeatherStateSo weatherState, int successiveCount)> currentWeatherState in CurrentWeatherStates.ToList())
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
                                break;
                            }
                        
                            CurrentWeatherStates[currentWeatherState.Key] = (endProbability.WeatherStateSo, 1);
                        
                            break;
                        }
                    }
                }
            }
        }
        
        InfoDisplayManager.instance.DisplayWeather();
    }


}
