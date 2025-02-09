using NaughtyAttributes;
using UnityEngine;

public class IndoorsCameraBehavior : SimpleCameraBehavior
{
    
    [BoxGroup("New settings")]public Vector3 aimedCamPos;
    [BoxGroup("New settings")]public Vector3 aimedCamRotation;
    [BoxGroup("New settings")]public Vector3 targetOffset;
    [BoxGroup("New settings")]public bool posClampX;
    [BoxGroup("New settings")]public bool posClampY;
    [BoxGroup("New settings")]public bool posClampZ;
    [BoxGroup("New settings")]public bool rotationClampX;
    [BoxGroup("New settings")]public bool rotationClampY;
    [BoxGroup("New settings")]public bool rotationClampZ;
    
    
    public override void Awake()
    {
        base.Awake();

        cam = Camera.main;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void Update()
    {
        
    }


    public override void FixedUpdate()
    {
     
        transform.parent.position = Vector3.Lerp(transform.parent.position, player.position + cameraOffset,
            movement.isRunning ? positionLerp * 2.5f : positionLerp);
        Quaternion rotation = Quaternion.LookRotation((player.position - transform.position) + targetOffset, Vector3.up);
        transform.localRotation = rotation;
        
        // if (fixedPos && !fixedRotation)
        // {
        //     Quaternion rotation = Quaternion.LookRotation((player.position - transform.position) + Vector3.up, Vector3.up);
        //     transform.localRotation =
        //         Quaternion.Lerp(transform.localRotation, rotation, rotationLerp * (counter / transitionTime));
        //     
        // }
        // else
        // {
        //     cam.focalLength = Mathf.Lerp(cam.focalLength, targetFocalLength, focalLerp);
        //     overlayUiCam.focalLength = Mathf.Lerp(overlayUiCam.focalLength, targetFocalLength, focalLerp);
        //     transform.localRotation =
        //         Quaternion.Lerp(transform.localRotation, Quaternion.Euler(cameraRotation), rotationLerp);
        // }

    }

    public override void InstantCamUpdate()
    {
        
    }

    public override void ApplyScriptableCamSettings(CameraPreset preset, float TransitionTime)
    {
        
    }
}
