using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ChoppingCountertopBehaviour : PurchasableHouseItemBehaviour, IIngredientAddable
{
    [field: SerializeField] public CountertopVfxManager CountertopVfxManager { get; private set; }
    
    [SerializeField] private AudioSource choppingAudioSource;
    
    [SerializeField] private GameObject interactInputCanvasGameObject;
    
    public bool IsCharacterOnCountertop { get; set; }
    
    public List<CollectedIngredientBehaviour> CollectedIngredients { get; } = new();
    [field: SerializeField] public bool UseEndPoint { get; set; }
    [field: ShowIf("UseEndPoint")][field: SerializeField] public Transform EndPoint { get; set; }
    [field: ShowIf("UseEndPoint")][field: SerializeField] public float heightShove { get; set; }


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
        CollectedIngredients.Add(collectedIngredientBehaviour);
    }


    public void SetCutIngredient()
    {
        CollectedIngredients[0].SetCutMeshGameObject();
    }
    
    public void SetCutIngredientPositionAndRotation(int index)
    {
        CollectedIngredients[0].SetCutMeshPositionAndRotation(index);
    }

    public void ChopIngredient(CookHapticChallengeSo cookHapticChallengeSo)
    {
        CollectedIngredients[0].SetFinalCutMeshPositionAndRotation();
        
        CollectedIngredients[0].SetCookedForm(cookHapticChallengeSo);
        CharacterInteractController.Instance.AddToPile(CollectedIngredients[0]);
        CollectedIngredients.RemoveAt(0);

        if (CollectedIngredients.Count > 0)
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
        if (IsCharacterOnCountertop) return;
        
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
                characterInteractController.collectedStack[0].StackableItem is CollectedIngredientBehaviour
                    { CookedForm: null } &&
                ((CollectedIngredientBehaviour)characterInteractController.collectedStack[0].StackableItem)
                    .IngredientValuesSo.Type.IsChoppable)
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
        if (IsCharacterOnCountertop) return;

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
