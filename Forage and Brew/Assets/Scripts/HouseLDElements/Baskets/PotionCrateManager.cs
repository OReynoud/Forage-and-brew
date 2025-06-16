using System.Collections.Generic;
using UnityEngine;

public class PotionCrateManager : MonoBehaviour
{
    public static PotionCrateManager Instance { get; private set; }
    
    [field: SerializeField] public List<PotionCrateBehaviour> PotionCrates { get; private set; } = new();

    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
        
        foreach (PotionCrateBehaviour potionBasket in PotionCrates)
        {
            potionBasket.PotionCrateManager = this;
        }
    }

    private void Start()
    {
        ReactivateRightPotionCrates();
    }
    
    
    public void ReactivateRightPotionCrates()
    {
        for (int i = 0; i < PotionCrates.Count; i++)
        {
            if (OrderManager.Instance.CurrentOrders[i] != null)
            {
                PotionCrates[i].EnableCrate(
                    OrderManager.Instance.CurrentOrders[i].OrderContent,
                    OrderManager.Instance.CurrentOrders[i].RelatedLetter.Client);
            }
            else
            {
                PotionCrates[i].DisableCrate();
            }
        }
    }
}
