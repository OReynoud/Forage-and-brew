using Unity.VisualScripting;
using UnityEngine;

public class HouseCameraSettingsBehavior : MonoBehaviour
{
    public HouseCameraSetting settings;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, settings.triggerDistance);
    } 
}
