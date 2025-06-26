using System.Collections.Generic;
using UnityEngine;

public class ChoppingCountertopBehaviour : PurchasableHouseItemBehaviour, IIngredientAddable
{
    [field: SerializeField] public CountertopVfxManager CountertopVfxManager { get; private set; }
    
    [SerializeField] private AudioSource choppingAudioSource;
    
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

    public void ChopIngredient(CookHapticChallengeSo cookHapticChallengeSo)
    {
        _collectedIngredients[0].SetCookedForm(cookHapticChallengeSo);
        CharacterInteractController.Instance.AddToPile(_collectedIngredients[0]);
        _collectedIngredients.RemoveAt(0);

        if (_collectedIngredients.Count > 0)
        {
            ChoppingHapticChallengeManager.Instance.StartChoppingChallenge();
        }
    }
    
    
    public void PlayChoppingSound()
    {
        choppingAudioSource.Play();
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
                
                characterInteractController.CurrentNearChoppingCountertop = this;
            }
        }
        else if (Unlocked)
        {
            if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
                other.TryGetComponent(out ChoppingHapticChallengeManager choppingHapticChallengeManager) &&
                characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedIngredientBehaviour
                    { CookedForm: null })
            {
                LastTriggeredCollider = other;
                
                characterInteractController.CurrentNearChoppingCountertop = this;
                choppingHapticChallengeManager.CurrentChoppingCountertopBehaviour = this;

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
            other.TryGetComponent(out ChoppingHapticChallengeManager choppingHapticChallengeManager) &&
            characterInteractController.CurrentNearChoppingCountertop == this)
        {
            characterInteractController.CurrentNearChoppingCountertop = null;
            choppingHapticChallengeManager.CurrentChoppingCountertopBehaviour = null;
        }
            
        DisableInteract();
        pricePopUpBehaviour.HidePrice();
    }
}
