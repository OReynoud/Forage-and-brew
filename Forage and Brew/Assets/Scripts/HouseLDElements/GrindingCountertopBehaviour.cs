using System.Collections.Generic;
using UnityEngine;

public class GrindingCountertopBehaviour : PurchasableHouseItemBehaviour, IIngredientAddable
{
    [field: SerializeField] public CountertopVfxManager CountertopVfxManager { get; private set; }
    
    [SerializeField] private AudioSource grindingCrushAudioSource;
    [SerializeField] private AudioSource grindingEndAudioSource;
    [SerializeField] private AudioSource grindingTrailAudioSource;
    
    [SerializeField] private GameObject interactInputCanvasGameObject;
    
    private readonly List<CollectedIngredientBehaviour> _collectedIngredients = new();
    
    
    protected override void Start()
    {
        base.Start();
        
        DisableInteract();
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
        PlayGrindSound();
    }
    
    public void PlayGrindSound()
    {
        if (grindingTrailAudioSource.isPlaying) return;
        
        grindingTrailAudioSource.Play();
    }
    
    public void PauseGrindSound()
    {
        if (!grindingTrailAudioSource.isPlaying) return;
        
        grindingTrailAudioSource.Pause();
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
    
    
    protected override void ManageCharacterNear(Collider other)
    {
        if (CanPurchase)
        {
            if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
                characterInteractController.collectedStack.Count == 0)
            {
                LastTriggeredCollider = other;
                
                pricePopUpBehaviour.ShowPrice(purchaseCost);
                
                characterInteractController.CurrentNearGrindingCountertop = this;
            }
        }
        else if (Unlocked)
        {
            if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
                other.TryGetComponent(out GrindingHapticChallengeManager grindingHapticChallengeManager) &&
                characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedIngredientBehaviour
                    { CookedForm: null })
            {
                LastTriggeredCollider = other;
                
                characterInteractController.CurrentNearGrindingCountertop = this;
                grindingHapticChallengeManager.CurrentGrindingCountertopBehaviour = this;

                EnableInteract();

            }
        }
    }
    
    protected override void ManageCharacterFar(Collider other)
    {
        if (LastTriggeredCollider == other)
        {
            LastTriggeredCollider = null;
        }
        
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out GrindingHapticChallengeManager grindingHapticChallengeManager) &&
            characterInteractController.CurrentNearGrindingCountertop == this)
        {
            characterInteractController.CurrentNearGrindingCountertop = null;
            grindingHapticChallengeManager.CurrentGrindingCountertopBehaviour = null;
        }
            
        DisableInteract();
        pricePopUpBehaviour.HidePrice();
    }
}
