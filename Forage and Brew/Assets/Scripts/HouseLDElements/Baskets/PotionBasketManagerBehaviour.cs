using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PotionBasketManagerBehaviour : BasketManagerBehaviour
{
    [SerializeField] private List<PotionBasketBehaviour> potionBaskets;
    [SerializeField] private GameObject clientCanvasGameObject;
    [SerializeField] private TMP_Text clientNameText;
    
    private readonly List<PotionBasketBehaviour> _currentTriggeredPotionBaskets = new();
    private int _currentOrderIndex = -1;

    
    private void Awake()
    {
        foreach (PotionBasketBehaviour potionBasket in potionBaskets)
        {
            potionBasket.PotionBasketManagerBehaviour = this;
        }
    }

    private void Start()
    {
        _currentOrderIndex = GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 0 ? 0 : -1;

        for (int i = 0; i < potionBaskets.Count; i++)
        {
            potionBaskets[i].gameObject.SetActive(false);
            potionBaskets[i].PotionBasketIndex = i;
        }

        ReactivateRightPotionBaskets();
    }
    
    
    public override void IncreaseCurrentSetIndex()
    {
        _currentOrderIndex++;
        
        ReactivateRightPotionBaskets();
    }
    
    public override void DecreaseCurrentSetIndex()
    {
        _currentOrderIndex--;
        if (_currentOrderIndex < 0)
        {
            _currentOrderIndex = GameDontDestroyOnLoadManager.Instance.OrderPotions.Count - 1;
        }
        
        ReactivateRightPotionBaskets();
    }
    
    
    public void ReactivateRightPotionBaskets()
    {
        if (GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 0)
        {
            _currentOrderIndex %= GameDontDestroyOnLoadManager.Instance.OrderPotions.Count;
        }
        
        foreach (PotionBasketBehaviour potionBasket in potionBaskets)
        {
            potionBasket.gameObject.SetActive(false);
        }
        
        clientCanvasGameObject.SetActive(GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 0);

        if (GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 0)
        {
            clientNameText.text = GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentOrderIndex].ClientSo.Name;
            
            for (int i = 0; i < GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentOrderIndex].Potions.Count; i++)
            {
                potionBaskets[i].gameObject.SetActive(true);
                potionBaskets[i].SetBasketContent(_currentOrderIndex);
                potionBaskets[i].DoesNeedToCheckAvailability = true;
            }
        }
    }
    
    
    public void ManageTriggerEnter(PotionBasketBehaviour potionBasket)
    {
        if (_currentTriggeredPotionBaskets.Count == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Add(this);
        }
        
        _currentTriggeredPotionBaskets.Add(potionBasket);
    }
    
    public void ManageTriggerExit(PotionBasketBehaviour potionBasket)
    {
        _currentTriggeredPotionBaskets.Remove(potionBasket);
        
        if (_currentTriggeredPotionBaskets.Count == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Remove(this);
        }
    }
}
