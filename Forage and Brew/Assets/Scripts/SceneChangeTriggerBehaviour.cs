using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTriggerBehaviour : MonoBehaviour
{
    [SerializeField] private SceneListSo sceneListSo;
    [SerializeField] private Scene scene;
    [SerializeField] private bool doesNeedToBeDaytime;
    [SerializeField] private bool doesMakeItNighttime;
    
    private void OnTriggerEnter(Collider other)
    {
        if (doesNeedToBeDaytime && CharacterDontDestroyOnLoadManager.Instance.CurrentTimeOfDay != TimeOfDay.Daytime) return;
        
        if (other.CompareTag("Player") && CharacterDontDestroyOnLoadManager.Instance.PreviousScene != scene)
        {
            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Scene == scene)
                {
                    SceneManager.LoadScene(sceneName.Name);
                    break;
                }
            }
            
            if (doesMakeItNighttime)
            {
                CharacterDontDestroyOnLoadManager.Instance.CurrentTimeOfDay = TimeOfDay.Nighttime;
                Debug.Log("It's nighttime now");
            }
        }
    }
}
