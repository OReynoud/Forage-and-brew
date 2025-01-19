using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    public float transitionTime;
    private float timer;
    public float sleepWaitTime;
    public Vector3 sleepPos;
    public Vector3 sleepRotation;
    public CameraPreset sleepCam;

    public Vector2 fullyExtendedDimensions;
    public Vector2 focusedDimensions;
    public GameObject transitionElement;

    public RectTransform maskElement;
    public UnityEvent OnSleep { get; set; } = new();
    
    private Coroutine _coroutine;

    public void HandleSceneChange(string SceneName)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ChangeScenes(SceneName));
    }

    public void HandleLoadNewScene(Scene newScene)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ShowScreen(newScene));
        
    }

    public void HandleGoingToSleepTransition(Transform spawnPoint)
    {
        if (OnSleep != null)
            OnSleep.Invoke();
        StartCoroutine(HideScreen(spawnPoint));
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
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(fullyExtendedDimensions, Vector2.zero, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        var camSettings = CameraController.instance.TargetCamSettings;
        
        CameraController.instance.ApplyScriptableCamSettings(sleepCam,0);
        CameraController.instance.InstantCamUpdate();
        
        CharacterInteractController.Instance.transform.position = spawnPoint.position;
        CharacterInteractController.Instance.transform.rotation = spawnPoint.rotation;
        CharacterAnimManager.instance.animator.SetTrigger("DoSleep");
        CharacterAnimManager.instance.animator.transform.localPosition = sleepPos;
        CharacterAnimManager.instance.animator.transform.localRotation = Quaternion.Euler(sleepRotation);
        yield return new WaitForSecondsRealtime(0.1f);
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
        CameraController.instance.ApplyScriptableCamSettings(camSettings,0);
        CameraController.instance.InstantCamUpdate();
        CharacterAnimManager.instance.animator.transform.localPosition = Vector3.zero;
        CharacterAnimManager.instance.animator.transform.localRotation = Quaternion.identity;
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
        transitionElement.gameObject.SetActive(false);
        timer = 0;
        transitionElement.gameObject.SetActive(true);
        maskElement.sizeDelta = Vector2.zero;
        yield return new WaitForSecondsRealtime(0.5f);
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(Vector2.zero, fullyExtendedDimensions, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        transitionElement.gameObject.SetActive(false);
        
        GameDontDestroyOnLoadManager.Instance.CurrentScene = newScene;
        if (WeatherManager.Instance.CurrentWeatherStates.Count != 0)
        {
            InfoDisplayManager.instance.DisplayWeather();
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
        CharacterInputManager.Instance.DisableInputs();
        transitionElement.gameObject.SetActive(true);
        maskElement.sizeDelta = Vector2.zero;
        yield return new WaitForSecondsRealtime(0.5f);
        CharacterAnimManager.instance.animator.SetTrigger("DoWakeUp");
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(Vector2.zero, fullyExtendedDimensions, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        transitionElement.gameObject.SetActive(false);
    }
}
