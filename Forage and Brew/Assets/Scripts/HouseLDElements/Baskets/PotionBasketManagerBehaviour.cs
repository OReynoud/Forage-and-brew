using System.Collections.Generic;
using UnityEngine;

public class PotionBasketManagerBehaviour : MonoBehaviour
{
    [SerializeField] private List<PotionBasketBehaviour> potionBaskets;
    
    private int _currentOrderIndex = -1;
    
    private void Start()
    {
        _currentOrderIndex = GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentOrderIndex].Count > 0 ? 0 : -1;

        foreach (PotionBasketBehaviour potionBasket in potionBaskets)
        {
            potionBasket.gameObject.SetActive(false);
        }
        
        if (_currentOrderIndex >= 0)
        {
            for (int i = 0; i < GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentOrderIndex].Count; i++)
            {
                potionBaskets[i].gameObject.SetActive(true);
                potionBaskets[i].SetBasketContent(_currentOrderIndex);
            }
        }
    }
    
    public void IncreaseCurrentOrderIndex()
    {
        _currentOrderIndex++;
        _currentOrderIndex %= GameDontDestroyOnLoadManager.Instance.OrderPotions.Count;
        
        ReactivateRightPotionBaskets();
    }
    
    public void DecreaseCurrentOrderIndex()
    {
        _currentOrderIndex--;
        _currentOrderIndex %= GameDontDestroyOnLoadManager.Instance.OrderPotions.Count;
        
        ReactivateRightPotionBaskets();
    }
    
    private void ReactivateRightPotionBaskets()
    {
        foreach (PotionBasketBehaviour potionBasket in potionBaskets)
        {
            potionBasket.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentOrderIndex].Count; i++)
        {
            potionBaskets[i].gameObject.SetActive(true);
            potionBaskets[i].SetBasketContent(_currentOrderIndex);
        }
    }
}
