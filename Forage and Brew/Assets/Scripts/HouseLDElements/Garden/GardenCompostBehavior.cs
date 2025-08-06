using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor.Rendering;
using UnityEngine;

public class GardenCompostBehavior : MonoBehaviour, IIngredientAddable
{
    public IngredientTypeSo mushroomType;
    public IngredientTypeSo herbType;
    public IngredientTypeSo mossType;
    public IngredientTypeSo berryType;
    public IngredientTypeSo veggieType;
    public SeedValuesSo mushroomSeed;
    public SeedValuesSo herbSeed;
    public SeedValuesSo mossSeed;
    public SeedValuesSo berrySeed;
    public SeedValuesSo veggieSeed;
    [SerializeField] private GameObject interactInputCanvasGameObject;
    private bool compostIsFull;

    [ReadOnly] public SeedValuesSo currentSeed;
    public List<IngredientTypeSo> storedIngredients;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void HandlePlayerInput()
    {
        if (compostIsFull)
        {
            if (CharacterInteractController.Instance.collectedStack.Count > 0)
            {
                return;
            }
            //TODO: Compost HapticChallenge
            return;
        }
        var comparator =
            (CollectedIngredientBehaviour)CharacterInteractController.Instance.collectedStack[0].StackableItem;
        if (comparator.CookedForm == null)
        {
            CharacterAnimManager.instance.CatNo();
            return;
        }
        if (currentSeed == null)
        {
            if (comparator.IngredientValuesSo.Type == mushroomType)
            {
                currentSeed = mushroomSeed;
            }
            else if (comparator.IngredientValuesSo.Type == herbType)
            {
                currentSeed = herbSeed;
            }
            else if (comparator.IngredientValuesSo.Type == mossType)
            {
                currentSeed = mossSeed;
            }
            else if (comparator.IngredientValuesSo.Type == berryType)
            {
                currentSeed = berrySeed;
            }
            else if (comparator.IngredientValuesSo.Type == veggieType)
            {
                currentSeed = veggieSeed;
            }
            TryAddIngredients();
        }
        else if (currentSeed.RequiredIngredientType == comparator.IngredientValuesSo.Type)
        {
            TryAddIngredients();
        }
        else
        {
            CharacterAnimManager.instance.CatNo();
        }
    }

    private List<CharacterInteractController.CollectedStack> temp = new();
    public void TryAddIngredients()
    {
        for (int i = 0; i < currentSeed.RequiredIngredientTypeAmount - storedIngredients.Count; i++)
        {
            AddIngredient((CollectedIngredientBehaviour)CharacterInteractController.Instance.collectedStack[^1].StackableItem);
            if (CharacterInteractController.Instance.collectedStack.Count == 0 || storedIngredients.Count == currentSeed.RequiredIngredientTypeAmount)
            {
                CharacterInteractController.Instance.ShovePartialStackInTarget(transform, this, temp.ToArray());
                compostIsFull = true;
                break;
            }
        }
        temp.Clear();
    }
    
    public void AddIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        storedIngredients.Add(collectedIngredientBehaviour.IngredientValuesSo.Type);
        CharacterInteractController.Instance.collectedStack.RemoveAt( CharacterInteractController.Instance.collectedStack.Count - 1);
    }
    
    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            EnableInteract();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            DisableInteract();
            
            if (characterInteractController.CurrentNearCompostBox == this)
            {
                characterInteractController.CurrentNearCompostBox = null;
            }
        }
    }

    void CloseCompostBox()
    {
        
    }




    public bool UseEndPoint { get; set; }
    public Transform EndPoint { get; set; }
    public float heightShove { get; set; }
}
