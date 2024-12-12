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
        if (CurrentNearBaskets.Count > 0)
        {
            ChooseBasket();
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
            if (CurrentNearBaskets.Count > 0)
            {
                ShoveStackInTarget(CurrentNearBaskets[0].transform, CurrentNearBaskets[0]);
                CurrentNearBaskets[0].EnableInteract();
                CurrentNearBaskets[0].DisableCancel();
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
    }

    
    private void ChooseBasket()
    {
        if (collectedStack.Count >= maxStackSize) return;

        if (collectedStack.Count > 0)
        {
            int index;
            
            for (index = 0; index < CurrentNearBaskets.Count; index++)
            {
                if (CurrentNearBaskets[index].IngredientCount == 0) return;
                
                if ((CollectedIngredientBehaviour)collectedStack[0].stackable &&
                    ((CollectedIngredientBehaviour)collectedStack[0].stackable).IngredientValuesSo == CurrentNearBaskets[index].ingredient)
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
    
