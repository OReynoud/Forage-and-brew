using System.Collections.Generic;
using UnityEngine;

public class IngredientBasketManagerBehaviour : BasketManagerBehaviour
{
    [SerializeField] private List<IngredientBasketBehaviour> ingredientBaskets;
    [SerializeField] private IngredientListSo ingredientListSo;
    
    private int _currentTriggeredBasketCount;
    private readonly List<List<IngredientValuesSo>> _ingredientSets = new();
    private int _currentIngredientSetIndex;

    private void Awake()
    {
        foreach (IngredientBasketBehaviour ingredientBasket in ingredientBaskets)
        {
            ingredientBasket.IngredientBasketManagerBehaviour = this;
        }
    }

    private void Start()
    {
        int setIndex = 0;
        int ingredientIndex = 0;
        
        _ingredientSets.Add(new List<IngredientValuesSo>());
        
        foreach (IngredientValuesSo ingredient in ingredientListSo.IngredientValues)
        {
            if (ingredientIndex == ingredientBaskets.Count)
            {
                _ingredientSets.Add(new List<IngredientValuesSo>());
                setIndex++;
                ingredientIndex = 0;
            }
            
            _ingredientSets[setIndex - 1].Add(ingredient);
            ingredientIndex++;
        }
        
        for (int i = 0; i < ingredientBaskets.Count; i++)
        {
            ingredientBaskets[i].SetBasketContent(ingredientListSo.IngredientValues[i]);
        }
    }
    
    public override void IncreaseCurrentOrderIndex()
    {
        _currentIngredientSetIndex++;
        _currentIngredientSetIndex %= GameDontDestroyOnLoadManager.Instance.OrderPotions.Count;
        
        ReactivateRightPotionBaskets();
    }
    
    public override void DecreaseCurrentOrderIndex()
    {
        _currentIngredientSetIndex--;
        _currentIngredientSetIndex %= GameDontDestroyOnLoadManager.Instance.OrderPotions.Count;
        
        ReactivateRightPotionBaskets();
    }
    
    private void ReactivateRightPotionBaskets()
    {
        foreach (PotionBasketBehaviour potionBasket in ingredientBaskets)
        {
            potionBasket.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentIngredientSetIndex].Count; i++)
        {
            ingredientBaskets[i].gameObject.SetActive(true);
            ingredientBaskets[i].SetBasketContent(_currentIngredientSetIndex);
        }
    }
    
    public void ManageTriggerEnter()
    {
        if (_currentTriggeredBasketCount == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Add(this);
        }
        
        _currentTriggeredBasketCount++;
    }
    
    public void ManageTriggerExit()
    {
        _currentTriggeredBasketCount--;
        
        if (_currentTriggeredBasketCount == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Remove(this);
        }
    }
}
