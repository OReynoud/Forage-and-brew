using NaughtyAttributes;
using UnityEngine;

public class AnimationCurveInverterBehaviour : MonoBehaviour
{
    [SerializeField] private AnimationCurve animationCurve;

    [Button]
    private void InvertAnimationCurveHorizontally()
    {
        var invertedAnimationCurve = new AnimationCurve();
        for (var i = 0; i < animationCurve.keys.Length; i++)
        {
            var key = animationCurve.keys[animationCurve.keys.Length - i - 1];
            invertedAnimationCurve.AddKey(new Keyframe(1 - key.time, key.value, -key.outTangent, -key.inTangent));
        }

        animationCurve = invertedAnimationCurve;
    }

    [Button]
    private void InvertAnimationCurveVertically()
    {
        var invertedAnimationCurve = new AnimationCurve();
        for (var i = 0; i < animationCurve.keys.Length; i++)
        {
            var key = animationCurve.keys[i];
            invertedAnimationCurve.AddKey(new Keyframe(key.time, 1f - key.value, -key.inTangent, -key.outTangent));
        }
        animationCurve = invertedAnimationCurve;
    }
}
