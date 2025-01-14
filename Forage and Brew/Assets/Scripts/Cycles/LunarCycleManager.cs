using UnityEngine;

public class LunarCycleManager : MonoBehaviour
{
    // Singleton
    public static LunarCycleManager Instance { get; private set; }
    
    [SerializeField] private LunarCycleStateOrderSo lunarCycleStateOrderSo;
    [SerializeField] private int startingLunarCycleStateIndex;
    
    public int CurrentLunarCycleStateIndex { get; set; }
    public LunarCycleStateSo CurrentLunarCycleState => lunarCycleStateOrderSo.LunarCycleStates[CurrentLunarCycleStateIndex];
    

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
        if (GameDontDestroyOnLoadManager.Instance.IsFirstGameSession)
        {
            CurrentLunarCycleStateIndex = startingLunarCycleStateIndex;
        }
        Debug.Log("The current lunar cycle state is: " + lunarCycleStateOrderSo.LunarCycleStates[CurrentLunarCycleStateIndex].Name);
        InfoDisplayManager.instance.DisplayMoonCycles();
    }
    
    
    public void PassToNextLunarCycleState()
    {
        CurrentLunarCycleStateIndex++;
        if (CurrentLunarCycleStateIndex >= lunarCycleStateOrderSo.LunarCycleStates.Length)
        {
            CurrentLunarCycleStateIndex = 0;
        }
        
        Debug.Log("The current lunar cycle state is: " + lunarCycleStateOrderSo.LunarCycleStates[CurrentLunarCycleStateIndex].Name);
        
        InfoDisplayManager.instance.DisplayMoonCycles();
    }
}
