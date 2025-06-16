using System.Collections.Generic;
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
        InstantiateOutCookedPotions(GameDontDestroyOnLoadManager.Instance.FloorCookedPotions,
            GameDontDestroyOnLoadManager.Instance.OutCookedPotions);
    }
    
    
    public void InstantiateOutCollectedIngredients()
    {
        foreach (FloorIngredient floorCollectedIngredient in GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.ToList())
        {
            CollectedIngredientBehaviour collectedIngredient = Instantiate(collectedIngredientPrefab,
                floorCollectedIngredient.Position, floorCollectedIngredient.Rotation);
            collectedIngredient.IngredientValuesSo = floorCollectedIngredient.Ingredient;
            collectedIngredient.CookedForm = floorCollectedIngredient.CookedForm;
            GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Add(collectedIngredient);
            GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.Remove(floorCollectedIngredient);
        }
    }
    
    public void InstantiateOutCookedPotions(List<FloorCookedPotion> floorCookedPotions, List<CollectedPotionBehaviour> collectedPotions)
    {
        foreach (FloorCookedPotion floorCookedPotion in floorCookedPotions.ToList())
        {
            CollectedPotionBehaviour collectedPotion = Instantiate(collectedPotionPrefab, floorCookedPotion.Position,
                floorCookedPotion.Rotation);
            collectedPotion.PotionValuesSo = floorCookedPotion.Potion;
            collectedPotions.Add(collectedPotion);
            floorCookedPotions.Remove(floorCookedPotion);
        }
    }

    public void StoreOutCollectedIngredients()
    {
        foreach (CollectedIngredientBehaviour collectedIngredientBehaviour in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.ToList())
        {
            GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Remove(collectedIngredientBehaviour);
            GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.Add(new FloorIngredient(
                collectedIngredientBehaviour.IngredientValuesSo, collectedIngredientBehaviour.CookedForm,
                collectedIngredientBehaviour.transform.position, collectedIngredientBehaviour.transform.rotation));
        }
    }

    public void StoreOutCookedPotions(List<FloorCookedPotion> floorCookedPotions, List<CollectedPotionBehaviour> collectedPotions)
    {
        foreach (CollectedPotionBehaviour collectedPotionBehaviour in collectedPotions.ToList())
        {
            collectedPotions.Remove(collectedPotionBehaviour);
            floorCookedPotions.Add(new FloorCookedPotion(
                collectedPotionBehaviour.PotionValuesSo, collectedPotionBehaviour.transform.position,
                collectedPotionBehaviour.transform.rotation));
        }
    }
}
