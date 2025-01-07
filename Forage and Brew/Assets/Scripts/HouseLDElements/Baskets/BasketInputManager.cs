using System.Collections.Generic;
using UnityEngine;

public class BasketInputManager : MonoBehaviour
{
    // Singleton
    public static BasketInputManager Instance { get; private set; }
    
    public List<BasketManagerBehaviour> CurrentBasketManagers { get; private set; } = new();
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    
    
    public void PreviousBasketSet()
    {
        foreach (BasketManagerBehaviour basketManager in CurrentBasketManagers.ToArray())
        {
            basketManager.DecreaseCurrentSetIndex();
        }
    }
    
    public void NextBasketSet()
    {
        foreach (BasketManagerBehaviour basketManager in CurrentBasketManagers.ToArray())
        {
            basketManager.IncreaseCurrentSetIndex();
        }
    }
}
