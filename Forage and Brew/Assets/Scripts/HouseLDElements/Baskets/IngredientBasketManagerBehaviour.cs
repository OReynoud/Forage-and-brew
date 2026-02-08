using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientBasketManagerBehaviour : BasketManagerBehaviour
{
    [SerializeField] private List<IngredientBasketBehaviour> ingredientBaskets;
    [SerializeField] private IngredientListSo ingredientListSo;
    [SerializeField] private IngredientTypeListSo ingredientTypeListSo;
    [SerializeField] private float enableDisableTime = 0.5f;
    
    [Header("UI")]
    [SerializeField] private GameObject localCanvasGameObject;
    [SerializeField] private Transform currentTypeBackgroundTransform;
    [SerializeField] private List<Transform> ingredientTypeTransforms;
    
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
        
        if (GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Count == 0 &&
            GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Count == 0)
        {
            currentTypeBackgroundTransform.gameObject.SetActive(false);
        }
        
        ReactivateRightIngredientBaskets();
        
        StartCoroutine(UpdateIngredientTypeBackgroundAtStart());
    }
    
    
    public override void IncreaseCurrentSetIndex()
    {
        do
        {
            _currentIngredientSetIndex++;
            _currentIngredientSetIndex %= _ingredientSets.Count;
        } while (_ingredientSets[_currentIngredientSetIndex].All(ingredient =>
            !GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Contains(ingredient) && 
            !GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Select(behaviour => behaviour.IngredientValuesSo)
                .Contains(ingredient)));
        
        ReactivateRightIngredientBaskets();
    }
    
    public override void DecreaseCurrentSetIndex()
    {
        do
        {
            _currentIngredientSetIndex--;
            if (_currentIngredientSetIndex < 0)
            {
                _currentIngredientSetIndex = _ingredientSets.Count - 1;
            }
        } while (_ingredientSets[_currentIngredientSetIndex].All(ingredient =>
            !GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Contains(ingredient) && 
            !GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Select(behaviour => behaviour.IngredientValuesSo)
                .Contains(ingredient)));
        
        ReactivateRightIngredientBaskets();
    }
    
    private void ReactivateRightIngredientBaskets()
    {
        currentTypeBackgroundTransform.position = ingredientTypeTransforms[_currentIngredientSetIndex].position;

        int activeBasketsCount = 0;
        
        for (int i = 0; i < ingredientBaskets.Count; i++)
        {
            if (i < _ingredientSets[_currentIngredientSetIndex].Count &&
                (GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Contains(_ingredientSets[_currentIngredientSetIndex][i]) ||
                 GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Select(behaviour => behaviour.IngredientValuesSo)
                     .Contains(_ingredientSets[_currentIngredientSetIndex][i])))
            {
                activeBasketsCount++;
                ingredientBaskets[i].SetBasketContent(_ingredientSets[_currentIngredientSetIndex][i]);
                ingredientBaskets[i].StartEnable(enableDisableTime);
                ingredientBaskets[i].DoesNeedToCheckAvailability = true;
                ingredientBaskets[i].BasketVfxManager.PlaySmokescreen();
            }
            else
            {
                ingredientBaskets[i].BasketVfxManager.PlaySmokescreen();
                ingredientBaskets[i].StartDisable(enableDisableTime);
            }
        }
        
        List<IngredientTypeSo> distinctCollectedIngredientTypes = GetDistinctCollectedIngredientTypes();

        if (distinctCollectedIngredientTypes.Count < 2 &&
            distinctCollectedIngredientTypes.Contains(ingredientTypeListSo.IngredientTypes[_currentIngredientSetIndex]))
        {
            DisableChangeSet();
        }
    }
    
    private IEnumerator UpdateIngredientTypeBackgroundAtStart()
    {
        yield return new WaitForNextFrameUnit();
        
        currentTypeBackgroundTransform.position = ingredientTypeTransforms[_currentIngredientSetIndex].position;
    }
    

    private void EnableChangeSet()
    {
        if (!BasketInputManager.Instance.CurrentBasketManagers.Contains(this))
        {
            BasketInputManager.Instance.CurrentBasketManagers.Add(this);
        }

        if (_ingredientSets.Count > 1)
        {
            localCanvasGameObject.SetActive(true);
        }
    }

    private void DisableChangeSet()
    {
        BasketInputManager.Instance.CurrentBasketManagers.Remove(this);
        localCanvasGameObject.SetActive(false);
    }
    
    
    public void ManageTriggerEnter(IngredientBasketBehaviour ingredientBasket)
    {
        _currentTriggeredIngredientBaskets.Add(ingredientBasket);
    }

    public void ManageTriggerExit(IngredientBasketBehaviour ingredientBasket)
    {
        _currentTriggeredIngredientBaskets.Remove(ingredientBasket);
    }


    private void OnTriggerEnter(Collider other)
    {
        List<IngredientTypeSo> distinctCollectedIngredientTypes = GetDistinctCollectedIngredientTypes();

        if (other.CompareTag("Player") && (distinctCollectedIngredientTypes.Count >= 2 || distinctCollectedIngredientTypes.Count >= 1 &&
                !distinctCollectedIngredientTypes.Contains(ingredientTypeListSo.IngredientTypes[_currentIngredientSetIndex])))
        {
            EnableChangeSet();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DisableChangeSet();
        }
    }
    
    
    private List<IngredientTypeSo> GetDistinctCollectedIngredientTypes()
    {
        List<IngredientTypeSo> distinctCollectedIngredientTypes = new();

        foreach (IngredientValuesSo ingredientValues in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
        {
            if (!distinctCollectedIngredientTypes.Contains(ingredientValues.Type))
            {
                distinctCollectedIngredientTypes.Add(ingredientValues.Type);
            }
        }
        
        foreach (CollectedIngredientBehaviour outIngredient in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients)
        {
            if (!distinctCollectedIngredientTypes.Contains(outIngredient.IngredientValuesSo.Type))
            {
                distinctCollectedIngredientTypes.Add(outIngredient.IngredientValuesSo.Type);
            }
        }

        return distinctCollectedIngredientTypes;
    }
}
