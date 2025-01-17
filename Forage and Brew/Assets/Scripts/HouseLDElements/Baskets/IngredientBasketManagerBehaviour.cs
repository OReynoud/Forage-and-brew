using System.Collections.Generic;
using UnityEngine;

public class IngredientBasketManagerBehaviour : BasketManagerBehaviour
{
    [SerializeField] private List<IngredientBasketBehaviour> ingredientBaskets;
    [SerializeField] private IngredientListSo ingredientListSo;
    [SerializeField] private IngredientTypeListSo ingredientTypeListSo;
    [SerializeField] private float enableDisableTime = 0.5f;
    
    [Header("UI")]
    [SerializeField] private GameObject localCanvasGameObject;
    
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
        localCanvasGameObject.SetActive(false);
        
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
                ingredientBaskets[i].StartEnable(enableDisableTime);
                ingredientBaskets[i].DoesNeedToCheckAvailability = true;
                ingredientBaskets[i].BasketVfxManager.PlaySmokescreen();
            }
            else
            {
                if (i < _ingredientSets[_currentIngredientSetIndex - 1].Count)
                {
                    ingredientBaskets[i].BasketVfxManager.PlaySmokescreen();
                    ingredientBaskets[i].StartDisable(enableDisableTime);
                }
            }
        }
    }
    
    
    public void ManageTriggerEnter(IngredientBasketBehaviour ingredientBasket)
    {
        if (_currentTriggeredIngredientBaskets.Count == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Add(this);
            
            if (_ingredientSets.Count > 1)
            {
                localCanvasGameObject.SetActive(true);
            }
        }
        
        _currentTriggeredIngredientBaskets.Add(ingredientBasket);
    }
    
    public void ManageTriggerExit(IngredientBasketBehaviour ingredientBasket)
    {
        _currentTriggeredIngredientBaskets.Remove(ingredientBasket);
        
        if (_currentTriggeredIngredientBaskets.Count == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Remove(this);
            localCanvasGameObject.SetActive(false);
        }
    }
}
