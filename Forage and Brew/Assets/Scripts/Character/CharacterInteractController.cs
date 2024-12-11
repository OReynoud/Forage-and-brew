using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterInteractController : MonoBehaviour
{
    public static CharacterInteractController Instance { get; private set; }
    
    [field:Foldout("Debug")] [ReadOnly] public List<CollectedIngredientStack> collectedIngredientStack = new();
    [field:Foldout("Debug")] [ReadOnly] public List<CollectedPotionStack> collectedPotionStack = new();

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

    [Serializable]
    public class CollectedPotionStack
    {
        [field:SerializeField] [field:ReadOnly] public CollectedPotionBehaviour potion { get; set; }
        [field:SerializeField] [field:ReadOnly] public bool isPickedUp { get; set; }

        public CollectedPotionStack(CollectedPotionBehaviour Potion)
        {
            potion = Potion;
            isPickedUp = false;
        }
    }
    
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public IngredientToCollectBehaviour CurrentIngredientToCollectBehaviour { get; private set; }

    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<CollectedIngredientBehaviour> CurrentCollectedIngredientBehaviours { get; private set; } = new();
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<CollectedPotionBehaviour> CurrentCollectedPotionBehaviours { get; private set; } = new();

    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public BedBehaviour CurrentNearBed { get; set; }
    
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public MailBox CurrentNearMailBox { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public CauldronBehaviour CurrentNearCauldron { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public ChoppingCountertopBehaviour CurrentNearChoppingCountertop { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<BasketBehaviour> CurrentNearBaskets { get; set; } = new();
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
    
    [SerializeField] private Vector3 choppingOffset = new(0f, 1.15f, -0.05f);


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

    public void AddNewCollectedPotion(CollectedPotionBehaviour newCollectedPotionBehaviour)
    {
        if (CurrentCollectedPotionBehaviours.Count > 0)
        {
            CurrentCollectedPotionBehaviours[^1].DisableGrab();
        }

        CurrentCollectedPotionBehaviours.Add(newCollectedPotionBehaviour);
    }

    public void RemoveCollectedPotion(CollectedPotionBehaviour newCollectedPotionBehaviour)
    {
        CurrentCollectedPotionBehaviours.Remove(newCollectedPotionBehaviour);
        
        if (CurrentCollectedPotionBehaviours.Count > 0)
        {
            CurrentCollectedPotionBehaviours[^1].EnableGrab();
        }
    }

    public void Interact()
    {
        if (CurrentNearBaskets.Count > 0)
        {
            ChooseBasket();
        }
        else if (CurrentNearCauldron && collectedIngredientStack.Count > 0)
        {
            CurrentNearCauldron.DisableInteract(true);
            for (int i = 0; i < collectedIngredientStack.Count; i++)
            {
                GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Remove(collectedIngredientStack[i].ingredient.IngredientValuesSo);
            }
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
        else if (CurrentCollectedPotionBehaviours.Count > 0)
        {
            AddToPile(CurrentCollectedPotionBehaviours[^1]);
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
            if (CurrentNearBaskets.Count > 0)
            {
                ShoveStackInTarget(CurrentNearBaskets[0].transform, CurrentNearBaskets[0]);
                CurrentNearBaskets[0].EnableInteract();
                CurrentNearBaskets[0].DisableCancel();
            }
            else
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
        
        if (collectedPotionStack.Count > 0)
        {
            int length = collectedPotionStack.Count;

            for (int i = 0; i < length; i++)
            {
                collectedPotionStack[0].potion.GrabMethod(false);
                collectedPotionStack[0].potion.transform.SetParent(null);
                collectedPotionStack.RemoveAt(0);
            }

            AreHandsFull = false;
        }
    }

    
    private void ChooseBasket()
    {
        if (collectedIngredientStack.Count >= maxStackSize) return;

        if (collectedIngredientStack.Count > 0)
        {
            int index;
            
            for (index = 0; index < CurrentNearBaskets.Count; index++)
            {
                if (CurrentNearBaskets[index].IngredientCount == 0) return;
                
                if (collectedIngredientStack[0].ingredient.IngredientValuesSo == CurrentNearBaskets[index].ingredient)
                {
                    AddToPile(CurrentNearBaskets[index].InstantiateCollectedIngredient());
                    break;
                }
            }
            
            if (index != CurrentNearBaskets.Count)
            {
                if (CurrentNearBaskets[index].IngredientCount == 0)
                {
                    CurrentNearBaskets[index].DisableInteract();
                }
                
                BasketBehaviour item = CurrentNearBaskets[index];
                CurrentNearBaskets.RemoveAt(index);
                index = CurrentNearBaskets.Count;
                CurrentNearBaskets.Insert(CurrentNearBaskets.Count, item);
            }
            
            int length = CurrentNearBaskets.Count;
            
            for (int i = 0; i < length; i++)
            {
                if (i != index)
                {
                    CurrentNearBaskets[0].DisableInteract();
                    CurrentNearBaskets.RemoveAt(0);
                }
            }

            return;
        }

        (int index, float dotValue) largestDot = (0, -1);
        
        for (int i = 0; i < CurrentNearBaskets.Count; i++)
        {
            float dotValue = Vector3.Dot(transform.forward, CurrentNearBaskets[i].transform.position - transform.position);
            
            if (dotValue > largestDot.dotValue)
            {
                largestDot = (i, dotValue);
            }
        }
        
        AddToPile(CurrentNearBaskets[largestDot.index].InstantiateCollectedIngredient());
        CurrentNearBaskets[largestDot.index].EnableCancel();
        
        if (CurrentNearBaskets[largestDot.index].IngredientCount == 0)
        {
            CurrentNearBaskets[largestDot.index].DisableInteract();
        }

        BasketBehaviour basket = CurrentNearBaskets[largestDot.index];
        CurrentNearBaskets.RemoveAt(largestDot.index);
        largestDot.index = CurrentNearBaskets.Count;
        CurrentNearBaskets.Insert(CurrentNearBaskets.Count, basket);
        
        int count = CurrentNearBaskets.Count;
            
        for (int i = 0; i < count; i++)
        {
            if (i != largestDot.index)
            {
                CurrentNearBaskets[0].DisableInteract();
                CurrentNearBaskets.RemoveAt(0);
            }
        }
    }
    
    
    public void DropIngredientsInChoppingCountertop()
    {
        if (!CurrentNearChoppingCountertop || collectedIngredientStack.Count == 0) return;
        
        CurrentNearChoppingCountertop.DisableInteract();
        ShoveStackInTarget(CurrentNearChoppingCountertop.transform, CurrentNearChoppingCountertop, choppingOffset);
        
        ChoppingHapticChallengeManager.Instance.StartChoppingChallenge();
    }


    public void AddToPile(CollectedIngredientBehaviour ingredient)
    {
        if (collectedIngredientStack.Count > 0 && collectedIngredientStack.Count < maxStackSize)
        {
            if (collectedIngredientStack[0].ingredient.IngredientValuesSo != ingredient.IngredientValuesSo)
                return;
        }
        
        ingredient.GrabMethod(true);
        ingredient.transform.SetParent(transform);
        collectedIngredientStack.Add(new CollectedIngredientStack(ingredient));
        RemoveCollectedIngredient(ingredient);
        
        AreHandsFull = true;
    }

    public void AddToPile(CollectedPotionBehaviour potion)
    {
        if (collectedPotionStack.Count > 0 && collectedPotionStack.Count < maxStackSize)
        {
            if (collectedPotionStack[0].potion.PotionValuesSo != potion.PotionValuesSo)
                return;
        }
        
        potion.GrabMethod(true);
        potion.transform.SetParent(transform);
        collectedPotionStack.Add(new CollectedPotionStack(potion));
        RemoveCollectedPotion(potion);
        
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
        
        for (var i = 0; i < collectedPotionStack.Count; i++)
        {
            clampedDisplacement = Mathf.Clamp(rb.linearVelocity.magnitude, 0, stackDisplacementClamp);
            var potion = collectedPotionStack[i].potion;

            if (collectedPotionStack[i].isPickedUp)
            {
                //y lerp
                potion.transform.localPosition = Vector3.Lerp(potion.transform.localPosition,
                    new Vector3(potion.transform.localPosition.x, potion.StackHeight * i,
                        potion.transform.localPosition.z), pickupLerp);
                
                //x and z lerp
                if (i == 0)
                {
                    potion.transform.localPosition = Vector3.Lerp(potion.transform.localPosition,
                        new Vector3(0, potion.transform.localPosition.y, 0), stackLerp);
                }
                else
                {
                    potion.transform.localPosition = Vector3.Lerp(potion.transform.localPosition,
                        new Vector3(0, potion.transform.localPosition.y, -clampedDisplacement * i), stackLerp);
                }

                continue;
            }


            //y lerp
            potion.transform.position = Vector3.Lerp(potion.transform.position,
                new Vector3(potion.transform.position.x, stackPlacement.position.y + potion.StackHeight * i,
                    potion.transform.position.z), pickupLerp);

            //x and z lerp
            if (Vector2.Distance(new Vector2(potion.transform.position.x, potion.transform.position.z),
                    new Vector2(stackPlacement.position.x, stackPlacement.position.z)) > stackRadius)
            {
                potion.transform.position = Vector3.Lerp(potion.transform.position,
                    new Vector3(stackPlacement.position.x, potion.transform.position.y, stackPlacement.position.z),
                    pickupLerp);
            }
            else if (!collectedPotionStack[i].isPickedUp)
            {
                collectedPotionStack[i].isPickedUp = true;
                potion.transform.SetParent(stackPlacement);
            }

        }
    }

    
}
    
