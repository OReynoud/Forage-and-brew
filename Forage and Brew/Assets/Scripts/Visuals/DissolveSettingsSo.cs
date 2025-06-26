using UnityEngine;

[CreateAssetMenu(fileName = "D_DissolveSettings", menuName = "Visuals/DissolveSettingsSo")]
public class DissolveSettingsSo : ScriptableObject
{
    [field: SerializeField] public float AnimationDuration { get; private set; } = 0.5f;
    [field: SerializeField] public AnimationCurve AnimationCurve { get; private set; } = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
}
