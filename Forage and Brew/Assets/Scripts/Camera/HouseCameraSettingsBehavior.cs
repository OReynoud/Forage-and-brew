using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HouseCameraSettingsBehavior : MonoBehaviour
{
    public HouseCameraSetting settings;

    void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, settings.triggerDistance);
        //Gizmos.DrawWireSphere(transform.position, settings.triggerDistance);
    } 
}
