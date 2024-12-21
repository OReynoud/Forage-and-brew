using System.Linq;
using UnityEngine;

public class OutStackableManager : MonoBehaviour
{
    // Singleton
    public static OutStackableManager Instance { get; private set; }
    
    [SerializeField] private CollectedIngredientBehaviour collectedIngredientPrefab;
    [SerializeField] private CollectedPotionBehaviour collectedPotionPrefab;
    
    
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
    
    private void Start()
    {
        InstantiateOutCollectedIngredients();
        InstantiateOutCookedPotions();
    }
    
    
    private void InstantiateOutCollectedIngredients()
    {
        foreach (var floorCollectedIngredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.ToList())
        {
            CollectedIngredientBehaviour collectedIngredient = Instantiate(collectedIngredientPrefab, floorCollectedIngredient.position, floorCollectedIngredient.rotation);
            collectedIngredient.IngredientValuesSo = floorCollectedIngredient.ingredient;
            GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Add(collectedIngredient);
            GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.Remove(floorCollectedIngredient);
        }
    }
    
    private void InstantiateOutCookedPotions()
    {
        foreach (var floorCookedPotion in GameDontDestroyOnLoadManager.Instance.FloorCookedPotions.ToList())
        {
            CollectedPotionBehaviour collectedPotion = Instantiate(collectedPotionPrefab, floorCookedPotion.position, floorCookedPotion.rotation);
            collectedPotion.PotionValuesSo = floorCookedPotion.potion;
            GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Add(collectedPotion);
            GameDontDestroyOnLoadManager.Instance.FloorCookedPotions.Remove(floorCookedPotion);
        }
    }
}
