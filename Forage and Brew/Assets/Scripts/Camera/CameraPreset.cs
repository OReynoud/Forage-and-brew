using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraPresets", menuName = "Scriptable Objects/CameraPreset")]
public class CameraPreset : ScriptableObject
{
    public Color groupColor = Color.white;
    public bool isFixedCameraPos;
    [ShowIf("isFixedCameraPos")] public bool isFixedCameraRotation = true;
    public Vector3 cameraOffset;
    public Vector3 cameraRotation;
    public float targetFocalLength;

    public float distanceFromPlayer;
    [Range(0,1)]public float positionLerp = 0.07f;
    [Range(0,1)]public float rotationLerp = 0.02f;
    [Range(0,1)]public float focalLerp = 0.07f;
    
    [BoxGroup("Adjustable Variables")] public Vector3 posMaxClamp = new(0,0,100);
    [BoxGroup("Adjustable Variables")] public Vector3 posMinClamp = new(0,0,-100);
    [BoxGroup("Adjustable Variables")] public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0,0,1,1);
}
