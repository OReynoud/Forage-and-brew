using UnityEngine;

[CreateAssetMenu(fileName = "D_LunarCycleState", menuName = "Lunar Cycle/LunarCycleStateSo")]
public class LunarCycleStateSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Color Color { get; private set; }
}
