using System.Collections.Generic;
using UnityEngine;

public class CauldronBehaviour : Singleton<CauldronBehaviour>, IIngredientAddable
{
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private GameObject buttonAGameObject;
    [SerializeField] private GameObject buttonYGameObject;
    [field: SerializeField] public Transform SpoonTransform { get; private set; }
    

    
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

        PinnedRecipe.instance.UpdateRecipeStepsCounter();
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
        GameDontDestroyOnLoadManager.Instance.CauldronTemperatureAndIngredients.Clear();
        CauldronVfxManager.Instance.ChangeSmokeVfx(false);
        
        return temperatureAndIngredientsList;
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
