using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GardenCompostBehaviour : MonoBehaviour, IIngredientAddable
{
    // Singleton
    public static GardenCompostBehaviour Instance { get; private set; }
    
    public CollectedSeedBehaviour seedBehaviourPrefab;
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [Foldout("Debug")] [SerializeField] private bool compostIsFull;

    [ReadOnly] public SeedValuesSo currentSeed;
    public List<IngredientTypeSo> storedIngredients;
    [field: SerializeField] public bool UseEndPoint { get; set; }
    [field: ShowIf("UseEndPoint")][field: SerializeField] public Transform EndPoint { get; set; }
    [field: ShowIf("UseEndPoint")][field: SerializeField] public float heightShove { get; set; }

    
    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        GardenManager.instance.compostBox = this;
    }
    
    public void HandlePlayerInput()
    {
        if (compostIsFull)
        {
            if (CharacterInteractController.Instance.collectedStack.Count > 0)
            {            
                Debug.Log("Cant add more ingredients");
                CharacterAnimManager.instance.CatNo();
            }
            
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
    
    public void HandlePlayerInputHapticChallenge()
    {
        if (!compostIsFull) return;
        
        if (CharacterInteractController.Instance.collectedStack.Count > 0)
        {            
            Debug.Log("Cant add more ingredients");
            CharacterAnimManager.instance.CatNo();
            return;
        }
        
        CompostHapticChallengeManager.Instance.StartCompostChallenge();
    }
    

    private List<CharacterInteractController.CollectedStack> temp = new();
    public void TryAddIngredients()
    {
        CharacterAnimManager.instance.CatThrow();
        for (int i = Mathf.Min(currentSeed.RequiredIngredientTypeAmount - storedIngredients.Count,
                 CharacterInteractController.Instance.collectedStack.Count) - 1; i >= 0; i--)
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
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out CompostHapticChallengeManager compostHapticChallengeManager))
        {
            if (characterInteractController.collectedStack.Count <= 0) return;
            
            characterInteractController.CurrentNearCompostBox = this;
            compostHapticChallengeManager.CurrentCompost = this;
            
            if (compostIsFull) return;
            
            EnableInteract();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out CompostHapticChallengeManager compostHapticChallengeManager))
        {
            DisableInteract();
            
            if (characterInteractController.CurrentNearCompostBox == this)
            {
                characterInteractController.CurrentNearCompostBox = null;
            }
            
            if (compostHapticChallengeManager.CurrentCompost == this)
            {
                compostHapticChallengeManager.CurrentCompost = null;
            }
        }
    }


    private void CloseCompostBox()
    {
        compostIsFull = true;
    }

    public void CompleteCompostHapticChallenge()
    {
        CollectedSeedBehaviour newSeed = Instantiate(seedBehaviourPrefab, transform.position, transform.rotation);
        newSeed.SeedValuesSo = currentSeed;
        currentSeed = null;
        CharacterInteractController.Instance.AddToPile(newSeed);
        compostIsFull = false;
        
        Debug.Log("Obtained new Seed");
    }
}
