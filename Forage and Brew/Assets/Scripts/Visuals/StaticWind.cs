using UnityEngine;

public class StaticWind : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
