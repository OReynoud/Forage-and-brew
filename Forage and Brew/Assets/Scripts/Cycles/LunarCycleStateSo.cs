using UnityEngine;

[CreateAssetMenu(fileName = "D_LunarCycleState", menuName = "Lunar Cycle/LunarCycleStateSo")]
public class LunarCycleStateSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int Index { get; private set; }
    // 0 = FullMoon ; 1 = HalfMoon ; 2 = NewMoon ; 3 = ShootingStars
}
