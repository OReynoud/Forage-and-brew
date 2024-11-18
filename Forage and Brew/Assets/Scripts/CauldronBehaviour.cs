using UnityEngine;

public class CauldronBehaviour : MonoBehaviour
{
    [SerializeField] private CameraPreset cauldronCameraPreset;
    [SerializeField] private float cauldronCameraTransitionTime = 0.5f;
    private CameraPreset _previousCameraPreset;
    
    [SerializeField] private GameObject interactInputCanvasGameObject;


    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
    }


    private void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            _previousCameraPreset = CameraController.instance.TargetCamSettings;
            CameraController.instance.ApplyScriptableCamSettings(cauldronCameraPreset, cauldronCameraTransitionTime);
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            CameraController.instance.ApplyScriptableCamSettings(_previousCameraPreset, cauldronCameraTransitionTime);
            DisableInteract();
        }
    }
}
