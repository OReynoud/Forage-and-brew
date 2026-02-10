using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    private static readonly int DoSleep = Animator.StringToHash("DoSleep");
    private static readonly int DoWakeUp = Animator.StringToHash("DoWakeUp");
    [SerializeField] private SceneListSo sceneListSo;
    public float transitionTime;
    public float timer;
    public float transitionFrameCount;
    public float frameCounter;
    public float sleepWaitTime;
    public Vector3 sleepPos;
    public Vector3 sleepRotation;
    public CameraPreset sleepCam;

    public Vector2 fullyExtendedDimensions;
    public Vector2 focusedDimensions;
    public GameObject transitionElement;

    public RectTransform maskElement;

    public bool debugSleep;
    public UnityEvent OnSleep { get; set; } = new();
    
    private Coroutine _coroutine;

    private bool showScreenBehavior;
    private Scene NewScene;

    private void Start()
    {
        if (debugSleep)
        {
            HandleGoingToSleepTransition(transform);
        }

        SceneManager.sceneUnloaded += SceneManagerOnsceneLoaded;
        if (sceneListSo.SceneNames[0].Name != SceneManager.GetActiveScene().name)
        {
            SceneManagerOnsceneLoaded(SceneManager.GetActiveScene());
        }
        //SceneManager.sceneUnloaded += SceneManagerOnsceneUnloaded;
    }

    // private void SceneManagerOnsceneUnloaded(UnityEngine.SceneManagement.Scene arg0)
    // {
    //     Debug.Log("Unloaded a scene");
    // }

    private void SceneManagerOnsceneLoaded(UnityEngine.SceneManagement.Scene arg0)
    {
        //showScreenBehavior = true;
        DOTween.To(() => maskElement.sizeDelta, x => maskElement.sizeDelta = x, fullyExtendedDimensions,
            transitionTime).OnComplete((() =>
        {
            showScreenBehavior = false;
            frameCounter = 0;        
            transitionElement.gameObject.SetActive(false);
        
            GameDontDestroyOnLoadManager.Instance.CurrentScene = NewScene;
            if (WeatherManager.Instance.CurrentWeatherState != null)
            {
                InfoDisplayManager.instance.DisplayWeather();
            }
        
            if (GameDontDestroyOnLoadManager.Instance.CurrentScene is Scene.Biome1 or Scene.Biome2)
            {
                AddBiomeListeners();
            }
            else
            {
                AddHouseListeners();
            }
        
        
            CharacterMovementController.Instance.SetupAudio(GameDontDestroyOnLoadManager.Instance.CurrentScene);
            CharacterInputManager.Instance.EnableInputs();
        }));
    }



    public void FixedUpdate()
    {
        if (!showScreenBehavior) return;
        frameCounter++;
        maskElement.sizeDelta = Vector2.Lerp(Vector2.zero, fullyExtendedDimensions, frameCounter/transitionFrameCount);
        if (frameCounter > transitionFrameCount)
        {
            frameCounter = 0;
            showScreenBehavior = false;
            transitionElement.gameObject.SetActive(false);
        }

    }

    public void HandleSceneChange(string SceneName)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ChangeScenes(SceneName));
    }

    public void HandleLoadNewScene(Scene newScene)
    {
        transitionElement.gameObject.SetActive(false);
        timer = 0;
        transitionElement.gameObject.SetActive(true);
        maskElement.sizeDelta = Vector2.zero;
        // if (_coroutine != null)
        //     StopCoroutine(_coroutine);
        // _coroutine = StartCoroutine(ShowScreen(newScene));
        
        NewScene = newScene;

    }

    public void HandleGoingToSleepTransition(Transform spawnPoint)
    {
        if (!debugSleep)
        {
            if (OnSleep != null)
                OnSleep.Invoke();
            StartCoroutine(HideScreen(spawnPoint));
        }
        else
        {
            StartCoroutine(HideScreen(spawnPoint));
        }
    }

    private IEnumerator ChangeScenes(string sceneName)
    {
        CharacterInputManager.Instance.DisableInputs();
        timer = 0;
        transitionElement.gameObject.SetActive(true);
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(fullyExtendedDimensions, Vector2.zero, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        SceneManager.LoadScene(sceneName);
    }
    
    private IEnumerator HideScreen(Transform spawnPoint)
    {
        CharacterInputManager.Instance.DisableInputs();
        timer = 0;
        transitionElement.gameObject.SetActive(true);
        
        CharacterMovementController.Instance.rb.constraints = RigidbodyConstraints.FreezeAll;
        CharacterMovementController.Instance.transform.GetComponent<Collider>().enabled = false;
        
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(fullyExtendedDimensions, Vector2.zero, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        var camSettings = SimpleCameraBehavior.instance.TargetCamSettings;
        ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = true;
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(sleepCam,0);
        SimpleCameraBehavior.instance.InstantCamUpdate(sleepCam);
        
        CharacterAnimManager.instance.animator.SetTrigger(DoSleep);
        CharacterAnimManager.instance.PlayPurrSound();
        CharacterAnimManager.instance.transform.position = sleepPos;
        CharacterAnimManager.instance.transform.rotation = Quaternion.Euler(sleepRotation);
        yield return new WaitForSecondsRealtime(0.4f);
        timer = 0;
        while (timer < 0.2f)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(Vector2.zero, focusedDimensions, timer/0.2f);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        
        
        yield return new WaitForSecondsRealtime(sleepWaitTime);
        
        
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(focusedDimensions, Vector2.zero, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(camSettings,0);
        SimpleCameraBehavior.instance.InstantCamUpdate(camSettings);
        ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = false;
        CharacterAnimManager.instance.transform.position = spawnPoint.position;
        CharacterAnimManager.instance.transform.rotation = spawnPoint.rotation;
        CharacterMovementController.Instance.rb.constraints = RigidbodyConstraints.FreezeRotation;
        CharacterMovementController.Instance.transform.GetComponent<Collider>().enabled = true;
        yield return new WaitForSecondsRealtime(0.5f);
        
        
        GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay = TimeOfDay.Daytime;
        GameDontDestroyOnLoadManager.Instance.DayPassed++;
        InfoDisplayManager.instance.DisplayDays();
        // Cycles
        WeatherManager.Instance.PassToNextWeatherState();
        LunarCycleManager.Instance.PassToNextLunarCycleState();
        
        WeatherLightingManager.Instance.SetRightLighting();
        CharacterVfxManager.Instance.CheckForRainVfx();
        
        //StartCoroutine(WakeUp());

        Wake();
        
    }


    public void Wake()
    {
        //StartCoroutine(WakeUp());
        showScreenBehavior = true;
        frameCounter = 0;
        transitionElement.gameObject.SetActive(true);
        maskElement.sizeDelta = Vector2.zero;
        CharacterAnimManager.instance.animator.SetTrigger(DoWakeUp);
        CharacterAnimManager.instance.StopPurrSound();
        
        if (GameDontDestroyOnLoadManager.Instance.unlockedOnWakeUp)
        {
            CharacterInputManager.Instance.EnableInputs();
        }
    }

    
    void AddBiomeListeners()
    {
        // Debug.Log("Adding Biome Listeners");
        CollectHapticChallengeManager.Instance.UpdateCounters.RemoveAllListeners();
        foreach (var recipeDisplay in CodexContentManager.instance.recipes)
        {
            foreach (var container in recipeDisplay.ingredientDisplayContainers)
            {
                container.AddCollectListener();
            }
        }

        foreach (var ingredientDisplay in CodexContentManager.instance.ingredientPages)
        {
            ingredientDisplay.ingredientCounter.AddCollectListener();
        }
        
    }
    
    private void AddHouseListeners()
    {
        StirHapticChallengeManager.Instance.OnAddIngredient.RemoveAllListeners();
        // Debug.Log("Adding House Listeners");
        foreach (var recipeDisplay in CodexContentManager.instance.recipes)
        {
            foreach (var container in recipeDisplay.ingredientDisplayContainers)
            {
                container.AddCauldronListener();
            }
        }    
        
        foreach (var ingredientDisplay in CodexContentManager.instance.ingredientPages)
        {
            ingredientDisplay.ingredientCounter.AddCauldronListener();
        }
    }
    
}
