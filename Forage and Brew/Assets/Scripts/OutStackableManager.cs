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
        foreach (FloorIngredient floorCollectedIngredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.ToList())
        {
            CollectedIngredientBehaviour collectedIngredient = Instantiate(collectedIngredientPrefab,
                floorCollectedIngredient.Position, floorCollectedIngredient.Rotation);
            collectedIngredient.IngredientValuesSo = floorCollectedIngredient.Ingredient;
            GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Add(collectedIngredient);
            GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.Remove(floorCollectedIngredient);
        }
    }
    
    private void InstantiateOutCookedPotions()
    {
        foreach (FloorCookedPotion floorCookedPotion in GameDontDestroyOnLoadManager.Instance.FloorCookedPotions.ToList())
        {
            CollectedPotionBehaviour collectedPotion = Instantiate(collectedPotionPrefab, floorCookedPotion.Position,
                floorCookedPotion.Rotation);
            collectedPotion.PotionValuesSo = floorCookedPotion.Potion;
            GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Add(collectedPotion);
            GameDontDestroyOnLoadManager.Instance.FloorCookedPotions.Remove(floorCookedPotion);
        }
    }

    public void StoreOutCollectedIngredients()
    {
        foreach (CollectedIngredientBehaviour collectedIngredientBehaviour in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.ToList())
        {
            GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Remove(collectedIngredientBehaviour);
            GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.Add(new FloorIngredient(
                collectedIngredientBehaviour.IngredientValuesSo, collectedIngredientBehaviour.transform.position,
                collectedIngredientBehaviour.transform.rotation));
        }
    }

    public void StoreOutCookedPotions()
    {
        foreach (CollectedPotionBehaviour collectedPotionBehaviour in GameDontDestroyOnLoadManager.Instance.OutCookedPotions.ToList())
        {
            GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Remove(collectedPotionBehaviour);
            GameDontDestroyOnLoadManager.Instance.FloorCookedPotions.Add(new FloorCookedPotion(
                collectedPotionBehaviour.PotionValuesSo, collectedPotionBehaviour.transform.position,
                collectedPotionBehaviour.transform.rotation));
        }
    }
}
