using System.Collections.Generic;
using UnityEngine;

public class IngredientBasketManagerBehaviour : BasketManagerBehaviour
{
    [SerializeField] private List<IngredientBasketBehaviour> ingredientBaskets;
    [SerializeField] private IngredientListSo ingredientListSo;
    [SerializeField] private IngredientTypeListSo ingredientTypeListSo;
    
    private readonly List<IngredientBasketBehaviour> _currentTriggeredIngredientBaskets = new();
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
        int setIndex = -1;
        int ingredientIndex = 0;

        foreach (IngredientTypeSo ingredientType in ingredientTypeListSo.IngredientTypes)
        {
            foreach (IngredientValuesSo ingredient in ingredientListSo.IngredientValues)
            {
                if (ingredientType != ingredient.Type) continue;
                
                if (ingredientIndex == 0)
                {
                    _ingredientSets.Add(new List<IngredientValuesSo>());
                    setIndex++;
                }
            
                _ingredientSets[setIndex].Add(ingredient);
            
                ingredientIndex++;
                ingredientIndex %= ingredientBaskets.Count;
            }
            
            ingredientIndex = 0;
        }
        
        ReactivateRightIngredientBaskets();
    }
    
    
    public override void IncreaseCurrentSetIndex()
    {
        _currentIngredientSetIndex++;
        _currentIngredientSetIndex %= _ingredientSets.Count;
        
        ReactivateRightIngredientBaskets();
    }
    
    public override void DecreaseCurrentSetIndex()
    {
        _currentIngredientSetIndex--;
        if (_currentIngredientSetIndex < 0)
        {
            _currentIngredientSetIndex = _ingredientSets.Count - 1;
        }
        
        ReactivateRightIngredientBaskets();
    }
    
    private void ReactivateRightIngredientBaskets()
    {
        for (int i = 0; i < ingredientBaskets.Count; i++)
        {
            if (i < _ingredientSets[_currentIngredientSetIndex].Count)
            {
                ingredientBaskets[i].SetBasketContent(_ingredientSets[_currentIngredientSetIndex][i]);
                ingredientBaskets[i].gameObject.SetActive(true);
                ingredientBaskets[i].DoesNeedToCheckAvailability = true;
            }
            else
            {
                ingredientBaskets[i].gameObject.SetActive(false);
            }
        }
    }
    
    
    public void ManageTriggerEnter(IngredientBasketBehaviour ingredientBasket)
    {
        if (_currentTriggeredIngredientBaskets.Count == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Add(this);
        }
        
        _currentTriggeredIngredientBaskets.Add(ingredientBasket);
    }
    
    public void ManageTriggerExit(IngredientBasketBehaviour ingredientBasket)
    {
        _currentTriggeredIngredientBaskets.Remove(ingredientBasket);
        
        if (_currentTriggeredIngredientBaskets.Count == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Remove(this);
        }
    }
}
