using UnityEngine;

public class AudioListenerBehaviour : MonoBehaviour
{
    [SerializeField] private SimpleCameraBehavior simpleCameraBehavior;
    [SerializeField] private Camera mainCamera;
    

    private void LateUpdate()
    {
        transform.position = simpleCameraBehavior.player.transform.position;
        transform.rotation = mainCamera.transform.rotation;
    }
}
