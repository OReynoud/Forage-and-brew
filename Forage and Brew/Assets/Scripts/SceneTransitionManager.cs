using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    public float transitionTime;
    private float timer;

    public Vector2 fullyExtendedDimensions;
    public GameObject transitionElement;

    public RectTransform maskElement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleSceneChange(string SceneName)
    {
        StartCoroutine(ChangeScenes(SceneName));
    }

    public void HandleLoadNewScene()
    {
        StartCoroutine(ShowScreen());
    }

    private IEnumerator ChangeScenes(string sceneName)
    {
        Time.timeScale = 0;
        timer = 0;
        transitionElement.gameObject.SetActive(true);
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(fullyExtendedDimensions, Vector2.zero, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
    private IEnumerator ShowScreen()
    {
        transitionElement.gameObject.SetActive(false);
        Time.timeScale = 0;
        timer = 0;
        maskElement.sizeDelta = Vector2.zero;
        transitionElement.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        while (timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            maskElement.sizeDelta = Vector2.Lerp(Vector2.zero, fullyExtendedDimensions, timer/transitionTime);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        transitionElement.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
