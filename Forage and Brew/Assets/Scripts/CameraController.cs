using System;
using NaughtyAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector]public Camera cam;
    public Transform player;

    [BoxGroup("Calculated at Start")] public Vector3 cameraOffset;
    [BoxGroup("Calculated at Start")] public float targetFocalLength;

    [BoxGroup("Adjustable Variables")] public float distanceFromPlayer;
    [BoxGroup("Adjustable Variables")] [Range(0,1)]public float cameraLerp = 0.07f;
    [BoxGroup("Adjustable Variables")] [Range(0,1)]public float focalLerp = 0.07f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            transform.LookAt(player);
            transform.localPosition = -transform.forward * distanceFromPlayer;
            
        }

    }
    

    [Button]
    public void AdjustCameraPos()
    {
        transform.localPosition = -transform.forward * distanceFromPlayer;
    }

    void Start()
    {
        cam = Camera.main;
        targetFocalLength = cam.focalLength;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.parent.position = Vector3.Lerp(transform.parent.position, player.position+cameraOffset,cameraLerp);
        transform.localPosition =
            Vector3.Lerp(transform.localPosition, -transform.forward * distanceFromPlayer, cameraLerp);
        cam.focalLength = Mathf.Lerp(cam.focalLength,targetFocalLength,focalLerp);

    }
}
