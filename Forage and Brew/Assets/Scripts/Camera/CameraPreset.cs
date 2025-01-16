using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraPresets", menuName = "Scriptable Objects/CameraPreset")]
public class CameraPreset : ScriptableObject
{
    public Vector3 cameraOffset;
    public Vector3 cameraRotation;
    public float targetFocalLength;

    public float distanceFromPlayer;
    [Range(0,1)]public float positionLerp = 0.07f;
    [Range(0,1)]public float rotationLerp = 0.02f;
    [Range(0,1)]public float focalLerp = 0.07f;
    
    [BoxGroup("Adjustable Variables")] public Vector3 posMaxClamp = new Vector3(0,0,100);
    [BoxGroup("Adjustable Variables")] public Vector3 posMinClamp = new Vector3(0,0,-100);
}
