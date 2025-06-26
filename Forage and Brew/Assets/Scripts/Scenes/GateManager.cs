using UnityEngine;
using UnityEngine.Events;

public class GateManager : MonoBehaviour
{
    public static GateManager Instance { get; private set; }
    
    public UnityEvent OnAreaPurchased { get; private set; } = new();
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
