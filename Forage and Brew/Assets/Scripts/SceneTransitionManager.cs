using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    public float transitionTime;
    private float timer;

    public Vector2 fullyExtendedDimensions;
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

    public void HandleLoadNewScene()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ShowScreen());
        
    }

    public void HandleGoingToSleepTransition(float waitTime)
    {
        if (OnSleep != null)
            OnSleep.Invoke();
        StartCoroutine(HideScreen());
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
    
    private IEnumerator HideScreen()
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
        GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay = TimeOfDay.Daytime;
        GameDontDestroyOnLoadManager.Instance.DayPassed++;
        InfoDisplayManager.instance.DisplayDays();
        
                
        // Cycles
        WeatherManager.Instance.PassToNextWeatherState();
        LunarCycleManager.Instance.PassToNextLunarCycleState();

        
        StartCoroutine(WakeUp());
    }
    private IEnumerator ShowScreen()
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
        
        CharacterInputManager.Instance.EnableInputs();
    }

    public void Wake()
    {
        StartCoroutine(WakeUp());
    }
    private IEnumerator WakeUp()
    {
        CharacterInputManager.Instance.DisableInputs();
        transitionElement.gameObject.SetActive(false);
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
