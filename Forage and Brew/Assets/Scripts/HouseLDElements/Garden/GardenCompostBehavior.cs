using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

public class GardenCompostBehavior : MonoBehaviour, IIngredientAddable
{
    public CollectedSeedBehavior seedBehaviorPrefab;
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [Foldout("Debug")] [SerializeField] private bool compostIsFull;

    [ReadOnly] public SeedValuesSo currentSeed;
    public List<IngredientTypeSo> storedIngredients;
    [field: SerializeField] public bool UseEndPoint { get; set; }
    [field: ShowIf("UseEndPoint")][field: SerializeField] public Transform EndPoint { get; set; }
    [field: ShowIf("UseEndPoint")][field: SerializeField] public float heightShove { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GardenManager.instance.compostBox = this;
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
                Debug.Log("Cant add more ingredients");
                CharacterAnimManager.instance.CatNo();
                return;
            }
            //TODO: Compost HapticChallenge
            CompleteCompostHapticChallenge();
            return;
        }

        if (CharacterInteractController.Instance.collectedStack.Count <= 0)
            return;
        
        if (CharacterInteractController.Instance.collectedStack[0].StackableItem is not CollectedIngredientBehaviour)
            return;
        
        var comparator =
            (CollectedIngredientBehaviour)CharacterInteractController.Instance.collectedStack[0].StackableItem;
        if (comparator.CookedForm == null)
        {
            Debug.Log("Ingredient not cooked");
            CharacterAnimManager.instance.CatNo();
            return;
        }
        if (currentSeed == null)
        {
            Debug.Log("Started Seed making");
            currentSeed = comparator.IngredientValuesSo.Type.AssociatedSeedValuesSo;
            TryAddIngredients();
        }
        else if (currentSeed.RequiredIngredientType == comparator.IngredientValuesSo.Type)
        {
            Debug.Log("Added same ingredient type");
            TryAddIngredients();
        }
        else
        {
            Debug.Log("Ingredient not valid");
            CharacterAnimManager.instance.CatNo();
        }
    }



    private List<CharacterInteractController.CollectedStack> temp = new();
    public void TryAddIngredients()
    {
        CharacterAnimManager.instance.CatThrow();
        for (int i = currentSeed.RequiredIngredientTypeAmount - storedIngredients.Count - 1; i >= 0; i--)
        {
            if (CharacterInteractController.Instance.collectedStack.Count == 0)
            {
                CharacterInteractController.Instance.AreHandsFull = false;
                break;
            }

            AddIngredient((CollectedIngredientBehaviour)CharacterInteractController.Instance.collectedStack[i].StackableItem);
            
            if (storedIngredients.Count == currentSeed.RequiredIngredientTypeAmount)
            {
                if (CharacterInteractController.Instance.collectedStack.Count == 0)
                    CharacterInteractController.Instance.AreHandsFull = false;
                
                CharacterInteractController.Instance.ShovePartialStackInTarget(transform, this, temp.ToArray());
                CloseCompostBox();
                temp.Clear();
                return;
            }
        }
        CharacterInteractController.Instance.ShovePartialStackInTarget(transform, this, temp.ToArray());
        temp.Clear();
    }
    
    public void AddIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        storedIngredients.Add(collectedIngredientBehaviour.IngredientValuesSo.Type);
        temp.Add(CharacterInteractController.Instance.collectedStack[^1]);
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
            characterInteractController.CurrentNearCompostBox = this;

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
        compostIsFull = true;
    }

    private void CompleteCompostHapticChallenge()
    {
        var newSeed = Instantiate(seedBehaviorPrefab, transform.position, transform.rotation);
        newSeed.SeedValuesSo = currentSeed;
        currentSeed = null;
        CharacterInteractController.Instance.AddToPile(newSeed);
        compostIsFull = false;
        
        Debug.Log("Obtained new Seed");
    }
    
}
