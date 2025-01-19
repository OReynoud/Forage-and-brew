using System.Collections.Generic;
using UnityEngine;

public class ChoppingCountertopBehaviour : MonoBehaviour, IIngredientAddable
{
    [field: SerializeField] public CountertopVfxManager CountertopVfxManager { get; private set; }
    [SerializeField] private GameObject interactInputCanvasGameObject;
    
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
    
    
    private void OnTriggerEnter(Collider other)
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
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out ChoppingHapticChallengeManager choppingHapticChallengeManager) &&
            characterInteractController.CurrentNearChoppingCountertop == this)
        {
            characterInteractController.CurrentNearChoppingCountertop = null;
            choppingHapticChallengeManager.CurrentChoppingCountertopBehaviour = null;
            DisableInteract();
        }
    }
}
