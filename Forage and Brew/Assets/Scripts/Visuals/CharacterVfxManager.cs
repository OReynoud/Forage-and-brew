using System.Collections;
using UnityEngine;

public class CharacterVfxManager : MonoBehaviour
{
    // Singleton
    public static CharacterVfxManager Instance { get; private set; }

    [Header("Rain")]
    [SerializeField] private WeatherStateSo rainWeatherState;
    [SerializeField] private GameObject rainVfxGameObject;

    [Header("Puff")]
    [SerializeField] private ParticleSystem puffVfxParticleSystem;

    
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


    #region Rain VFX

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
        else
        {
            StopRainVfx();
        }
    }
    
    public void PlayRainVfx()
    {
        rainVfxGameObject.SetActive(true);
    }
    
    public void StopRainVfx()
    {
        rainVfxGameObject.SetActive(false);
    }

    #endregion


    #region Puff VFX

    public void PlayPuffVfx()
    {
        puffVfxParticleSystem.Play();
    }

    #endregion
}
