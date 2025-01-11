using UnityEngine;

public class GateBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject openedGateGameObject;
    [SerializeField] private GameObject closedGateGameObject;
    
    private void Start()
    {
        if (GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay == TimeOfDay.Daytime)
        {
            OpenGate();
        }
        else
        {
            CloseGate();
        }
    }

    
    private void OpenGate()
    {
        openedGateGameObject.SetActive(true);
        closedGateGameObject.SetActive(false);
    }
    
    private void CloseGate()
    {
        openedGateGameObject.SetActive(false);
        closedGateGameObject.SetActive(true);
    }
}
