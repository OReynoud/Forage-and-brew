using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : Singleton<CameraController>
{
    [HideInInspector]public Camera cam;
    public Transform player;

    [BoxGroup("Calculated at Start")] public float targetFocalLength;
    [BoxGroup("Calculated at Start")] public Vector3 cameraRotation;

    [BoxGroup("Adjustable Variables")] public Vector3 cameraOffset;
    [BoxGroup("Adjustable Variables")] public float distanceFromPlayer;
    [BoxGroup("Adjustable Variables")] [Range(0,1)]public float positionLerp = 0.07f;
    [BoxGroup("Adjustable Variables")] [Range(0,1)]public float rotationLerp = 0.02f;
    [BoxGroup("Adjustable Variables")] [Range(0,1)]public float focalLerp = 0.07f;

    [BoxGroup] [Expandable] public CameraPreset scriptableCamSettings;
    private CameraPreset previousCamSettings;
    private CameraPreset targetCamSettings;

    private float transitionTime = 0.001f;
    private float counter;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            transform.parent.position = player.position + cameraOffset;
            transform.localPosition = -transform.forward * distanceFromPlayer;
            transform.LookAt(transform.parent);
            
        }

    }


    public override void Awake()
    {
        base.Awake();
        
    }


    [Button]
    public void ApplyScriptableCamSettings()
    {
        if (scriptableCamSettings == null)
        {
            Debug.LogError("No Scriptable Cam Settings found!");
            return;
        }
        cam.focalLength = scriptableCamSettings.targetFocalLength;
        targetFocalLength = scriptableCamSettings.targetFocalLength;
        transform.rotation = Quaternion.Euler(scriptableCamSettings.cameraRotation);
        cameraRotation = scriptableCamSettings.cameraRotation;
        cameraOffset = scriptableCamSettings.cameraOffset;

        distanceFromPlayer = scriptableCamSettings.distanceFromPlayer;
        positionLerp = scriptableCamSettings.positionLerp;
        rotationLerp = scriptableCamSettings.rotationLerp;
        focalLerp = scriptableCamSettings.focalLerp;
        
        transform.localPosition = -transform.forward * distanceFromPlayer;
    }
    
    [Button]
    public void SaveCurrentSettingsToCameraPreset()
    {
        if (scriptableCamSettings == null)
        {
            Debug.LogError("No Scriptable Cam Settings found!");
            return;
        }
        scriptableCamSettings.targetFocalLength = cam.focalLength;
        scriptableCamSettings.cameraRotation = transform.rotation.eulerAngles;
        scriptableCamSettings.cameraOffset = cameraOffset;

        scriptableCamSettings.distanceFromPlayer = distanceFromPlayer;
        scriptableCamSettings.positionLerp = positionLerp;
        scriptableCamSettings.rotationLerp = rotationLerp;
        scriptableCamSettings.focalLerp = focalLerp;
        
    }

    public void ApplyScriptableCamSettings(CameraPreset preset, float TransitionTime)
    {
        previousCamSettings = targetCamSettings;
        targetCamSettings = preset;
        transitionTime = TransitionTime == 0 ? 0.001f : TransitionTime;
        counter = 0;
    }

    void Start()
    {
        cam = Camera.main;
        targetFocalLength = cam.focalLength;
        cameraRotation = transform.rotation.eulerAngles;
        previousCamSettings = scriptableCamSettings;
        targetCamSettings = scriptableCamSettings;
        if (scriptableCamSettings == null)
        {
            Debug.LogError("No Scriptable Cam Settings found, camera might work unpredictably");
            return;
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.parent.position = Vector3.Lerp(transform.parent.position, player.position+cameraOffset,positionLerp);
        transform.localPosition =
            Vector3.Lerp(transform.localPosition, -transform.forward * distanceFromPlayer, positionLerp);
        cam.focalLength = Mathf.Lerp(cam.focalLength,targetFocalLength,focalLerp);
        
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cameraRotation),positionLerp);
    }

    private void Update()
    {
        if (counter < transitionTime)
        {
            counter += Time.deltaTime;
        }
        
        
        cameraRotation = Vector3.Lerp(previousCamSettings.cameraRotation,targetCamSettings.cameraRotation,counter / transitionTime);
        cameraOffset = Vector3.Lerp(previousCamSettings.cameraOffset,targetCamSettings.cameraOffset,counter / transitionTime);

        targetFocalLength = Mathf.Lerp(previousCamSettings.targetFocalLength,targetCamSettings.targetFocalLength, counter / transitionTime);
        distanceFromPlayer = Mathf.Lerp(previousCamSettings.distanceFromPlayer,targetCamSettings.distanceFromPlayer, counter / transitionTime);
        positionLerp = Mathf.Lerp(previousCamSettings.positionLerp,targetCamSettings.positionLerp, counter / transitionTime);
        rotationLerp = Mathf.Lerp(previousCamSettings.rotationLerp,targetCamSettings.rotationLerp, counter / transitionTime);
        focalLerp = Mathf.Lerp(previousCamSettings.focalLerp,targetCamSettings.focalLerp, counter / transitionTime);
    }
}
