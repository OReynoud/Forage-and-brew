using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawnBehaviour : MonoBehaviour
{
    [SerializeField] private SceneListSo sceneListSo;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Scene sourceScene;
    [SerializeField] private CameraPreset camSettings;

    private void Start()
    {
        if (sourceScene == GameDontDestroyOnLoadManager.Instance.CurrentScene)
        {
            characterTransform.position = transform.position;
            characterTransform.rotation = transform.rotation;
            
            
            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Name == SceneManager.GetActiveScene().name)
                {
                    CharacterVfxManager.Instance.CheckForRainVfx(sceneName.Scene);
                    WeatherLightingManager.Instance?.SetRightLighting(sceneName.Scene);
                    
                    if (sourceScene == Scene.House && sceneName.Scene == Scene.House)
                    {
                        SceneTransitionManager.instance.Wake();
                    }
                    else
                    {
                        SceneTransitionManager.instance.HandleLoadNewScene(sceneName.Scene);
                    }
                    break;
                }
            }
            
            PinnedRecipe.instance.Start();
            
            if (camSettings != null)
            {
                SimpleCameraBehavior.instance.ApplyScriptableCamSettings(camSettings, 0);
                SimpleCameraBehavior.instance.InstantCamUpdate();
            }


        }
    }
}
