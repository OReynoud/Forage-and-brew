using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CauldronBehaviour : Singleton<CauldronBehaviour>, IIngredientAddable
{
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private GameObject buttonAGameObject;
    [SerializeField] private GameObject buttonYGameObject;
    [field: SerializeField] public Transform SpoonTransform { get; private set; }
    
    [SerializeField] private List<AudioSource> fireAmbianceAudioSources;
    private readonly List<float> _fireAmbianceVolumes = new();
    [SerializeField] private float fireAmbianceFadeDuration = 1f;
    [SerializeField] private AudioSource checkInputAudioSource;
    [SerializeField] private List<AudioClip> checkInputAudioClips;
    [SerializeField] private AudioSource checkInputFinalAudioSource;
    [SerializeField] private AudioSource brewingAudioSource;
    [SerializeField] private List<AudioClip> brewingAudioClips;
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        
        if (GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients.Count > 0)
        {
            CauldronVfxManager.Instance.ChangeSmokeVfx(true);
        }
    }


    public void EnableInteract(bool areHandsFull)
    {
        buttonAGameObject.SetActive(areHandsFull);
        buttonYGameObject.SetActive(!areHandsFull);
        
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract(bool isStillNear = false)
    {
        if (isStillNear)
        {
            buttonAGameObject.SetActive(!buttonAGameObject.activeSelf);
            buttonYGameObject.SetActive(!buttonYGameObject.activeSelf);
        }
        else
        {
            interactInputCanvasGameObject.SetActive(false);
        }
    }
    
    
    public void AddIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        if (GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients.Count == 0)
        {
            CauldronVfxManager.Instance.ChangeSmokeVfx(true);
        }
        
        if (GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients.Count == 0 ||
            GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients[^1].Temperature != Temperature.None)
        {
            GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients.Add(
                new TemperatureChallengeIngredients(new List<CookedIngredientForm>(), Temperature.None));
        }
        
        GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Remove(collectedIngredientBehaviour);
        GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients[^1].CookedIngredients.Add(new CookedIngredientForm(
            collectedIngredientBehaviour.IngredientValuesSo, collectedIngredientBehaviour.CookedForm));
        
        collectedIngredientBehaviour.OnIngredientDropEnd.AddListener(DestroyIngredient);

        PinnedRecipe.instance.UpdateRecipeStepsCounter();
    }
    
    private void DestroyIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        Destroy(collectedIngredientBehaviour.gameObject);
    }
    
    public void AddTemperature(Temperature temperature)
    {
        if (GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients.Count == 0)
        {
            GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients.Add(new TemperatureChallengeIngredients(
                new List<CookedIngredientForm>(),
                temperature));
        }
        else
        {
            GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients[^1] = new TemperatureChallengeIngredients(
                GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients[^1].CookedIngredients,
                temperature);
        }
        
        PinnedRecipe.instance.UpdateRecipeStepsCounter();
    }
    
    public List<TemperatureChallengeIngredients> ClearIngredients()
    {
        List<TemperatureChallengeIngredients> temperatureAndIngredientsList =
            new(GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients);
        CauldronVfxManager.Instance.ChangeSmokeVfx(false);
        
        return temperatureAndIngredientsList;
    }
    
    
    public void StopFireAmbiance()
    {
        _fireAmbianceVolumes.Clear();
        
        foreach (AudioSource fireAmbianceAudioSource in fireAmbianceAudioSources)
        {
            _fireAmbianceVolumes.Add(fireAmbianceAudioSource.volume);
            fireAmbianceAudioSource.DOFade(0f, fireAmbianceFadeDuration).OnComplete(() => fireAmbianceAudioSource.Stop());
        }
    }
    
    public void PlayFireAmbiance()
    {
        for (int i = 0; i < fireAmbianceAudioSources.Count; i++)
        {
            fireAmbianceAudioSources[i].DOKill();
            fireAmbianceAudioSources[i].Play();
            fireAmbianceAudioSources[i].volume = _fireAmbianceVolumes[i];
        }
    }
    
    public void PlayCheckInputSound(int index)
    {
        checkInputAudioSource.PlayOneShot(checkInputAudioClips[index]);
    }
    
    public void PlayCheckInputFinalSound()
    {
        checkInputFinalAudioSource.Play();
    }
    
    public void PlayBrewingSound(int index)
    {
        brewingAudioSource.clip = brewingAudioClips[index];
        brewingAudioSource.Play();
    }
    
    public void StopBrewingSound()
    {
        brewingAudioSource.Stop();
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out StirHapticChallengeManager stirHapticChallengeManager))
        {
            characterInteractController.CurrentNearCauldron = this;
            stirHapticChallengeManager.CurrentCauldron = this;
            EnableInteract(characterInteractController.AreHandsFull);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out StirHapticChallengeManager stirHapticChallengeManager) &&
            characterInteractController.CurrentNearCauldron == this)
        {
            characterInteractController.CurrentNearCauldron = null;
            stirHapticChallengeManager.CurrentCauldron = null;
            DisableInteract();
        }
    }
}
