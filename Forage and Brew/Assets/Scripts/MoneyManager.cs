using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    // Singleton
    public static MoneyManager Instance { get; private set; }
    
    public int MoneyAmount { get; private set; }
    
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }
    
    private void Start()
    {
        InfoDisplayManager.instance.DisplayMoney();
    }
    
    
    public void AddMoney(int amount)
    {
        MoneyAmount += amount;
        InfoDisplayManager.instance.DisplayMoney();
    }
    
    public void SubtractMoney(int amount)
    {
        MoneyAmount -= amount;
        InfoDisplayManager.instance.DisplayMoney();
    }
}
