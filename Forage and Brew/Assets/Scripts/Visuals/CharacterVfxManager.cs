using System.Collections;
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


    public void CheckForRainVfx()
    {
        StartCoroutine(CheckForRainVfxCoroutine());
    }

    private IEnumerator CheckForRainVfxCoroutine()
    {
        yield return new WaitUntil(() => WeatherManager.Instance);
        
        if (WeatherManager.Instance.CurrentWeatherState.WeatherStateSo == rainWeatherState)
        {
            PlayRainVfx();
        }
    }
    
    public void PlayRainVfx()
    {
        rainVfxGameObject.SetActive(true);
    }
}
