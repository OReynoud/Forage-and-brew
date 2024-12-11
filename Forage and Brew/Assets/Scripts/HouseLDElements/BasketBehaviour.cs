using UnityEngine;

public class BasketBehaviour : MonoBehaviour
{
    [SerializeField] private Transform meshParentTransform;
    [field: SerializeField] public IngredientValuesSo ingredient { get; set; }
    [SerializeField] private CollectedIngredientBehaviour collectedIngredientBehaviourPrefab;
    [SerializeField] private GameObject interactInputCanvasGameObject;
    public int IngredientCount { get; private set; }
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        
        if (GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Contains(ingredient))
        {
            Instantiate(ingredient.MeshGameObject, meshParentTransform);

            foreach (IngredientValuesSo collectedIngredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
            {
                if (collectedIngredient == ingredient)
                {
                    IngredientCount++;
                }
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
        IngredientCount--;

        if (IngredientCount == 0)
        {
            meshParentTransform.gameObject.SetActive(false);
        }
        
        return collectedIngredientBehaviour;
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            (characterInteractController.collectedIngredientStack.Count == 0 ||
             characterInteractController.collectedIngredientStack[0].ingredient.IngredientValuesSo == ingredient) &&
            IngredientCount > 0)
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
