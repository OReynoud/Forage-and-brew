using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawnBehaviour : MonoBehaviour
{
    [SerializeField] private SceneListSo sceneListSo;
    [SerializeField] private Scene sourceScene;

    private void Start()
    {
        if (sourceScene == CharacterDontDestroyOnLoadManager.Instance.PreviousScene)
        {
            CharacterDontDestroyOnLoadManager.Instance.transform.position = transform.position;
            CharacterDontDestroyOnLoadManager.Instance.transform.rotation = transform.rotation;

            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Name == SceneManager.GetActiveScene().name)
                {
                    CharacterDontDestroyOnLoadManager.Instance.PreviousScene = sceneName.Scene;
                    break;
                }
            }
        }
    }
}
