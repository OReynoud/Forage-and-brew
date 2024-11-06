using System.Collections.Generic;
using UnityEngine;

public class CharacterDontDestroyOnLoadManager : MonoBehaviour
{
    public static CharacterDontDestroyOnLoadManager Instance { get; private set; }
    
    public Scene PreviousScene { get; set; }
    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Daytime;
    
    public List<IngredientValuesSo> CollectedIngredients { get; private set; } = new();
    
    
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
