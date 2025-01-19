using System.Collections;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class InfoDisplayManager : Singleton<InfoDisplayManager>
{
    [BoxGroup("References")] [SerializeField]
    private CanvasGroup background;

    [BoxGroup("References")] [SerializeField]
    private CanvasGroup pauseBackground;

    [BoxGroup("References")] [SerializeField]
    private RectTransform pauseMenu;

    [BoxGroup("References")] [SerializeField]
    private InputSystemUIInputModule uiInput;

    [BoxGroup("References")] [SerializeField]
    private RectTransform optionsMenu;

    [BoxGroup("References")] [SerializeField]
    private Slider musicSlider;

    [BoxGroup("References")] [SerializeField]
    private Slider sfxSlider;

    [BoxGroup("References")] [SerializeField]
    private AudioMixer mixer;


    [BoxGroup("Top Right")] [SerializeField]
    private RectTransform weatherUIContainer;

    [BoxGroup("Top Right")] [SerializeField]
    private TextMeshProUGUI daysPassedText;

    [BoxGroup("Top Right")] [SerializeField]
    private GameObject forestDisplay;

    [BoxGroup("Top Right")] [SerializeField]
    private Image forestWeather;

    [BoxGroup("Top Right")] [SerializeField]
    private GameObject swampDisplay;

    [BoxGroup("Top Right")] [SerializeField]
    private Image swampWeather;

    [BoxGroup("Top Right")] [SerializeField]
    private Image moonCyclesDisplay;

    [BoxGroup("Top Left")] [SerializeField]
    private RectTransform tutorialPopup;

    [BoxGroup("Bottom Right")] [SerializeField]
    private RectTransform moneyUIContainer;

    [BoxGroup("Bottom Right")] [SerializeField]
    private TextMeshProUGUI moneyText;

    [BoxGroup("Bottom Left")] [SerializeField]
    private RectTransform codexIcon;

    [BoxGroup("Behavior")] [SerializeField]
    private float lerp;

    [BoxGroup("Behavior")] [SerializeField] [ReadOnly]
    private bool canShow;

    [BoxGroup("Behavior")] [SerializeField]
    private Vector2 topRightShownPos;

    [BoxGroup("Behavior")] [SerializeField]
    private Vector2 topRightHiddenPos;

    [BoxGroup("Behavior")] [SerializeField]
    private Vector2 topLeftShownPos;

    [BoxGroup("Behavior")] [SerializeField]
    private Vector2 topLeftHiddenPos;

    [BoxGroup("Behavior")] [SerializeField]
    private Vector2 bottomRightShownPos;

    [BoxGroup("Behavior")] [SerializeField]
    private Vector2 bottomRightHiddenPos;

    [BoxGroup("Behavior")] [SerializeField]
    private Vector2 bottomLeftShownPos;

    [BoxGroup("Behavior")] [SerializeField]
    private Vector2 bottomLeftHiddenPos;

    public float tutorialTimer { get; set; }
    private bool showBackground;
    private bool showPause;
    private bool showOptions;
    private bool _canInputPause = true;


    public void DisplayAll()
    {
        DisplayWeather();
        DisplayDays();
        DisplayMoney();
        DisplayMoonCycles();
    }

    private void Start()
    {
        CharacterInputManager.Instance.OnInputsEnabled.AddListener(UpdateUIVisibility);
        CharacterInputManager.Instance.OnNavigationChange.AddListener(UpdateUIVisibilityReverse);

        float volume = PlayerPrefs.GetFloat(Ex.MusicVolume);
        float volumeFX = PlayerPrefs.GetFloat(Ex.SfxVolume);
        mixer.SetFloat(Ex.MusicVolume, volume == musicSlider.minValue ? -80 : volume);
        musicSlider.value = volume;
        mixer.SetFloat(Ex.SfxVolume, volumeFX == sfxSlider.minValue ? -80 : volumeFX);
        sfxSlider.value = volumeFX;
    }

    private void UpdateUIVisibility(bool arg0)
    {
        canShow = arg0;
    }

    private void UpdateUIVisibilityReverse(bool arg0)
    {
        canShow = !arg0;
    }

    private void Update()
    {
        if (canShow)
        {
            weatherUIContainer.anchoredPosition =
                Vector2.Lerp(weatherUIContainer.anchoredPosition, topRightShownPos, lerp);
            moneyUIContainer.anchoredPosition =
                Vector2.Lerp(moneyUIContainer.anchoredPosition, bottomRightShownPos, lerp);
            if (!CharacterInputManager.Instance.showCodex)
            {
                codexIcon.anchoredPosition = Vector2.Lerp(codexIcon.anchoredPosition, bottomLeftShownPos, lerp);
            }
            else
            {
                codexIcon.anchoredPosition = Vector2.Lerp(codexIcon.anchoredPosition, bottomLeftHiddenPos, lerp);
            }
        }
        else
        {
            weatherUIContainer.anchoredPosition =
                Vector2.Lerp(weatherUIContainer.anchoredPosition, topRightHiddenPos, lerp);
            moneyUIContainer.anchoredPosition =
                Vector2.Lerp(moneyUIContainer.anchoredPosition, bottomRightHiddenPos, lerp);
            codexIcon.anchoredPosition = Vector2.Lerp(codexIcon.anchoredPosition, bottomLeftHiddenPos, lerp);
        }

        if (tutorialTimer > 0)
        {
            tutorialPopup.anchoredPosition = Vector2.Lerp(tutorialPopup.anchoredPosition, topLeftShownPos, lerp);
            tutorialTimer -= Time.deltaTime;
        }
        else
        {
            tutorialPopup.anchoredPosition = Vector2.Lerp(tutorialPopup.anchoredPosition, topLeftHiddenPos, lerp);
        }

        background.alpha = Mathf.Lerp(background.alpha, showBackground ? 1 : 0, lerp);

        pauseBackground.alpha = Mathf.Lerp(pauseBackground.alpha, showPause ? 1 : 0, lerp);

        pauseMenu.anchoredPosition = Vector2.Lerp(pauseMenu.anchoredPosition,
            showPause ? Vector2.zero : new Vector2(0, Screen.height), lerp);

        optionsMenu.anchoredPosition = Vector2.Lerp(optionsMenu.anchoredPosition,
            showOptions ? Vector2.zero : new Vector2(0, Screen.height), lerp);
    }


    public void DisplayWeather()
    {
        forestDisplay.SetActive(false);
        swampDisplay.SetActive(false);

        Biome currentBiome = Biome.None;

        switch (GameDontDestroyOnLoadManager.Instance.CurrentScene)
        {
            case Scene.House:
            case Scene.Outdoor:
                break;
            case Scene.Biome1:
                currentBiome = Biome.Forest;
                break;
            case Scene.Biome2:
                currentBiome = Biome.Swamp;
                break;
        }

        switch (currentBiome)
        {
            case Biome.None:
                forestDisplay.SetActive(true);
                DisplayBiomeWeather(Biome.Forest, forestWeather);

                swampDisplay.SetActive(true);
                DisplayBiomeWeather(Biome.Swamp, swampWeather);
                break;

            case Biome.Forest:
                forestDisplay.SetActive(true);
                DisplayBiomeWeather(Biome.Forest, forestWeather);
                break;

            case Biome.Swamp:
                swampDisplay.SetActive(true);
                DisplayBiomeWeather(Biome.Swamp, swampWeather);
                break;
        }
    }

    private void DisplayBiomeWeather(Biome biome, Image weatherDisplay)
    {
        weatherDisplay.sprite = WeatherManager.Instance.CurrentWeatherStates[biome].WeatherStateSo.Icon;
        weatherDisplay.color = WeatherManager.Instance.CurrentWeatherStates[biome].WeatherStateSo.Color;
    }

    public void DisplayDays()
    {
        daysPassedText.text = "Day " + (GameDontDestroyOnLoadManager.Instance.DayPassed + 1);
    }

    public void DisplayMoney()
    {
        moneyText.text = MoneyManager.Instance.MoneyAmount.ToString();
    }

    public void DisplayMoonCycles()
    {
        moonCyclesDisplay.sprite = LunarCycleManager.Instance.CurrentLunarCycleState.Icon;
        moonCyclesDisplay.color = LunarCycleManager.Instance.CurrentLunarCycleState.Color;
    }

    public void ShowBackground()
    {
        showBackground = true;
    }

    public void HideBackground()
    {
        showBackground = false;
    }

    public void ShowPause()
    {
        if (showPause) return;
        if (!_canInputPause) return;
        
        showPause = true;
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnablePauseInputs();
        uiInput.enabled = true;
        
        _canInputPause = false;
        StartCoroutine(PauseInputBuffer());
    }

    public void HidePause()
    {
        if (!showPause) return;
        if (!_canInputPause) return;
        
        showPause = false;
        CharacterInputManager.Instance.EnableInputs();
        if (CharacterInputManager.Instance.showCodex)
        {
            CharacterInputManager.Instance.EnableCodexExit();
        }

        uiInput.enabled = false;
        
        _canInputPause = false;
        StartCoroutine(PauseInputBuffer());
    }
    
    private IEnumerator PauseInputBuffer()
    {
        yield return new WaitForNextFrameUnit();
        
        _canInputPause = true;
    }

    public void ShowOptions()
    {
        showOptions = true;
        CharacterInputManager.Instance.DisablePauseInputs();
    }

    public void HideOptions()
    {
        showOptions = false;
        CharacterInputManager.Instance.EnablePauseInputs();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateSoundSettings()
    {
        mixer.SetFloat(Ex.MusicVolume, musicSlider.value == musicSlider.minValue ? -80 : musicSlider.value);
        mixer.SetFloat(Ex.SfxVolume, sfxSlider.value == sfxSlider.minValue ? -80 : sfxSlider.value);
        PlayerPrefs.SetFloat(Ex.MusicVolume, musicSlider.value);
        PlayerPrefs.SetFloat(Ex.SfxVolume, sfxSlider.value);
        PlayerPrefs.Save();
    }
}