using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup background;
    private bool showBackground;
    private bool showOptions;
    private bool showCredits;
    public RectTransform options;
    public RectTransform credits;
    
    
    public Slider musicSlider;
    public Slider sfxSlider;
    
    public AudioMixer mixer;

    public float alphaLerp;
    public float posLerp;
    public void StartGame()
    {
        SceneManager.LoadScene("SC_HouseTintin_Perso");
    }

    public void ShowOptions()
    {
        showBackground = true;
        showOptions = true;
    }

    public void ShowCredits()
    {
        showBackground = true;
        showCredits = true;
    }
    
    public void HideOptions()
    {
        showBackground = false;
        showOptions = false;
    }

    public void HideCredits()
    {
        showBackground = false;
        showCredits = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Start()
    {
        mixer.GetFloat("musicVolume", out float volume);
        musicSlider.value = volume;
        mixer.GetFloat("sfxVolume", out volume);
        sfxSlider.value = volume;
    }

    private void Update()
    {
        
        background.alpha = Mathf.Lerp(background.alpha, showBackground ? 1 : 0, alphaLerp);

        options.anchoredPosition = Vector2.Lerp(options.anchoredPosition, showOptions ? Vector2.zero : new Vector2(0, Screen.height), posLerp);
        
        
        credits.anchoredPosition = Vector2.Lerp(credits.anchoredPosition, showCredits ? Vector2.zero : new Vector2(0, Screen.height), posLerp);

    }

    public void UpdateSoundSettings()
    {
        
        mixer.SetFloat("musicVolume", musicSlider.value == musicSlider.minValue ? -80 : musicSlider.value );
        mixer.SetFloat("sfxVolume", sfxSlider.value == sfxSlider.minValue ? -80 : sfxSlider.value );
        
    }
}
