using System;
using UnityEngine;
using UnityEngine.Audio;

public class CharacterVfxManager : MonoBehaviour
{
    // Singleton
    public static CharacterVfxManager Instance { get; private set; }

    [Header("Rain")]
    [SerializeField] private WeatherStateSo rainWeatherState;
    [SerializeField] private WeatherStateSo cloudyWeatherState;
    [SerializeField] private WeatherStateSo sunnyWeatherState;
    [SerializeField] private GameObject rainVfxGameObject;
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


    public void CheckForRainVfx(Scene scene)
    {
        switch (scene)
        {
            case Scene.House:
                musicSource.resource = musicHouse[0];
                musicSource.Play();
                break;
            case Scene.Outdoor:
                musicSource.resource = musicOutdoor[0];
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
                    PlayRainVfx();
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
                    PlayRainVfx();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
        }
        

        if (scene == Scene.Biome2 && WeatherManager.Instance.CurrentWeatherStates[Biome.Swamp].WeatherStateSo == rainWeatherState)
        {
            PlayRainVfx();
        }

        

    }
    
    public void PlayRainVfx()
    {
        rainVfxGameObject.SetActive(true);
    }
}
