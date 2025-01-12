using UnityEngine;

public class LunarCycleManager : MonoBehaviour
{
    // Singleton
    public static LunarCycleManager Instance { get; private set; }
    
    [SerializeField] private LunarCycleStateOrderSo lunarCycleStateOrderSo;
    [SerializeField] private int startingLunarCycleStateIndex;
    
    private int _currentLunarCycleStateIndex;
    public LunarCycleStateSo CurrentLunarCycleState => lunarCycleStateOrderSo.LunarCycleStates[_currentLunarCycleStateIndex];
    

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
        _currentLunarCycleStateIndex = startingLunarCycleStateIndex;
        Debug.Log("The current lunar cycle state is: " + lunarCycleStateOrderSo.LunarCycleStates[_currentLunarCycleStateIndex].Name);
        InfoDisplayManager.instance.DisplayMoonCycles();
    }
    
    
    public void PassToNextLunarCycleState()
    {
        _currentLunarCycleStateIndex++;
        if (_currentLunarCycleStateIndex >= lunarCycleStateOrderSo.LunarCycleStates.Length)
        {
            _currentLunarCycleStateIndex = 0;
        }
        
        Debug.Log("The current lunar cycle state is: " + lunarCycleStateOrderSo.LunarCycleStates[_currentLunarCycleStateIndex].Name);
        
        InfoDisplayManager.instance.DisplayMoonCycles();
    }
}
