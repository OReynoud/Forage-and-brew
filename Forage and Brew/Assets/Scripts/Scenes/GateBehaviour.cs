using UnityEngine;

public class GateBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject openedGateGameObject;
    [SerializeField] private GameObject closedGateGameObject;
    [SerializeField] private bool doesLockOnDay6;
    
    private void Start()
    {
        if (GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay == TimeOfDay.Nighttime ||
            doesLockOnDay6 && GameDontDestroyOnLoadManager.Instance.DayPassed == 5)
        {
            CloseGate();
        }
        else
        {
            OpenGate();
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
