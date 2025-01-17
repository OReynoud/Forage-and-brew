using UnityEngine;

public class LocalLightManager : MonoBehaviour
{
    
    [SerializeField] private GameObject sunriseVfxGameObject;
    [SerializeField] private GameObject sunsetVfxGameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sunriseVfxGameObject.SetActive(false);
        sunsetVfxGameObject.SetActive(false);
        if (GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay == TimeOfDay.Daytime)
        {
            sunriseVfxGameObject.SetActive(true);
        }
        else
        {
            sunsetVfxGameObject.SetActive(true);
        }
    }
}
