using UnityEngine;

public class WeatherLightingManager : MonoBehaviour
{
    // Singleton
    public static WeatherLightingManager Instance { get; private set; }

    [SerializeField] private WeatherStateSo cloudWeatherState;
    [SerializeField] private GameObject cloudLightingGameObject;
    [SerializeField] private WeatherStateSo rainWeatherState;
    [SerializeField] private GameObject rainLightingGameObject;
    [SerializeField] private WeatherStateSo sunWeatherState;
    [SerializeField] private GameObject sunLightingGameObject;
    

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
    
    
    public void SetRightLighting(Scene scene)
    {
        if (scene == Scene.Biome1)
        {
            if (WeatherManager.Instance.CurrentWeatherStates[Biome.Forest].WeatherStateSo == cloudWeatherState)
            {
                SetCloudLighting();
            }
            else if (WeatherManager.Instance.CurrentWeatherStates[Biome.Forest].WeatherStateSo == rainWeatherState)
            {
                SetRainLighting();
            }
            else if (WeatherManager.Instance.CurrentWeatherStates[Biome.Forest].WeatherStateSo == sunWeatherState)
            {
                SetSunLighting();
            }
        }
    }
    
    public void SetCloudLighting()
    {
        cloudLightingGameObject.SetActive(true);
        rainLightingGameObject.SetActive(false);
        sunLightingGameObject.SetActive(false);
    }
    
    public void SetRainLighting()
    {
        cloudLightingGameObject.SetActive(false);
        rainLightingGameObject.SetActive(true);
        sunLightingGameObject.SetActive(false);
    }
    
    public void SetSunLighting()
    {
        cloudLightingGameObject.SetActive(false);
        rainLightingGameObject.SetActive(false);
        sunLightingGameObject.SetActive(true);
    }
}
