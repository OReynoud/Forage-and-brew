using System.Collections.Generic;
using UnityEngine;

public class WeatherLightingManager : MonoBehaviour
{
    // Singleton
    public static WeatherLightingManager Instance { get; private set; }
    
    [SerializeField] private GameObject daytimeGameObject;
    [SerializeField] private GameObject nighttimeGameObject;

    [SerializeField] private WeatherStateSo cloudWeatherState;
    [SerializeField] private List<GameObject> cloudLightingGameObjects;
    [SerializeField] private float cloudFogDensity = 0.08f;
    [SerializeField] private WeatherStateSo rainWeatherState;
    [SerializeField] private List<GameObject> rainLightingGameObjects;
    [SerializeField] private float rainFogDensity = 0.08f;
    [SerializeField] private WeatherStateSo sunWeatherState;
    [SerializeField] private List<GameObject> sunLightingGameObjects;
    [SerializeField] private float sunFogDensity;
    

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
    

    public void SetRightLighting()
    {
        SetRightDaytimeLighting(GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay);
        SetRightWeatherLighting();
    }
    
    public void SetRightDaytimeLighting(TimeOfDay timeOfDay)
    {
        daytimeGameObject.SetActive(timeOfDay == TimeOfDay.Daytime);
        nighttimeGameObject.SetActive(timeOfDay == TimeOfDay.Nighttime);
    }
    
    
    public void SetRightWeatherLighting()
    {
        if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == cloudWeatherState)
        {
            SetCloudLighting();
        }
        else if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == rainWeatherState)
        {
            SetRainLighting();
        }
        else if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == sunWeatherState)
        {
            SetSunLighting();
        }
    }
    
    public void SetCloudLighting()
    {
        foreach (GameObject lightingGameObject in cloudLightingGameObjects)
        {
            lightingGameObject.SetActive(true);
        }
        foreach (GameObject lightingGameObject in rainLightingGameObjects)
        {
            lightingGameObject.SetActive(false);
        }
        foreach (GameObject lightingGameObject in sunLightingGameObjects)
        {
            lightingGameObject.SetActive(false);
        }
        
        RenderSettings.fog = cloudFogDensity > 0f;
        RenderSettings.fogDensity = cloudFogDensity;
    }
    
    public void SetRainLighting()
    {
        foreach (GameObject lightingGameObject in cloudLightingGameObjects)
        {
            lightingGameObject.SetActive(false);
        }
        foreach (GameObject lightingGameObject in rainLightingGameObjects)
        {
            lightingGameObject.SetActive(true);
        }
        foreach (GameObject lightingGameObject in sunLightingGameObjects)
        {
            lightingGameObject.SetActive(false);
        }
        
        RenderSettings.fog = rainFogDensity > 0f;
        RenderSettings.fogDensity = rainFogDensity;
    }
    
    public void SetSunLighting()
    {
        foreach (GameObject lightingGameObject in cloudLightingGameObjects)
        {
            lightingGameObject.SetActive(false);
        }
        foreach (GameObject lightingGameObject in rainLightingGameObjects)
        {
            lightingGameObject.SetActive(false);
        }
        foreach (GameObject lightingGameObject in sunLightingGameObjects)
        {
            lightingGameObject.SetActive(true);
        }
        
        RenderSettings.fog = sunFogDensity > 0f;
        RenderSettings.fogDensity = sunFogDensity;
    }
}
