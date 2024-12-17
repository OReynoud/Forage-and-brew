using UnityEngine;

public class IngredientBasketBehaviour : BasketBehaviour, IIngredientAddable
{
    [field: SerializeField] public IngredientValuesSo ingredient { get; set; }
    [SerializeField] private CollectedIngredientBehaviour collectedIngredientBehaviourPrefab;
    public IngredientBasketManagerBehaviour IngredientBasketManagerBehaviour { get; set; }
    public int IngredientCount { get; private set; }
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        
        SetBasketContent(ingredient);
    }
    
    private void OnDisable()
    {
        if (CharacterInteractController.Instance.CurrentNearIngredientBaskets.Contains(this))
        {
            CharacterInteractController.Instance.CurrentNearIngredientBaskets.Remove(this);
        }
    }
    

    public void SetBasketContent(IngredientValuesSo newIngredient)
    {
        if (meshParentTransform.childCount > 0)
        {
            Destroy(meshParentTransform.GetChild(0).gameObject);
        }
        
        IngredientCount = 0;
        ingredient = newIngredient;
        
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

    public void AddIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        IngredientCount++;
        Destroy(collectedIngredientBehaviour.gameObject);
        meshParentTransform.gameObject.SetActive(true);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            IngredientBasketManagerBehaviour.ManageTriggerEnter();
            
            characterInteractController.CurrentNearIngredientBaskets.Add(this);
            
            if (characterInteractController.collectedStack.Count > 0 &&
                (CollectedIngredientBehaviour)characterInteractController.collectedStack[0].stackable &&
                ((CollectedIngredientBehaviour)characterInteractController.collectedStack[0].stackable).IngredientValuesSo == ingredient)
            {
                EnableCancel();

                if (IngredientCount > 0)
                {
                    EnableInteract();
                }
            }
            else if (characterInteractController.collectedStack.Count == 0 && IngredientCount > 0)
            {
                EnableInteract();
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!DoesNeedToCheckAvailability) return;
        
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            if (characterInteractController.collectedStack.Count > 0 &&
                (CollectedIngredientBehaviour)characterInteractController.collectedStack[0].stackable &&
                ((CollectedIngredientBehaviour)characterInteractController.collectedStack[0].stackable).IngredientValuesSo == ingredient)
            {
                EnableCancel();

                if (IngredientCount > 0)
                {
                    EnableInteract();
                }
                else
                {
                    DisableInteract();
                }
            }
            else if (characterInteractController.collectedStack.Count == 0 && IngredientCount > 0)
            {
                EnableInteract();
                DisableCancel();
            }
            else
            {
                DisableInteract();
                DisableCancel();
            }
        
            DoesNeedToCheckAvailability = false;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            IngredientBasketManagerBehaviour.ManageTriggerExit();
            
            if (characterInteractController.CurrentNearIngredientBaskets.Contains(this))
            {
                characterInteractController.CurrentNearIngredientBaskets.Remove(this);
                DisableInteract();
                DisableCancel();
            }
        }
    }
}
