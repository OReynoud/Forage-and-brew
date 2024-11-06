using UnityEngine;

[CreateAssetMenu(fileName = "D_LunarCycleStateOrder", menuName = "Lunar Cycle/LunarCycleStateOrderSo")]
public class LunarCycleStateOrderSo : ScriptableObject
{
    [field: SerializeField] public LunarCycleStateSo[] LunarCycleStates { get; private set; }
}
