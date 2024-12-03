using System.Collections.Generic;
using UnityEngine;

public class ChoppingCountertopBehaviour : MonoBehaviour, IIngredientAddable
{
    [SerializeField] private GameObject interactInputCanvasGameObject;
    
    private List<CollectedIngredientBehaviour> _collectedIngredients = new();

    
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
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ChoppingHapticChallengeManager cookHapticChallengeManager))
        {
            cookHapticChallengeManager.CurrentChoppingCountertopBehaviour = this;
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ChoppingHapticChallengeManager cookHapticChallengeManager) &&
            cookHapticChallengeManager.CurrentChoppingCountertopBehaviour == this)
        {
            cookHapticChallengeManager.CurrentChoppingCountertopBehaviour = null;
            DisableInteract();
        }
    }
}
