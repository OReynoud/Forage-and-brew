using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterInteractController : MonoBehaviour
{
    public static CharacterInteractController Instance { get; private set; }
    
    [field:Foldout("Debug")] [ReadOnly] public List<CollectedIngredientStack> collectedIngredientStack = new();

    [Serializable]
    public class CollectedIngredientStack
    {
        [field:SerializeField] [field:ReadOnly] public CollectedIngredientBehaviour ingredient { get; set; }
        [field:SerializeField] [field:ReadOnly] public bool isPickedUp { get; set; }

        public CollectedIngredientStack(CollectedIngredientBehaviour Ingredient)
        {
            ingredient = Ingredient;
            isPickedUp = false;
        }
    }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public IngredientToCollectBehaviour CurrentIngredientToCollectBehaviour { get; private set; }

    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<CollectedIngredientBehaviour> CurrentCollectedIngredientBehaviours { get; private set; } = new();

    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public BedBehaviour CurrentNearBed { get; set; }
    
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public MailBox CurrentNearMailBox { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public CauldronBehaviour CurrentNearCauldron { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public ChoppingCountertopBehaviour CurrentNearChoppingCountertop { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public bool AreHandsFull { get; private set; }

    private Rigidbody rb { get; set; }

    [BoxGroup("Collected ingredients stack variables")]
    public Transform stackPlacement;

    [BoxGroup("Collected ingredients stack variables")] [field: Min(0f)]
    public int maxStackSize;

    [BoxGroup("Collected ingredients stack variables")] [field: Range(0f, 1f)]
    public float pickupLerp;

    [BoxGroup("Collected ingredients stack variables")] [field: Range(0f, 1f)]
    public float stackLerp;

    [BoxGroup("Collected ingredients stack variables")] [field: Min(0f)]
    public float stackRadius;

    [BoxGroup("Collected ingredients stack variables")] [field: Min(0f)]
    public float stackDisplacementClamp;


    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetNewIngredientToCollect(IngredientToCollectBehaviour newIngredientToCollectBehaviour)
    {
        if (CurrentIngredientToCollectBehaviour)
        {
            CurrentIngredientToCollectBehaviour.DisableCollect();
        }

        CurrentIngredientToCollectBehaviour = newIngredientToCollectBehaviour;
    }

    public void AddNewCollectedIngredient(CollectedIngredientBehaviour newCollectedIngredientBehaviour)
    {
        if (CurrentCollectedIngredientBehaviours.Count > 0)
        {
            CurrentCollectedIngredientBehaviours[^1].DisableGrab();
        }

        CurrentCollectedIngredientBehaviours.Add(newCollectedIngredientBehaviour);
    }

    public void RemoveCollectedIngredient(CollectedIngredientBehaviour newCollectedIngredientBehaviour)
    {
        CurrentCollectedIngredientBehaviours.Remove(newCollectedIngredientBehaviour);
        
        if (CurrentCollectedIngredientBehaviours.Count > 0)
        {
            CurrentCollectedIngredientBehaviours[^1].EnableGrab();
        }
    }

    public void Interact()
    {
        if (CurrentNearCauldron && collectedIngredientStack.Count > 0)
        {
            CurrentNearCauldron.DisableInteract(true);
            ShoveStackInTarget(CurrentNearCauldron.transform, CurrentNearCauldron);
        }
        else if (CurrentIngredientToCollectBehaviour)
        {
            CharacterInputManager.Instance.DisableMoveInputs();
            CurrentIngredientToCollectBehaviour.DisableCollect();
            CollectHapticChallengeManager.Instance.StartCollectHapticChallenge(CurrentIngredientToCollectBehaviour);
            CurrentIngredientToCollectBehaviour = null;
        }
        else if (CurrentCollectedIngredientBehaviours.Count > 0)
        {
            AddToPile(CurrentCollectedIngredientBehaviours[^1]);
        }
        else if (CurrentNearBed && collectedIngredientStack.Count == 0)
        {
            CurrentNearBed.Sleep();
        }
        else if (CurrentNearMailBox)
        {
            CurrentNearMailBox.ShowLetters();
            Debug.Log("Check letters");
        }
    }
    
    public void Cancel()
    {
        if (collectedIngredientStack.Count > 0)
        {
            int length = collectedIngredientStack.Count;

            for (int i = 0; i < length; i++)
            {
                collectedIngredientStack[0].ingredient.GrabMethod(false);
                collectedIngredientStack[0].ingredient.transform.SetParent(null);
                collectedIngredientStack.RemoveAt(0);
            }
            
            AreHandsFull = false;
        }
    }
    
    public void DropIngredientsInChoppingCountertop()
    {
        if (!CurrentNearChoppingCountertop || collectedIngredientStack.Count == 0) return;
        
        CurrentNearChoppingCountertop.DisableInteract();
        ShoveStackInTarget(CurrentNearChoppingCountertop.transform, CurrentNearChoppingCountertop, new Vector3(0.5f, 1.15f, -0.05f));
        
        ChoppingHapticChallengeManager.Instance.StartChoppingChallenge();
    }


    public void AddToPile(CollectedIngredientBehaviour ingredient)
    {
        if (collectedIngredientStack.Count > 0 && collectedIngredientStack.Count < maxStackSize)
        {
            if (collectedIngredientStack[0].ingredient.IngredientValuesSo.Type != ingredient.IngredientValuesSo.Type)
                return;
        }
        
        ingredient.GrabMethod(true);
        ingredient.transform.SetParent(transform);
        collectedIngredientStack.Add(new CollectedIngredientStack(ingredient));
        RemoveCollectedIngredient(ingredient);
        
        AreHandsFull = true;
    }

    private void ShoveStackInTarget(Transform targetTransform, IIngredientAddable targetBehaviour, Vector3 offset = default)
    {
        for (int i = 0; i < collectedIngredientStack.Count; i++)
        {
            collectedIngredientStack[i].ingredient.transform.SetParent(targetTransform);
            collectedIngredientStack[i].ingredient.DropInTarget(targetTransform, offset);
            targetBehaviour.AddIngredient(collectedIngredientStack[i].ingredient);
        }
        
        collectedIngredientStack.Clear();
        AreHandsFull = false;
    }

    private void FixedUpdate()
    {
        DisplaceStack();
    }

    float clampedDisplacement;

    void DisplaceStack()
    {
        for (var i = 0; i < collectedIngredientStack.Count; i++)
        {
            clampedDisplacement = Mathf.Clamp(rb.linearVelocity.magnitude, 0, stackDisplacementClamp);
            var ingredient = collectedIngredientStack[i].ingredient;

            if (collectedIngredientStack[i].isPickedUp)
            {
                //y lerp
                ingredient.transform.localPosition = Vector3.Lerp(ingredient.transform.localPosition,
                    new Vector3(ingredient.transform.localPosition.x, ingredient.StackHeight * i,
                        ingredient.transform.localPosition.z), pickupLerp);
                
                //x and z lerp
                if (i == 0)
                {
                    ingredient.transform.localPosition = Vector3.Lerp(ingredient.transform.localPosition,
                        new Vector3(0, ingredient.transform.localPosition.y, 0), stackLerp);
                }
                else
                {
                    ingredient.transform.localPosition = Vector3.Lerp(ingredient.transform.localPosition,
                        new Vector3(0, ingredient.transform.localPosition.y, -clampedDisplacement * i), stackLerp);
                }

                continue;
            }


            //y lerp
            ingredient.transform.position = Vector3.Lerp(ingredient.transform.position,
                new Vector3(ingredient.transform.position.x, stackPlacement.position.y + ingredient.StackHeight * i,
                    ingredient.transform.position.z), pickupLerp);

            //x and z lerp
            if (Vector2.Distance(new Vector2(ingredient.transform.position.x, ingredient.transform.position.z),
                    new Vector2(stackPlacement.position.x, stackPlacement.position.z)) > stackRadius)
            {
                ingredient.transform.position = Vector3.Lerp(ingredient.transform.position,
                    new Vector3(stackPlacement.position.x, ingredient.transform.position.y, stackPlacement.position.z),
                    pickupLerp);
            }
            else if (!collectedIngredientStack[i].isPickedUp)
            {
                collectedIngredientStack[i].isPickedUp = true;
                ingredient.transform.SetParent(stackPlacement);
            }

        }
    }

    
}
    
