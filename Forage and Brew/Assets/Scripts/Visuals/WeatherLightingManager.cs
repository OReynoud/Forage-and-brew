using UnityEngine;
using UnityEngine.Rendering;

public class WeatherLightingManager : MonoBehaviour
{
    // Singleton
    public static WeatherLightingManager Instance { get; private set; }

    [SerializeField] private Volume globalVolume;
    [SerializeField] private WeatherStateSo cloudWeatherState;
    [SerializeField] private GameObject cloudLightingGameObject;
    [SerializeField] private VolumeProfile cloudVolumeProfile;
    [SerializeField] private WeatherStateSo rainWeatherState;
    [SerializeField] private GameObject rainLightingGameObject;
    [SerializeField] private VolumeProfile rainVolumeProfile;
    [SerializeField] private WeatherStateSo sunWeatherState;
    [SerializeField] private GameObject sunLightingGameObject;
    [SerializeField] private VolumeProfile sunVolumeProfile;
    

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
        globalVolume.profile = cloudVolumeProfile;
    }
    
    public void SetRainLighting()
    {
        cloudLightingGameObject.SetActive(false);
        rainLightingGameObject.SetActive(true);
        sunLightingGameObject.SetActive(false);
        globalVolume.profile = rainVolumeProfile;
    }
    
    public void SetSunLighting()
    {
        cloudLightingGameObject.SetActive(false);
        rainLightingGameObject.SetActive(false);
        sunLightingGameObject.SetActive(true);
        globalVolume.profile = sunVolumeProfile;
    }
}
