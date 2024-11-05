using UnityEngine;

public class CharacterDontDestroyOnLoadBehaviour : MonoBehaviour
{
    public static CharacterDontDestroyOnLoadBehaviour Instance { get; private set; }
    
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
