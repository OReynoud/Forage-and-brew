using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    // Singleton
    public static WeatherManager Instance { get; private set; }
    
    [SerializeField] private WeatherStateSo forestStartingWeatherState;
    [SerializeField] private WeatherStateSo swampStartingWeatherState;
    
    public Dictionary<Biome, WeatherSuccessiveDays> CurrentWeatherStates { get; } = new();

    

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
        if (GameDontDestroyOnLoadManager.Instance.IsFirstGameSession)
        {
            CurrentWeatherStates.Add(Biome.Forest, new WeatherSuccessiveDays(forestStartingWeatherState, 1));
            CurrentWeatherStates.Add(Biome.Swamp, new WeatherSuccessiveDays(swampStartingWeatherState, 1));
        }
        Debug.Log("The weather state for the first day is " + CurrentWeatherStates[Biome.Forest].WeatherStateSo.Name +
                  " in the forest and " + CurrentWeatherStates[Biome.Swamp].WeatherStateSo.Name + " in the swamp.");
        InfoDisplayManager.instance.DisplayWeather();
    }
    
    
    public void PassToNextWeatherState()
    {
        foreach (KeyValuePair<Biome, WeatherSuccessiveDays> currentWeatherState in CurrentWeatherStates.ToList())
        {
            foreach (WeatherStateEndProbabilityBySuccessiveDayNumber weatherStateEndProbability in currentWeatherState.Value.WeatherStateSo.EndProbabilities)
            {
                if (weatherStateEndProbability.SuccessiveDayNumber == currentWeatherState.Value.SuccessiveDays)
                {
                    float randomValue = Random.Range(0f, 1000f);
                    float cumulativeProbability = 0f;
                
                    foreach (WeatherStateEndProbability endProbability in weatherStateEndProbability.WeatherStateEndProbabilities)
                    {
                        cumulativeProbability += endProbability.EndProbability;
                    
                        if (randomValue <= cumulativeProbability)
                        {
                            Debug.Log("The weather state for the next day is " + endProbability.WeatherStateSo.Name);
                        
                            if (endProbability.WeatherStateSo == currentWeatherState.Value.WeatherStateSo)
                            {
                                CurrentWeatherStates[currentWeatherState.Key].SuccessiveDays++;
                            }
                            else
                            {
                                CurrentWeatherStates[currentWeatherState.Key].SuccessiveDays = 1;
                            }
                        
                            CurrentWeatherStates[currentWeatherState.Key].WeatherStateSo = endProbability.WeatherStateSo;
                        
                            break;
                        }
                    }
                }
            }
        }
        
        InfoDisplayManager.instance.DisplayWeather();
    }
}
