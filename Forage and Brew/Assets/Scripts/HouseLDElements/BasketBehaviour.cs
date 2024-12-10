using UnityEngine;

public class BasketBehaviour : MonoBehaviour
{
    [SerializeField] private Transform meshParentTransform;
    [field: SerializeField] public IngredientValuesSo ingredient { get; set; }
    [SerializeField] private CollectedIngredientBehaviour collectedIngredientBehaviourPrefab;
    [SerializeField] private GameObject interactInputCanvasGameObject;
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        
        foreach (IngredientValuesSo collectedIngredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
        {
            if (collectedIngredient == ingredient)
            {
                Instantiate(collectedIngredient.MeshGameObject, meshParentTransform);
            }
        }
    }


    private void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    
    public CollectedIngredientBehaviour InstantiateCollectedIngredient()
    {
        CollectedIngredientBehaviour collectedIngredientBehaviour =
            Instantiate(collectedIngredientBehaviourPrefab, transform);
        collectedIngredientBehaviour.IngredientValuesSo = ingredient;
        
        return collectedIngredientBehaviour;
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            (characterInteractController.collectedIngredientStack.Count == 0 ||
             characterInteractController.collectedIngredientStack[0].ingredient.IngredientValuesSo == ingredient) &&
            GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Contains(ingredient))
        {
            characterInteractController.CurrentNearBaskets.Add(this);
            EnableInteract();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.CurrentNearBaskets.Contains(this))
        {
            characterInteractController.CurrentNearBaskets.Remove(this);
            DisableInteract();
        }
    }
}
