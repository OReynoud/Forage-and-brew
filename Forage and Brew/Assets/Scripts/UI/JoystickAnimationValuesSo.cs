using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "D_JoystickAnimationValues", menuName = "UI/JoystickAnimationValuesSo")]
public class JoystickAnimationValuesSo : ScriptableObject
{
    [field: SerializeField] public bool IsDrivenExternally { get; private set; }
    [field: SerializeField] public JoystickDirection JoystickDirection { get; private set; }
    [field: SerializeField] [field: Min(0f)] public float AnimationDuration { get; private set; } = 0.4f;
    [field: SerializeField] [field: Min(0f)] public float AnimationDistance { get; private set; } = 100f;
    [field: HideIf("IsDrivenExternally")] [field: SerializeField] [field: Min(0f)] public float AnimationDelay { get; private set; } = 0.1f;
    
    [field: ShowIf("JoystickDirection", JoystickDirection.ArcOfACircleDownCounterClockwise)]
    [field: SerializeField] public float AnimationRotationAngle { get; private set; }
    [field: ShowIf("JoystickDirection", JoystickDirection.ArcOfACircleDownCounterClockwise)]
    [field: SerializeField] [field: Min(0f)] public float AnimationRotationAngleTolerance { get; private set; }
    
    [field: SerializeField] public AnimationCurve AnimationCurve { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
