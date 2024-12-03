using System.Collections.Generic;
using UnityEngine;

public class CauldronBehaviour : Singleton<CauldronBehaviour>, IIngredientAddable
{
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private GameObject buttonAGameObject;
    [SerializeField] private GameObject buttonYGameObject;
    [field: SerializeField] public Transform SpoonTransform { get; private set; }

    public List<TemperatureChallengeIngredients> TemperatureAndIngredients { get; } = new();

    
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
        if (TemperatureAndIngredients.Count == 0 || TemperatureAndIngredients[^1].Temperature != Temperature.None)
        {
            TemperatureAndIngredients.Add(new TemperatureChallengeIngredients(
                new List<CookedIngredientForm>(),
                Temperature.None));
        }
        
        TemperatureAndIngredients[^1].CookedIngredients.Add(new CookedIngredientForm(
            collectedIngredientBehaviour.IngredientValuesSo, collectedIngredientBehaviour.CookedForm));
    }
    
    public void AddTemperature(Temperature temperature)
    {
        if (TemperatureAndIngredients.Count == 0)
        {
            TemperatureAndIngredients.Add(new TemperatureChallengeIngredients(
                new List<CookedIngredientForm>(),
                temperature));
        }
        else
        {
            TemperatureAndIngredients[^1] = new TemperatureChallengeIngredients(
                TemperatureAndIngredients[^1].CookedIngredients,
                temperature);
        }
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
