using UnityEngine;

public class CharacterVfxManager : MonoBehaviour
{
    // Singleton
    public static CharacterVfxManager Instance { get; private set; }

    [Header("Rain")]
    [SerializeField] private WeatherStateSo rainWeatherState;
    [SerializeField] private GameObject rainVfxGameObject;

    
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
        if (scene == Scene.Biome1 && WeatherManager.Instance.CurrentWeatherStates[Biome.Forest].WeatherStateSo == rainWeatherState ||
            scene == Scene.Biome2 && WeatherManager.Instance.CurrentWeatherStates[Biome.Swamp].WeatherStateSo == rainWeatherState)
        {
            PlayRainVfx();
        }
    }
    
    public void PlayRainVfx()
    {
        rainVfxGameObject.SetActive(true);
    }
}
