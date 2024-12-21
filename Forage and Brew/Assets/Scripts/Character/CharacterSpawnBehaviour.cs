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
        if (sourceScene == GameDontDestroyOnLoadManager.Instance.PreviousScene)
        {
            characterTransform.position = transform.position;
            characterTransform.rotation = transform.rotation;
            
            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Name == SceneManager.GetActiveScene().name)
                {
                    GameDontDestroyOnLoadManager.Instance.PreviousScene = sceneName.Scene;
                    CharacterVfxManager.Instance.CheckForRainVfx(sceneName.Scene);
                    WeatherLightingManager.Instance?.SetRightLighting(sceneName.Scene);
                    break;
                }
            }
            
            PinnedRecipe.instance.Start();

            if (WeatherManager.Instance.CurrentWeatherStates.Count != 0)
            {
                InfoDisplayManager.instance.DisplayWeather();
            }
            
            if (camSettings != null)
            {
                CameraController.instance.scriptableCamSettings = camSettings;
                CameraController.instance.ApplyScriptableCamSettings();
                CameraController.instance.ApplyScriptableCamSettings(camSettings, 0);
                CameraController.instance.InstantCamUpdate();
            }
        }
    }
}
