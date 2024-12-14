using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterInteractController : MonoBehaviour
{
    public static CharacterInteractController Instance { get; private set; }
    
    [field:Foldout("Debug")] [ReadOnly] public List<CollectedStack> collectedStack = new();

    [Serializable]
    public class CollectedStack
    {
        [field:SerializeField] [field:ReadOnly] public IStackable stackable { get; set; }
        [field:SerializeField] [field:ReadOnly] public bool isPickedUp { get; set; }

        public CollectedStack(IStackable Stackable)
        {
            stackable = Stackable;
            isPickedUp = false;
        }
    }
    
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public IngredientToCollectBehaviour CurrentIngredientToCollectBehaviour { get; private set; }

    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<IStackable> CurrentStackableBehaviours { get; private set; } = new();

    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public BedBehaviour CurrentNearBed { get; set; }
    
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public MailBox CurrentNearMailBox { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public CauldronBehaviour CurrentNearCauldron { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public ChoppingCountertopBehaviour CurrentNearChoppingCountertop { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<IngredientBasketBehaviour> CurrentNearIngredientBaskets { get; set; } = new();
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<PotionBasketBehaviour> CurrentNearPotionBaskets { get; set; } = new();
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

    public void AddNewCollectedStackable(IStackable newStackable)
    {
        if (CurrentStackableBehaviours.Count > 0)
        {
            CurrentStackableBehaviours[^1].DisableGrab();
        }

        CurrentStackableBehaviours.Add(newStackable);
    }

    public void RemoveCollectedStackable(IStackable newStackable)
    {
        CurrentStackableBehaviours.Remove(newStackable);
        
        if (CurrentStackableBehaviours.Count > 0)
        {
            CurrentStackableBehaviours[^1].EnableGrab();
        }
    }
    

    public void Interact()
    {
        if (CurrentNearPotionBaskets.Count > 0 && collectedStack.Count > 0)
        {
            ChoosePotionBasket();
        }
        else if (CurrentNearIngredientBaskets.Count > 0)
        {
            ChooseIngredientBasket();
        }
        else if (CurrentNearCauldron && collectedStack.Count > 0 && (CollectedIngredientBehaviour) collectedStack[0].stackable)
        {
            CurrentNearCauldron.DisableInteract(true);
            for (int i = 0; i < collectedStack.Count; i++)
            {
                GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Remove(
                    ((CollectedIngredientBehaviour)collectedStack[i].stackable).IngredientValuesSo);
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
        else if (CurrentStackableBehaviours.Count > 0)
        {
            AddToPile(CurrentStackableBehaviours[^1]);
        }
        else if (CurrentNearBed && collectedStack.Count == 0)
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
        if (collectedStack.Count > 0)
        {
            if (CurrentNearIngredientBaskets.Count > 0)
            {
                ShoveStackInTarget(CurrentNearIngredientBaskets[0].transform, CurrentNearIngredientBaskets[0]);
                CurrentNearIngredientBaskets[0].EnableInteract();
                CurrentNearIngredientBaskets[0].DisableCancel();
            }
            else
            {
                int length = collectedStack.Count;

                for (int i = 0; i < length; i++)
                {
                    collectedStack[0].stackable.GrabMethod(false);
                    collectedStack[0].stackable.GetTransform().SetParent(null);
                    collectedStack.RemoveAt(0);
                }

                AreHandsFull = false;
            }
        }
        else if (CurrentNearPotionBaskets.Count > 0 && collectedStack.Count == 0)
        {
            ChoosePotionBasket(true);
        }
    }

    
    private void ChooseIngredientBasket()
    {
        if (collectedStack.Count >= maxStackSize) return;

        if (collectedStack.Count > 0)
        {
            int index;
            
            for (index = 0; index < CurrentNearIngredientBaskets.Count; index++)
            {
                if (CurrentNearIngredientBaskets[index].IngredientCount == 0) return;
                
                if ((CollectedIngredientBehaviour)collectedStack[0].stackable &&
                    ((CollectedIngredientBehaviour)collectedStack[0].stackable).IngredientValuesSo == CurrentNearIngredientBaskets[index].ingredient)
                {
                    AddToPile(CurrentNearIngredientBaskets[index].InstantiateCollectedIngredient());
                    break;
                }
            }
            
            if (index != CurrentNearIngredientBaskets.Count)
            {
                if (CurrentNearIngredientBaskets[index].IngredientCount == 0)
                {
                    CurrentNearIngredientBaskets[index].DisableInteract();
                }
                
                IngredientBasketBehaviour item = CurrentNearIngredientBaskets[index];
                CurrentNearIngredientBaskets.RemoveAt(index);
                index = CurrentNearIngredientBaskets.Count;
                CurrentNearIngredientBaskets.Insert(CurrentNearIngredientBaskets.Count, item);
            }
            
            int length = CurrentNearIngredientBaskets.Count;
            
            for (int i = 0; i < length; i++)
            {
                if (i != index)
                {
                    CurrentNearIngredientBaskets[0].DisableInteract();
                    CurrentNearIngredientBaskets.RemoveAt(0);
                }
            }

            return;
        }

        (int index, float dotValue) largestDot = (0, -1);
        
        for (int i = 0; i < CurrentNearIngredientBaskets.Count; i++)
        {
            float dotValue = Vector3.Dot(transform.forward, CurrentNearIngredientBaskets[i].transform.position - transform.position);
            
            if (dotValue > largestDot.dotValue)
            {
                largestDot = (i, dotValue);
            }
        }
        
        AddToPile(CurrentNearIngredientBaskets[largestDot.index].InstantiateCollectedIngredient());
        CurrentNearIngredientBaskets[largestDot.index].EnableCancel();
        
        if (CurrentNearIngredientBaskets[largestDot.index].IngredientCount == 0)
        {
            CurrentNearIngredientBaskets[largestDot.index].DisableInteract();
        }

        IngredientBasketBehaviour ingredientBasket = CurrentNearIngredientBaskets[largestDot.index];
        CurrentNearIngredientBaskets.RemoveAt(largestDot.index);
        largestDot.index = CurrentNearIngredientBaskets.Count;
        CurrentNearIngredientBaskets.Insert(CurrentNearIngredientBaskets.Count, ingredientBasket);
        
        int count = CurrentNearIngredientBaskets.Count;
            
        for (int i = 0; i < count; i++)
        {
            if (i != largestDot.index)
            {
                CurrentNearIngredientBaskets[0].DisableInteract();
                CurrentNearIngredientBaskets.RemoveAt(0);
            }
        }
    }
    
    private void ChoosePotionBasket(bool hasToGrab = false)
    {
        (int index, float dotValue) largestDot = (0, -1);
        
        for (int i = 0; i < CurrentNearPotionBaskets.Count; i++)
        {
            float dotValue = Vector3.Dot(transform.forward, CurrentNearPotionBaskets[i].transform.position - transform.position);

            if (hasToGrab && !GameDontDestroyOnLoadManager.Instance.OrderPotions[CurrentNearPotionBaskets[i].OrderIndex]
                    [CurrentNearPotionBaskets[i].PotionBasketIndex]) continue;

            if (!hasToGrab && GameDontDestroyOnLoadManager.Instance.OrderPotions[CurrentNearPotionBaskets[i].OrderIndex]
                    [CurrentNearPotionBaskets[i].PotionBasketIndex]) continue;
            
            if (dotValue > largestDot.dotValue)
            {
                largestDot = (i, dotValue);
            }
        }

        if (hasToGrab)
        {
            AddToPile(CurrentNearPotionBaskets[largestDot.index].InstantiateCollectedPotion());
            CurrentNearPotionBaskets[largestDot.index].DisableCancel();
            CurrentNearPotionBaskets[largestDot.index].EnableInteract();
        }
        else
        {
            ShoveStackInTarget(CurrentNearPotionBaskets[0].transform, CurrentNearPotionBaskets[0]);
            CurrentNearPotionBaskets[largestDot.index].EnableCancel();
            CurrentNearPotionBaskets[largestDot.index].DisableInteract();
        }
    }
    
    
    public void DropIngredientsInChoppingCountertop()
    {
        if (!CurrentNearChoppingCountertop || collectedStack.Count == 0) return;
        
        CurrentNearChoppingCountertop.DisableInteract();
        ShoveStackInTarget(CurrentNearChoppingCountertop.transform, CurrentNearChoppingCountertop, choppingOffset);
        
        ChoppingHapticChallengeManager.Instance.StartChoppingChallenge();
    }


    public void AddToPile(IStackable stackable)
    {
        if (collectedStack.Count > 0 && collectedStack.Count < maxStackSize)
        {
            if (collectedStack[0].stackable.GetStackableValuesSo() != stackable.GetStackableValuesSo())
                return;
        }
        
        stackable.GrabMethod(true);
        stackable.GetTransform().SetParent(transform);
        collectedStack.Add(new CollectedStack(stackable));
        RemoveCollectedStackable(stackable);
        
        AreHandsFull = true;
    }

    private void ShoveStackInTarget(Transform targetTransform, IIngredientAddable targetBehaviour, Vector3 offset = default)
    {
        for (int i = 0; i < collectedStack.Count; i++)
        {
            collectedStack[i].stackable.GetTransform().SetParent(targetTransform);
            collectedStack[i].stackable.DropInTarget(targetTransform, offset);
            if ((CollectedIngredientBehaviour)collectedStack[i].stackable)
            {
                targetBehaviour.AddIngredient((CollectedIngredientBehaviour)collectedStack[i].stackable);
            }
        }
        
        collectedStack.Clear();
        AreHandsFull = false;
    }

    private void ShoveStackInTarget(Transform targetTransform, IPotionAddable targetBehaviour, Vector3 offset = default)
    {
        for (int i = 0; i < collectedStack.Count; i++)
        {
            collectedStack[i].stackable.GetTransform().SetParent(targetTransform);
            collectedStack[i].stackable.DropInTarget(targetTransform, offset);
            if ((CollectedPotionBehaviour)collectedStack[i].stackable)
            {
                targetBehaviour.AddPotion((CollectedPotionBehaviour)collectedStack[i].stackable);
            }
        }
        
        collectedStack.Clear();
        AreHandsFull = false;
    }

    private void FixedUpdate()
    {
        DisplaceStack();
    }

    float clampedDisplacement;

    void DisplaceStack()
    {
        for (var i = 0; i < collectedStack.Count; i++)
        {
            clampedDisplacement = Mathf.Clamp(rb.linearVelocity.magnitude, 0, stackDisplacementClamp);
            var stackable = collectedStack[i].stackable;

            if (collectedStack[i].isPickedUp)
            {
                //y lerp
                stackable.GetTransform().localPosition = Vector3.Lerp(stackable.GetTransform().localPosition,
                    new Vector3(stackable.GetTransform().localPosition.x, stackable.GetStackHeight() * i,
                        stackable.GetTransform().localPosition.z), pickupLerp);
                
                //x and z lerp
                if (i == 0)
                {
                    stackable.GetTransform().localPosition = Vector3.Lerp(stackable.GetTransform().localPosition,
                        new Vector3(0, stackable.GetTransform().localPosition.y, 0), stackLerp);
                }
                else
                {
                    stackable.GetTransform().localPosition = Vector3.Lerp(stackable.GetTransform().localPosition,
                        new Vector3(0, stackable.GetTransform().localPosition.y, -clampedDisplacement * i), stackLerp);
                }

                continue;
            }


            //y lerp
            stackable.GetTransform().position = Vector3.Lerp(stackable.GetTransform().position,
                new Vector3(stackable.GetTransform().position.x, stackPlacement.position.y + stackable.GetStackHeight() * i,
                    stackable.GetTransform().position.z), pickupLerp);

            //x and z lerp
            if (Vector2.Distance(new Vector2(stackable.GetTransform().position.x, stackable.GetTransform().position.z),
                    new Vector2(stackPlacement.position.x, stackPlacement.position.z)) > stackRadius)
            {
                stackable.GetTransform().position = Vector3.Lerp(stackable.GetTransform().position,
                    new Vector3(stackPlacement.position.x, stackable.GetTransform().position.y, stackPlacement.position.z),
                    pickupLerp);
            }
            else if (!collectedStack[i].isPickedUp)
            {
                collectedStack[i].isPickedUp = true;
                stackable.GetTransform().SetParent(stackPlacement);
            }

        }
    }
}
    
