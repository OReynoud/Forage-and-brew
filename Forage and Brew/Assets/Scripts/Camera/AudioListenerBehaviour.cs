using UnityEngine;

public class AudioListenerBehaviour : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Camera mainCamera;
    

    private void LateUpdate()
    {
        transform.position = cameraController.player.transform.position;
        transform.rotation = mainCamera.transform.rotation;
    }
}
