using UnityEditor;
using UnityEngine;

public class HouseCameraSettingsBehavior : MonoBehaviour
{
    public HouseCameraSetting settings;
    public SphereCollider collider;
    

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.color = settings.cameraPreset.groupColor;
        Handles.DrawWireDisc(transform.position, Vector3.up, settings.triggerDistance);
    }
    
    #endif

    void OnDrawGizmosSelected()
    {
        collider.radius = settings.triggerDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        HouseCameraBehavior.cameraTriggerBehavior.Invoke(true, this);
    }

    private void OnTriggerExit(Collider other)
    {
        HouseCameraBehavior.cameraTriggerBehavior.Invoke(false, this);
    }
}
