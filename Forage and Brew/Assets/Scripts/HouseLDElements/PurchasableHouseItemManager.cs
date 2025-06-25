using UnityEngine;
using UnityEngine.Events;

public class PurchasableHouseItemManager : MonoBehaviour
{
    public static PurchasableHouseItemManager Instance { get; private set; }
    
    public UnityEvent OnItemPurchased { get; private set; } = new();
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
