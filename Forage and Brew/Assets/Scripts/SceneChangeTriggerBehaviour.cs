using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTriggerBehaviour : MonoBehaviour
{
    [SerializeField] private SceneListSo sceneListSo;
    [SerializeField] private Scene scene;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Scene == scene)
                {
                    SceneManager.LoadScene(sceneName.Name);
                    break;
                }
            }
        }
    }
}
