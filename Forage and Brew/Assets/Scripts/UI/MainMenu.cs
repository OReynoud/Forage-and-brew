using System.IO;
using System.Text;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private bool doesShowCursor;
    
    [BoxGroup("SaveSystem")]     
    [SerializeField] private SceneListSo sceneListSo;
    
    public CanvasGroup background;
    private bool showBackground;
    public bool showOptions;
    public bool showCredits;
    public RectTransform options;
    public RectTransform credits;
    
    
    public TextMeshProUGUI versionText;
    
    public Slider musicSlider;
    public Slider sfxSlider;
    
    public AudioMixer mixer;

    public float alphaLerp;
    public float posLerp;
    
    private readonly Encoding _encoding = Encoding.UTF8;
    
    public void StartGame()
    {
        if (File.Exists(SaveManager.FilePath))
        {
            string jsonData = File.ReadAllText(SaveManager.FilePath, _encoding);
            var data = JsonUtility.FromJson<SaveManager.SavedData>(jsonData);
        }
        SceneManager.LoadScene("SC_NewHouse");
        
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
    
    public void ClearData()
    {
        SaveManager.DeleteSave(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Start()
    {
        versionText.text = "v. " + Application.version;
        
        if (!doesShowCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        float volume = PlayerPrefs.GetFloat(Ex.MusicVolume);
        float volumeFX = PlayerPrefs.GetFloat(Ex.SfxVolume);
        mixer.SetFloat(Ex.MusicVolume, volume == musicSlider.minValue ? -80 : volume);
        musicSlider.value = volume;
        mixer.SetFloat(Ex.SfxVolume, volumeFX == sfxSlider.minValue ? -80 : volumeFX);
        sfxSlider.value = volumeFX;
        // Debug.Log(PlayerPrefs.GetFloat(Ex.SfxVolume));
        
    }

    private void Update()
    {
        
        background.alpha = Mathf.Lerp(background.alpha, showBackground ? 1 : 0, alphaLerp);

        options.anchoredPosition = Vector2.Lerp(options.anchoredPosition, showOptions ? Vector2.zero : new Vector2(0, options.rect.height), posLerp);
        
        credits.anchoredPosition = Vector2.Lerp(credits.anchoredPosition, showCredits ? Vector2.zero : new Vector2(0, credits.rect.height), posLerp);

    }

    public void UpdateSoundSettings()
    {
        
        mixer.SetFloat(Ex.MusicVolume, musicSlider.value == musicSlider.minValue ? -80 : musicSlider.value );
        mixer.SetFloat(Ex.SfxVolume, sfxSlider.value == sfxSlider.minValue ? -80 : sfxSlider.value );
        PlayerPrefs.SetFloat(Ex.MusicVolume, musicSlider.value);
        PlayerPrefs.SetFloat(Ex.SfxVolume, sfxSlider.value);
        PlayerPrefs.Save();
        // Debug.Log(PlayerPrefs.GetFloat(Ex.SfxVolume));
    }
}
