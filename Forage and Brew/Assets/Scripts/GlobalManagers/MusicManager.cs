using System;
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
    
    [SerializeField] private AudioResource[] musicHouse;
    [SerializeField] private AudioResource[] musicForest;
    [SerializeField] private AudioResource[] ambianceForest;
    [SerializeField] private AudioResource[] musicSwamp;
    [SerializeField] private AudioResource[] musicOutdoor;

    
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
            // TODO: Trigger behaviour
            case Scene.HouseOutdoor:
                musicSource.resource = musicHouse[0];
                musicSource.Play();
                break;
            case Scene.Biome1:
                if (WeatherManager.Instance.CurrentWeatherStates[Biome.Forest].WeatherStateSo == cloudyWeatherState)
                {
                    musicSource.resource = musicForest[0];
                    musicSource.Play();
                    ambianceSource.resource = ambianceForest[0];
                    ambianceSource.Play();
                }
                else if (WeatherManager.Instance.CurrentWeatherStates[Biome.Forest].WeatherStateSo == sunnyWeatherState)
                {
                    musicSource.resource = musicForest[1];
                    musicSource.Play();
                }
                else if (WeatherManager.Instance.CurrentWeatherStates[Biome.Forest].WeatherStateSo == rainWeatherState)
                {
                    musicSource.resource = musicForest[2];
                    musicSource.Play();
                }
                break;
            case Scene.Biome2:
                if (WeatherManager.Instance.CurrentWeatherStates[Biome.Swamp].WeatherStateSo == cloudyWeatherState)
                {
                    musicSource.resource = musicSwamp[0];
                    musicSource.Play();
                }
                else if (WeatherManager.Instance.CurrentWeatherStates[Biome.Swamp].WeatherStateSo == sunnyWeatherState)
                {
                    musicSource.resource = musicSwamp[1];
                    musicSource.Play();
                }
                else if (WeatherManager.Instance.CurrentWeatherStates[Biome.Swamp].WeatherStateSo == rainWeatherState)
                {
                    musicSource.resource = musicSwamp[2];
                    musicSource.Play();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
        }
    }
}
