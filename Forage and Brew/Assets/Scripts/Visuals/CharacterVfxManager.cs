using UnityEngine;
using UnityEngine.Audio;

public class CharacterVfxManager : MonoBehaviour
{
    // Singleton
    public static CharacterVfxManager Instance { get; private set; }

    [Header("Rain")]
    [SerializeField] private WeatherStateSo rainWeatherState;
    [SerializeField] private GameObject rainVfxGameObject;
    [SerializeField] private AudioSource musicSource;
    
    [SerializeField] private AudioResource musicHouse;

    
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
        if (scene == Scene.Biome1 && WeatherManager.Instance.CurrentWeatherStates[Biome.Forest].WeatherStateSo == rainWeatherState)
        {
            //musicSource.resource
            musicSource.Play();
            PlayRainVfx();
        }

        if (scene == Scene.Biome2 && WeatherManager.Instance.CurrentWeatherStates[Biome.Swamp].WeatherStateSo == rainWeatherState)
        {
            PlayRainVfx();
        }

        if (scene == Scene.House && GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay == TimeOfDay.Nighttime)
        {
            musicSource.Play();
        }
    }
    
    public void PlayRainVfx()
    {
        rainVfxGameObject.SetActive(true);
    }
}
