using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawnBehaviour : MonoBehaviour
{
    [SerializeField] private SceneListSo sceneListSo;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Scene sourceScene;

    
    private void Start()
    {
        if (sourceScene == GameDontDestroyOnLoadManager.Instance.PreviousScene)
        {
            characterTransform.position = transform.position;
            characterTransform.rotation = transform.rotation;

            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Name == SceneManager.GetActiveScene().name)
                {
                    GameDontDestroyOnLoadManager.Instance.PreviousScene = sceneName.Scene;
                    break;
                }
            }
        }
    }
}
