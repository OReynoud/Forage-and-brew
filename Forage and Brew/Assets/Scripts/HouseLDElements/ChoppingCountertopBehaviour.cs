using System.Collections.Generic;
using UnityEngine;

public class ChoppingCountertopBehaviour : PurchasableHouseItemBehaviour, IIngredientAddable
{
    [field: SerializeField] public CountertopVfxManager CountertopVfxManager { get; private set; }
    [SerializeField] private AudioSource choppingAudioSource;
    
    private readonly List<CollectedIngredientBehaviour> _collectedIngredients = new();
    
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
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (CanPurchase)
        {
            if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
                other.TryGetComponent(out ChoppingHapticChallengeManager choppingHapticChallengeManager))
            {
                ShowPrice();
                characterInteractController.CurrentNearChoppingCountertop = this;
                choppingHapticChallengeManager.CurrentChoppingCountertopBehaviour = this;
            }
        }
        else if(Unlocked)
        {
            if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
                other.TryGetComponent(out ChoppingHapticChallengeManager choppingHapticChallengeManager) &&
                characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedIngredientBehaviour
                    { CookedForm: not ChoppingHapticChallengeListSo })
            {
                characterInteractController.CurrentNearChoppingCountertop = this;
                choppingHapticChallengeManager.CurrentChoppingCountertopBehaviour = this;

                EnableInteract();

            }
        }

    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out ChoppingHapticChallengeManager choppingHapticChallengeManager) &&
            characterInteractController.CurrentNearChoppingCountertop == this)
        {
            characterInteractController.CurrentNearChoppingCountertop = null;
            choppingHapticChallengeManager.CurrentChoppingCountertopBehaviour = null;
            
            if (Unlocked)
            {
                DisableInteract();
            }
            else if (CanPurchase)
            {
                HidePrice();
            }
        }
    }
}
