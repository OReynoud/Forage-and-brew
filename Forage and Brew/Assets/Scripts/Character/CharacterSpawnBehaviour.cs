using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CharacterSpawnBehaviour : MonoBehaviour
{
    [SerializeField] private SceneListSo sceneListSo;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Scene sourceScene;
    [SerializeField] private CameraPreset camSettings;

    private void Awake()
    {

    }

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
                    break;
                }
            }
            if (camSettings != null)
            {
                CameraController.instance.scriptableCamSettings = camSettings;
                CameraController.instance.ApplyScriptableCamSettings();
                CameraController.instance.ApplyScriptableCamSettings(camSettings, 0);
                CameraController.instance.InstantCamUpdate();
                CameraController.instance.InstantCamUpdate();
            }
        }
    }
}
