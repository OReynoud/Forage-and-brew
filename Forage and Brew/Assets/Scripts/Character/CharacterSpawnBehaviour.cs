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
        if (SceneTransitionManager.instance.debugSleep) return;
        if (sourceScene == GameDontDestroyOnLoadManager.Instance.CurrentScene)
        {
            characterTransform.position = transform.position;
            characterTransform.rotation = transform.rotation;
            
            
            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Name == SceneManager.GetActiveScene().name)
                {
                    CharacterVfxManager.Instance.CheckForRainVfx();
                    WeatherLightingManager.Instance?.SetRightLighting();
                    
                    if (sourceScene == Scene.HouseOutdoor && sceneName.Scene == Scene.HouseOutdoor)
                    {
                        Debug.Log(gameObject,gameObject);
                        SceneTransitionManager.instance.Wake();
                        PinnedRecipe.instance.isInHouse = true;
                    }
                    else
                    {
                        SceneTransitionManager.instance.HandleLoadNewScene(sceneName.Scene);
                        PinnedRecipe.instance.isInHouse = false;
                    }
                    
                    MusicManager.Instance.PlaySceneMucic(sceneName.Scene);
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
