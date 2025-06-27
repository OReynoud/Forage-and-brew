using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    private static readonly int DoSleep = Animator.StringToHash("DoSleep");
    private static readonly int DoWakeUp = Animator.StringToHash("DoWakeUp");
    public float transitionTime;
    public float timer;
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

    private void Start()
    {
        if (debugSleep)
        {
            HandleGoingToSleepTransition(transform);
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
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ShowScreen(newScene));
        
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
        HouseCameraBehavior.overrideCameraLerp = true;
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(sleepCam,0);
        SimpleCameraBehavior.instance.InstantCamUpdate();
        
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
        
        
        HouseCameraBehavior.overrideCameraLerp = false;
        yield return new WaitForSecondsRealtime(sleepWaitTime);
        
        
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(focusedDimensions, Vector2.zero, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(camSettings,0);
        SimpleCameraBehavior.instance.InstantCamUpdate();
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

        
        StartCoroutine(WakeUp());
    }
    private IEnumerator ShowScreen(Scene newScene)
    {
        Debug.Log(timer);
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(Vector2.zero, fullyExtendedDimensions, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        transitionElement.gameObject.SetActive(false);
        
        GameDontDestroyOnLoadManager.Instance.CurrentScene = newScene;
        if (WeatherManager.Instance.CurrentWeatherState != null)
        {
            InfoDisplayManager.instance.DisplayWeather();
        }
        
        if (GameDontDestroyOnLoadManager.Instance.CurrentScene is Scene.Biome1 or Scene.Biome2)
        {
            AddListeners();
        }
        
        
        CharacterMovementController.Instance.SetupAudio(GameDontDestroyOnLoadManager.Instance.CurrentScene);
        CharacterInputManager.Instance.EnableInputs();
    }

    public void Wake()
    {

        StartCoroutine(WakeUp());
    }
    private IEnumerator WakeUp()
    {
        transitionElement.gameObject.SetActive(false);
        timer = 0;
        if (GameDontDestroyOnLoadManager.Instance.lockoutOnWakeUp)
        {
            CharacterInputManager.Instance.DisableInputs();
            GameDontDestroyOnLoadManager.Instance.lockoutOnWakeUp = false;
        }
        transitionElement.gameObject.SetActive(true);
        maskElement.sizeDelta = Vector2.zero;
        yield return new WaitForSecondsRealtime(0.5f);
        CharacterAnimManager.instance.animator.SetTrigger(DoWakeUp);
        CharacterAnimManager.instance.StopPurrSound();
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(Vector2.zero, fullyExtendedDimensions, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        transitionElement.gameObject.SetActive(false);
    }
    
    void AddListeners()
    {
        Debug.Log("Adding Listeners");
        foreach (var recipeDisplay in CodexContentManager.instance.recipes)
        {
            foreach (var container in recipeDisplay.ingredientDisplayContainers)
            {
                container.AddListener();
            }
        }

        foreach (var ingredientDisplay in CodexContentManager.instance.ingredientPages)
        {
            ingredientDisplay.ingredientCounter.AddListener();
        }
        
    }
    
}
