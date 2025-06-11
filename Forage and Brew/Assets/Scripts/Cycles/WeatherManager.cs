using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    // Singleton
    public static WeatherManager Instance { get; private set; }
    
    [SerializeField] private WeatherStateSo startingWeatherState;
    
    public WeatherSuccessiveDays CurrentWeatherState { get; set; }

    

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
            CurrentWeatherState = new WeatherSuccessiveDays(startingWeatherState, 1);
        }
        Debug.Log("The weather state for the first day is " + CurrentWeatherState.WeatherStateSo.Name +
                  " in the forest and the swamp.");
        InfoDisplayManager.instance.DisplayWeather();
    }
    
    
    public void PassToNextWeatherState()
    {
        foreach (WeatherStateEndProbabilityBySuccessiveDayNumber weatherStateEndProbability in CurrentWeatherState.WeatherStateSo.EndProbabilities)
        {
            if (weatherStateEndProbability.SuccessiveDayNumber != CurrentWeatherState.SuccessiveDays) continue;
            
            float randomValue = Random.Range(0f, 1000f);
            float cumulativeProbability = 0f;
                
            foreach (WeatherStateEndProbability endProbability in weatherStateEndProbability.WeatherStateEndProbabilities)
            {
                cumulativeProbability += endProbability.EndProbability;
                    
                if (randomValue <= cumulativeProbability)
                {
                    Debug.Log("The weather state for the next day is " + endProbability.WeatherStateSo.Name);
                        
                    if (endProbability.WeatherStateSo == CurrentWeatherState.WeatherStateSo)
                    {
                        CurrentWeatherState.SuccessiveDays++;
                    }
                    else
                    {
                        CurrentWeatherState.SuccessiveDays = 1;
                    }
                        
                    CurrentWeatherState.WeatherStateSo = endProbability.WeatherStateSo;
                        
                    break;
                }
            }
        }
        
        InfoDisplayManager.instance.DisplayWeather();
    }
}
