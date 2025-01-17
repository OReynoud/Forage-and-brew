using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SC_HouseTintin_Perso");
    }

    public void ShowOptions()
    {
        
    }

    public void ShowCredits()
    {
        
    }
    
    public void HideOptions()
    {
        
    }

    public void HideCredits()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }
}
