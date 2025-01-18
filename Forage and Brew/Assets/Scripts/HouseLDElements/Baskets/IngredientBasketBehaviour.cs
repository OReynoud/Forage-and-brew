using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IngredientBasketBehaviour : BasketBehaviour, IIngredientAddable
{
    [field: SerializeField] public IngredientValuesSo ingredient { get; set; }
    [SerializeField] private CollectedIngredientBehaviour collectedIngredientBehaviourPrefab;
    [SerializeField] private GameObject ingredientSpriteCanvasGameObject;
    [SerializeField] private Image ingredientSpriteImage;
    [SerializeField] private Image ingredientBackgroundImage;
    [SerializeField] private Sprite ingredientInBoxBackgroundSprite;
    [SerializeField] private Sprite ingredientOutOfBoxBackgroundSprite;
    public IngredientBasketManagerBehaviour IngredientBasketManagerBehaviour { get; set; }
    public int IngredientCount { get; private set; }
    
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        cancelInputCanvasGameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        IngredientBasketManagerBehaviour.ManageTriggerExit(this);
        
        if (CharacterInteractController.Instance.CurrentNearIngredientBaskets.Contains(this))
        {
            CharacterInteractController.Instance.CurrentNearIngredientBaskets.Remove(this);
        }
        
        DisableInteract();
        DisableCancel();
    }
    

    public void SetBasketContent(IngredientValuesSo newIngredient)
    {
        if (meshParentTransform.childCount > 0)
        {
            Destroy(meshParentTransform.GetChild(0).gameObject);
        }
        ingredientSpriteCanvasGameObject.SetActive(false);
        
        IngredientCount = 0;
        ingredient = newIngredient;
        
        if (GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Contains(ingredient))
        {
            Instantiate(ingredient.MeshGameObject, meshParentTransform);
            meshParentTransform.gameObject.SetActive(true);
            ingredientSpriteCanvasGameObject.SetActive(true);
            ingredientSpriteImage.sprite = ingredient.iconHigh;
            ingredientBackgroundImage.sprite = ingredientInBoxBackgroundSprite;

            foreach (IngredientValuesSo collectedIngredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
            {
                if (collectedIngredient == ingredient)
                {
                    IngredientCount++;
                }
            }
        }
        else if (GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients
                 .Select(behaviour => behaviour.IngredientValuesSo).Contains(ingredient))
        {
            ingredientSpriteCanvasGameObject.SetActive(true);
            ingredientSpriteImage.sprite = ingredient.iconHigh;
            ingredientBackgroundImage.sprite = ingredientOutOfBoxBackgroundSprite;
        }
    }
    
    public CollectedIngredientBehaviour InstantiateCollectedIngredient()
    {
        CollectedIngredientBehaviour collectedIngredientBehaviour =
            Instantiate(collectedIngredientBehaviourPrefab, transform);
        collectedIngredientBehaviour.IngredientValuesSo = ingredient;
        IngredientCount--;
        GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Add(collectedIngredientBehaviour);
        GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Remove(ingredient);

        if (IngredientCount == 0)
        {
            meshParentTransform.gameObject.SetActive(false);
            ingredientBackgroundImage.sprite = ingredientOutOfBoxBackgroundSprite;
        }
        
        return collectedIngredientBehaviour;
    }

    public void AddIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        IngredientCount++;
        GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Remove(collectedIngredientBehaviour);
        GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Add(ingredient);
        collectedIngredientBehaviour.OnIngredientDropEnd.AddListener(DestroyIngredient);
    }
    
    private void DestroyIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        Destroy(collectedIngredientBehaviour.gameObject);
        meshParentTransform.gameObject.SetActive(true);
        ingredientBackgroundImage.sprite = ingredientInBoxBackgroundSprite;
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            IngredientBasketManagerBehaviour.ManageTriggerEnter(this);
            
            characterInteractController.CurrentNearIngredientBaskets.Add(this);
            
            if (characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedIngredientBehaviour ingredientBehaviour &&
                ingredientBehaviour.IngredientValuesSo == ingredient)
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
                characterInteractController.collectedStack[0].stackable is CollectedIngredientBehaviour ingredientBehaviour &&
                ingredientBehaviour.IngredientValuesSo == ingredient)
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
            IngredientBasketManagerBehaviour.ManageTriggerExit(this);
            
            if (characterInteractController.CurrentNearIngredientBaskets.Contains(this))
            {
                characterInteractController.CurrentNearIngredientBaskets.Remove(this);
                DisableInteract();
                DisableCancel();
            }
        }
    }
}
