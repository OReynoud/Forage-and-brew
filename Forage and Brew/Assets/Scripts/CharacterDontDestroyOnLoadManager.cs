using UnityEngine;

public class CharacterDontDestroyOnLoadManager : MonoBehaviour
{
    public static CharacterDontDestroyOnLoadManager Instance { get; private set; }
    
    public Scene PreviousScene { get; set; }
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
}
