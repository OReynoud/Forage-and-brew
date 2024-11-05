using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawnBehaviour : MonoBehaviour
{
    [SerializeField] private SceneListSo sceneListSo;
    [SerializeField] private Scene sourceScene;

    private void Start()
    {
        if (sourceScene == CharacterDontDestroyOnLoadBehaviour.Instance.PreviousScene)
        {
            CharacterDontDestroyOnLoadBehaviour.Instance.transform.position = transform.position;
            CharacterDontDestroyOnLoadBehaviour.Instance.transform.rotation = transform.rotation;

            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Name == SceneManager.GetActiveScene().name)
                {
                    CharacterDontDestroyOnLoadBehaviour.Instance.PreviousScene = sceneName.Scene;
                    break;
                }
            }
        }
    }
}
