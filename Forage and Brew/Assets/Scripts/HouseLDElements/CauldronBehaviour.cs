using System.Collections.Generic;
using UnityEngine;

public class CauldronBehaviour : Singleton<CauldronBehaviour>, IIngredientAddable
{
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private GameObject buttonAGameObject;
    [SerializeField] private GameObject buttonYGameObject;
    [field: SerializeField] public Transform SpoonTransform { get; private set; }

    private readonly List<TemperatureChallengeIngredients> _temperatureAndIngredients = new();

    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
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
        if (_temperatureAndIngredients.Count == 0)
        {
            CauldronVfxManager.Instance.ChangeSmokeVfx(true);
        }
        
        if (_temperatureAndIngredients.Count == 0 || _temperatureAndIngredients[^1].Temperature != Temperature.None)
        {
            _temperatureAndIngredients.Add(new TemperatureChallengeIngredients(
                new List<CookedIngredientForm>(),
                Temperature.None));
        }
        
        _temperatureAndIngredients[^1].CookedIngredients.Add(new CookedIngredientForm(
            collectedIngredientBehaviour.IngredientValuesSo, collectedIngredientBehaviour.CookedForm));
    }
    
    public void AddTemperature(Temperature temperature)
    {
        if (_temperatureAndIngredients.Count == 0)
        {
            _temperatureAndIngredients.Add(new TemperatureChallengeIngredients(
                new List<CookedIngredientForm>(),
                temperature));
        }
        else
        {
            _temperatureAndIngredients[^1] = new TemperatureChallengeIngredients(
                _temperatureAndIngredients[^1].CookedIngredients,
                temperature);
        }
    }
    
    public List<TemperatureChallengeIngredients> ClearIngredients()
    {
        List<TemperatureChallengeIngredients> temperatureAndIngredientsList = new(_temperatureAndIngredients);
        _temperatureAndIngredients.Clear();
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
