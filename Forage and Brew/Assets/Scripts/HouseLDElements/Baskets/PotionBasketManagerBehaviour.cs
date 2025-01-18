using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PotionBasketManagerBehaviour : BasketManagerBehaviour
{
    [SerializeField] private List<PotionBasketBehaviour> potionBaskets;
    [SerializeField] private float enableDisableTime = 0.5f;
    
    [Header("UI")]
    [SerializeField] private GameObject clientCanvasGameObject;
    [SerializeField] private GameObject localCanvasGameObject;
    [SerializeField] private TMP_Text clientNameText;
    [SerializeField] private GameObject clientCheckmarkGameObject;
    
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
        localCanvasGameObject.SetActive(false);
        
        _currentOrderIndex = GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 0 ? 0 : -1;

        for (int i = 0; i < potionBaskets.Count; i++)
        {
            potionBaskets[i].PotionBasketIndex = i;
        }

        ReactivateRightPotionBaskets();
    }
    
    
    public override void IncreaseCurrentSetIndex()
    {
        if (_currentOrderIndex == 0 && GameDontDestroyOnLoadManager.Instance.OrderPotions.Count == 1) return;
        
        _currentOrderIndex++;
        
        ReactivateRightPotionBaskets();
    }
    
    public override void DecreaseCurrentSetIndex()
    {
        if (_currentOrderIndex == 0 && GameDontDestroyOnLoadManager.Instance.OrderPotions.Count == 1) return;
        
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
        
        int previousActivePotionBasketsCount = 0;
        
        foreach (PotionBasketBehaviour potionBasket in potionBaskets)
        {
            potionBasket.StartDisable(enableDisableTime);
            
            if (potionBasket.enabled)
            {
                previousActivePotionBasketsCount++;
            }
        }
        
        clientCanvasGameObject.SetActive(GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 0);
        clientCheckmarkGameObject.SetActive(GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 0 &&
                                            GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentOrderIndex]
                                                .Potions.All(potion => potion));

        if (GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 0)
        {
            clientNameText.text = GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentOrderIndex].ClientSo.Name;
            
            for (int i = 0; i < potionBaskets.Count; i++)
            {
                if (i < GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentOrderIndex].Potions.Count)
                {
                    potionBaskets[i].StartEnable(enableDisableTime);
                    potionBaskets[i].SetBasketContent(_currentOrderIndex);
                    potionBaskets[i].DoesNeedToCheckAvailability = true;
                    potionBaskets[i].BasketVfxManager.PlaySmokescreen();
                }
                else
                {
                    if (i < previousActivePotionBasketsCount)
                    {
                        potionBaskets[i].BasketVfxManager.PlaySmokescreen();
                    }
                }
            }
        }
    }
    
    public void CheckCompletion()
    {
        clientCheckmarkGameObject.SetActive(GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 0 &&
                                            GameDontDestroyOnLoadManager.Instance.OrderPotions[_currentOrderIndex]
                                                .Potions.All(potion => potion));
    }
    
    
    public void ManageTriggerEnter(PotionBasketBehaviour potionBasket)
    {
        if (_currentTriggeredPotionBaskets.Count == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Add(this);
            
            if (GameDontDestroyOnLoadManager.Instance.OrderPotions.Count > 1)
            {
                localCanvasGameObject.SetActive(true);
            }
        }
        
        _currentTriggeredPotionBaskets.Add(potionBasket);
    }
    
    public void ManageTriggerExit(PotionBasketBehaviour potionBasket)
    {
        _currentTriggeredPotionBaskets.Remove(potionBasket);
        
        if (_currentTriggeredPotionBaskets.Count == 0)
        {
            BasketInputManager.Instance.CurrentBasketManagers.Remove(this);
            localCanvasGameObject.SetActive(false);
        }
    }
}
