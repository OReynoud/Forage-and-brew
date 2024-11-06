using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    // Singleton
    public static WeatherManager Instance { get; private set; }
    
    [SerializeField] private WeatherStateSo biome1StartingWeatherState;
    
    private WeatherStateSo _biome1CurrentWeatherState;
    private int _biome1CurrentWeatherStateSuccessiveCount;
    

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
        _biome1CurrentWeatherState = biome1StartingWeatherState;
        _biome1CurrentWeatherStateSuccessiveCount = 1;
        Debug.Log("The weather state for the first day is " + _biome1CurrentWeatherState.Name);
    }
    
    
    public void PassToNextWeatherState()
    {
        foreach (WeatherStateEndProbabilityBySuccessiveDayNumber weatherStateEndProbability in _biome1CurrentWeatherState.EndProbabilities)
        {
            if (weatherStateEndProbability.SuccessiveDayNumber == _biome1CurrentWeatherStateSuccessiveCount)
            {
                float randomValue = Random.Range(0f, 1000f);
                float cumulativeProbability = 0f;
                
                foreach (WeatherStateEndProbability endProbability in weatherStateEndProbability.WeatherStateEndProbabilities)
                {
                    cumulativeProbability += endProbability.EndProbability;
                    
                    if (randomValue <= cumulativeProbability)
                    {
                        Debug.Log("The weather state for the next day is " + endProbability.WeatherStateSo.Name);
                        
                        if (endProbability.WeatherStateSo == _biome1CurrentWeatherState)
                        {
                            _biome1CurrentWeatherStateSuccessiveCount++;
                            return;
                        }
                        
                        _biome1CurrentWeatherState = endProbability.WeatherStateSo;
                        _biome1CurrentWeatherStateSuccessiveCount = 1;
                        
                        return;
                    }
                }
            }
        }
    }
}
