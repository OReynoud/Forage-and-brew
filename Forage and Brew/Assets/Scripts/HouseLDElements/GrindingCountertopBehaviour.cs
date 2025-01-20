using System.Collections.Generic;
using UnityEngine;

public class GrindingCountertopBehaviour : MonoBehaviour, IIngredientAddable
{
    [field: SerializeField] public CountertopVfxManager CountertopVfxManager { get; private set; }
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private AudioSource grindingCrushAudioSource;
    [SerializeField] private AudioSource grindingEndAudioSource;
    [SerializeField] private AudioSource grindingTrailAudioSource;
    
    private readonly List<CollectedIngredientBehaviour> _collectedIngredients = new();

    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
    }


    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    
    public void AddIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        _collectedIngredients.Add(collectedIngredientBehaviour);
    }

    public void GrindIngredient(CookHapticChallengeSo cookHapticChallengeSo)
    {
        _collectedIngredients[0].SetCookedForm(cookHapticChallengeSo);
        CharacterInteractController.Instance.AddToPile(_collectedIngredients[0]);
        _collectedIngredients.RemoveAt(0);

        if (_collectedIngredients.Count > 0)
        {
            GrindingHapticChallengeManager.Instance.StartGrindingChallenge();
        }
    }
    
    
    public void EnterGrindingChallenge()
    {
        grindingTrailAudioSource.Play();
    }
    
    public void ExitGrindingChallenge()
    {
        grindingTrailAudioSource.Stop();
        grindingEndAudioSource.Play();
    }
    
    public void PlayCrushSound()
    {
        grindingCrushAudioSource.Play();
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out GrindingHapticChallengeManager grindingHapticChallengeManager) &&
            characterInteractController.collectedStack.Count > 0 &&
            characterInteractController.collectedStack[0].stackable is CollectedIngredientBehaviour
                { CookedForm: not GrindingHapticChallengeSo })
        {
            characterInteractController.CurrentNearGrindingCountertop = this;
            grindingHapticChallengeManager.CurrentGrindingCountertopBehaviour = this;
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out GrindingHapticChallengeManager grindingHapticChallengeManager) &&
            characterInteractController.CurrentNearGrindingCountertop == this)
        {
            characterInteractController.CurrentNearGrindingCountertop = null;
            grindingHapticChallengeManager.CurrentGrindingCountertopBehaviour = null;
            DisableInteract();
        }
    }
}
