using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    // Singleton
    public static MusicManager Instance { get; private set; }
    
    [SerializeField] private WeatherStateSo rainWeatherState;
    [SerializeField] private WeatherStateSo cloudyWeatherState;
    [SerializeField] private WeatherStateSo sunnyWeatherState;
    
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambianceSource;
    
    [SerializeField] private AudioResource[] ambianceForest;
    
    [SerializeField] private List<MusicContainer> allMusics = new List<MusicContainer>();

    
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
    
    
    public void PlaySceneMucic(Scene scene)
    {
        switch (scene)
        {
            case Scene.HouseOutdoor:
                if (PinnedRecipe.instance.isInHouse)
                {
                    musicSource.resource = allMusics.Find(x => x.playsInHouse).Music;
                }
                else
                {
                    musicSource.resource = allMusics.Find(x => x.playsInHouse == false && x.Scene == Scene.HouseOutdoor).Music;
                }
                musicSource.Play();
                break;
            case Scene.Biome1:
                if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == cloudyWeatherState)
                {
                    musicSource.resource = allMusics.Find(x => x.Scene == Scene.Biome1 && x.Weather == cloudyWeatherState).Music;
                    ambianceSource.resource = ambianceForest[0];
                    ambianceSource.Play();
                }
                else if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == sunnyWeatherState)
                {
                    musicSource.resource = allMusics.Find(x => x.Scene == Scene.Biome1 && x.Weather == sunnyWeatherState).Music;
                }
                else if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == rainWeatherState)
                {
                    musicSource.resource = allMusics.Find(x => x.Scene == Scene.Biome1 && x.Weather == rainWeatherState).Music;
                }
                break;
            case Scene.Biome2:
                if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == cloudyWeatherState)
                {
                    musicSource.resource = allMusics.Find(x => x.Scene == Scene.Biome2 && x.Weather == cloudyWeatherState).Music;
                }
                else if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == sunnyWeatherState)
                {
                    musicSource.resource = allMusics.Find(x => x.Scene == Scene.Biome2 && x.Weather == sunnyWeatherState).Music;
                }
                else if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == rainWeatherState)
                {
                    musicSource.resource = allMusics.Find(x => x.Scene == Scene.Biome1 && x.Weather == rainWeatherState).Music;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
        }
        musicSource.Play();
    }
}
