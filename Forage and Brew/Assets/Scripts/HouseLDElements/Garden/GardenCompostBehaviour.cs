using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class GardenCompostBehaviour : MonoBehaviour, IIngredientAddable
{
    // Singleton
    public static GardenCompostBehaviour Instance { get; private set; }
    
    public CollectedSeedBehaviour seedBehaviourPrefab;
    [Header("Compost Humus")]
    [SerializeField] private Transform compostHumusTransform;
    [SerializeField] private List<float> compostHumusFillHeights;
    [SerializeField] private float compostHumusFillDuration;
    [SerializeField] private AnimationCurve compostHumusFillCurve;
    [SerializeField] private float compostHumusBreakingDownHeight;
    [SerializeField] private float compostHumusBreakingDownDuration;
    [SerializeField] private AnimationCurve compostHumusBreakingDownCurve;
    [Header("VFX")]
    [SerializeField] private ParticleSystem finishSeedParticleSystem;
    [Header("UI")]
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private GameObject buttonAGameObject;
    [SerializeField] private GameObject buttonXGameObject;
    [Header("Ingredient Type Display")]
    [SerializeField] private GameObject ingredientTypeCanvasGameObject;
    [SerializeField] private Image ingredientTypeImage;
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
            ingredientTypeCanvasGameObject.SetActive(true);
            ingredientTypeImage.sprite = currentSeed.RequiredIngredientType.IconHigh;
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
                EnableHapticChallenge();
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
        
        collectedIngredientBehaviour.OnIngredientDropEnd.AddListener(DestroyIngredient);
        
        DisableInteraction();
    }

    private void DestroyIngredient(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        Destroy(collectedIngredientBehaviour.gameObject);
        
        // Humus visual update
        compostHumusTransform.gameObject.SetActive(true);
        compostHumusTransform.DOKill();
        compostHumusTransform.DOLocalMoveY(compostHumusFillHeights[storedIngredients.Count - 1],
            compostHumusFillDuration).SetEase(compostHumusFillCurve);
    }
    
    
    public void BreakDownHumus()
    {
        compostHumusTransform.DOKill();
        compostHumusTransform.localPosition = new Vector3(compostHumusTransform.localPosition.x, 
            compostHumusFillHeights[^1], compostHumusTransform.localPosition.z);
        compostHumusTransform.DOLocalMoveY(compostHumusFillHeights[^1] + compostHumusBreakingDownHeight,
                compostHumusBreakingDownDuration).SetEase(compostHumusBreakingDownCurve).SetLoops(2, LoopType.Yoyo);
    }
    
    
    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
        buttonAGameObject.SetActive(true);
        buttonXGameObject.SetActive(false);
    }
    
    public void EnableHapticChallenge()
    {
        interactInputCanvasGameObject.SetActive(true);
        buttonAGameObject.SetActive(false);
        buttonXGameObject.SetActive(true);
    }
    
    public void DisableInteraction()
    {
        interactInputCanvasGameObject.SetActive(false);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out CompostHapticChallengeManager compostHapticChallengeManager))
        {
            characterInteractController.CurrentNearCompostBox = this;
            compostHapticChallengeManager.CurrentCompost = this;

            if (compostIsFull && characterInteractController.collectedStack.Count == 0)
            {
                EnableHapticChallenge();
            }
            else if (!compostIsFull && characterInteractController.collectedStack.Count > 0 && 
                     characterInteractController.collectedStack[0].StackableItem is CollectedIngredientBehaviour)
            {
                EnableInteract();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out CompostHapticChallengeManager compostHapticChallengeManager))
        {
            DisableInteraction();
            
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
        storedIngredients.Clear();
        ingredientTypeCanvasGameObject.SetActive(false);
        compostHumusTransform.DOKill();
        compostHumusTransform.gameObject.SetActive(false);
        CharacterInteractController.Instance.AddToPile(newSeed);
        compostIsFull = false;
        
        Debug.Log("Obtained new Seed");
    }

    public void PlayObtainedSeedVfx()
    {
        finishSeedParticleSystem.Play();
    }
}
