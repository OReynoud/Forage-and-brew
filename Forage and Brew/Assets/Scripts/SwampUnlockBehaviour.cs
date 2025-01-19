using UnityEngine;

public class SwampUnlockBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject beforeUnlockGameObject;
    [SerializeField] private GameObject afterUnlockGameObject;
    [SerializeField] private int unlockDay = 6;


    private void Start()
    {
        beforeUnlockGameObject.SetActive(GameDontDestroyOnLoadManager.Instance.DayPassed < unlockDay - 1);
        afterUnlockGameObject.SetActive(GameDontDestroyOnLoadManager.Instance.DayPassed >= unlockDay - 1);
    }
}
