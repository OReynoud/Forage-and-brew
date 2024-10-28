using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 cameraOffset;
    public Camera cam;

    public float targetFocalLength;

    [Range(0,1)]public float cameraLerp = 0.07f;
    [Range(0,1)]public float focalLerp = 0.07f;

    public Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetFocalLength = cam.focalLength;
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + cameraOffset,cameraLerp);
        cam.focalLength = Mathf.Lerp(cam.focalLength,targetFocalLength,focalLerp);
    }
}
