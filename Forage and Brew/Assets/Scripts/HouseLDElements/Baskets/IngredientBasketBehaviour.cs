using UnityEngine;

public class IngredientBasketBehaviour : BasketBehaviour, IIngredientAddable
{
    [field: SerializeField] public IngredientValuesSo ingredient { get; set; }
    [SerializeField] private CollectedIngredientBehaviour collectedIngredientBehaviourPrefab;
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
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            if (characterInteractController.collectedStack.Count > 0 &&
                (CollectedIngredientBehaviour)characterInteractController.collectedStack[0].stackable &&
                ((CollectedIngredientBehaviour)characterInteractController.collectedStack[0].stackable).IngredientValuesSo == ingredient)
            {
                characterInteractController.CurrentNearIngredientBaskets.Add(this);
                EnableCancel();

                if (IngredientCount > 0)
                {
                    EnableInteract();
                }
            }
            else if (characterInteractController.collectedStack.Count == 0 && IngredientCount > 0)
            {
                characterInteractController.CurrentNearIngredientBaskets.Add(this);
                EnableInteract();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.CurrentNearIngredientBaskets.Contains(this))
        {
            characterInteractController.CurrentNearIngredientBaskets.Remove(this);
            DisableInteract();
            DisableCancel();
        }
    }

    public void AddIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        IngredientCount++;
        Destroy(collectedIngredientBehaviour.gameObject);
        meshParentTransform.gameObject.SetActive(true);
    }
}
