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
        [field:SerializeField] [field:ReadOnly] public StackableItem StackableItem { get; set; }
        [field:SerializeField] [field:ReadOnly] public bool isPickedUp { get; set; }

        public CollectedStack(StackableItem stackableItem)
        {
            StackableItem = stackableItem;
            isPickedUp = false;
        }
    }
    
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<StackableItem> CurrentStackableBehaviours { get; private set; } = new();

    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public BedBehaviour CurrentNearBed { get; set; }
    
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public MailBoxBehaviour CurrentNearMailBoxBehaviour { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public BinBehaviour CurrentNearBin { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public CauldronBehaviour CurrentNearCauldron { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public BellowsBehaviour CurrentNearBellows { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public ChoppingCountertopBehaviour CurrentNearChoppingCountertop { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public GrindingCountertopBehaviour CurrentNearGrindingCountertop { get; set; }
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<IngredientBasketBehaviour> CurrentNearIngredientBaskets { get; set; } = new();
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public List<PotionCrateBehaviour> CurrentNearPotionBaskets { get; set; } = new();
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public PotionEnsembleBehaviour CurrentNearPotionEnsemble { get; set; } = new();
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public GateBehaviour CurrentNearChargedGate { get; set; }
    
    
    [field:Foldout("Debug")][field:SerializeField] [field:ReadOnly] public ICinematicInteraction CurrentNearCinematicInteraction { get; set; }
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
    
    [SerializeField] private Vector3 choppingOffset = new(0f, 1.3f, -0.05f);
    [SerializeField] private Vector3 grindingOffset = new(0f, 1.3f, 0.1f);
    [SerializeField] private Vector3 potionBasketOffset = new(0f, 1.5f, 0f);
    [SerializeField] private Vector3 binOffset = new(0f, 1f, 0f);
    
    // Animator Hashes
    private static readonly int DoThrow = Animator.StringToHash("DoThrow");
    private static readonly int DoNo = Animator.StringToHash("DoNo");
    

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    public void AddNewCollectedStackable(StackableItem newStackableItem)
    {
        if (CurrentStackableBehaviours.Count > 0)
        {
            CurrentStackableBehaviours[^1].DisableGrab();
        }

        CurrentStackableBehaviours.Add(newStackableItem);
    }

    public void RemoveCollectedStackable(StackableItem newStackableItem)
    {
        CurrentStackableBehaviours.Remove(newStackableItem);
        
        if (CurrentStackableBehaviours.Count > 0)
        {
            CurrentStackableBehaviours[^1].EnableGrab();
        }
    }
    

    public void Interact()
    {
        if (CurrentNearCauldron && collectedStack.Count > 0 && collectedStack[0].StackableItem is CollectedIngredientBehaviour)
        {
            CurrentNearCauldron.DisableInteract(true);
            ShoveStackInTarget(CurrentNearCauldron.transform, CurrentNearCauldron);
            CharacterAnimManager.instance.animator.SetTrigger(DoThrow);
        }
        else if (CurrentNearBin && collectedStack.Count > 0 && collectedStack[0].StackableItem is CollectedPotionBehaviour)
        {
            CurrentNearBin.DisableInteract();
            ShoveStackInTarget(CurrentNearBin.transform, CurrentNearBin, binOffset);
            CharacterAnimManager.instance.animator.SetTrigger(DoThrow);
        }
        else if (CurrentStackableBehaviours.Count > 0)
        {
            AddToPile(CurrentStackableBehaviours[^1]);
        }
        else if (CurrentNearBellows && !CurrentNearBellows.Unlocked && CurrentNearBellows.CanPurchase && collectedStack.Count == 0)
        {
            CurrentNearBellows.PurchaseItem();
        }
        else if (CurrentNearChoppingCountertop && !CurrentNearChoppingCountertop.Unlocked && CurrentNearChoppingCountertop.CanPurchase && collectedStack.Count == 0)
        {
            CurrentNearChoppingCountertop.PurchaseItem();
        }
        else if (CurrentNearGrindingCountertop && !CurrentNearGrindingCountertop.Unlocked && CurrentNearGrindingCountertop.CanPurchase && collectedStack.Count == 0)
        {
            CurrentNearGrindingCountertop.PurchaseItem();
        }
        else if (CurrentNearChargedGate && !CurrentNearChargedGate.Unlocked && collectedStack.Count == 0)
        {
            CurrentNearChargedGate.Purchase();
        }
        else if (CurrentNearPotionEnsemble && collectedStack.Count > 0 && collectedStack[0].StackableItem is CollectedPotionBehaviour)
        {
            ManagePotionEnsemble();
        }
        else if (CurrentNearPotionBaskets.Count > 0 && collectedStack.Count > 0 && collectedStack[0].StackableItem is CollectedPotionBehaviour)
        {
            ChoosePotionBasket();
            CharacterAnimManager.instance.animator.SetTrigger(DoThrow);
        }
        else if (CurrentNearIngredientBaskets.Count > 0)
        {
            ChooseIngredientBasket();
        }
        else if (CurrentNearBed && collectedStack.Count == 0)
        {
            CurrentNearBed.Sleep();
        }
        else if (CurrentNearMailBoxBehaviour)
        {
            CurrentNearMailBoxBehaviour.ShowLetters();
            // Debug.Log("Check letters");
        }
        else if (CurrentNearCinematicInteraction != null)
        {
            switch (CurrentNearCinematicInteraction)
            {
                case CodexPickUpBehaviour codex:
                    codex.StartInteraction();
                    break;
                case ObjectToSitBehaviour couch:
                    couch.StartInteraction();
                    break;
            }
        }
    }

    public void Cancel()
    {
        if (collectedStack.Count <= 0)
        {
            if (CurrentNearCinematicInteraction != null)
            {
                switch (CurrentNearCinematicInteraction)
                {
                    case ObjectToSitBehaviour couch:
                        couch.CancelCouch();
                        break;
                }
            }
            return;
        }

        CharacterAnimManager.instance.animator.SetTrigger(DoThrow);
        
        if (CurrentNearIngredientBaskets.Count > 0)
        {
            foreach (IngredientBasketBehaviour ingredientBasket in CurrentNearIngredientBaskets)
            {
                if (ingredientBasket.ingredient != ((CollectedIngredientBehaviour)collectedStack[0].StackableItem).IngredientValuesSo) continue;
                    
                ShoveStackInTarget(ingredientBasket.transform, ingredientBasket);
                break;
            }
                
            foreach (IngredientBasketBehaviour ingredientBasket in CurrentNearIngredientBaskets)
            {
                ingredientBasket.DoesNeedToCheckAvailability = true;
            }
        }
        else
        {
            int length = collectedStack.Count;

            for (int i = 0; i < length; i++)
            {
                collectedStack[0].StackableItem.GrabMethod(false);
                collectedStack[0].StackableItem.GetTransform().SetParent(null);
                collectedStack.RemoveAt(0);
            }

            AreHandsFull = false;
        }
    }


    private void ManagePotionEnsemble()
    {
        PotionValuesSo potion = ((CollectedPotionBehaviour)collectedStack[0].StackableItem).PotionValuesSo;

        if (CurrentNearPotionEnsemble.CheckPotion(potion))
        {
            CurrentNearPotionEnsemble.DisableInteract();
            ShoveStackInTarget(CurrentNearPotionEnsemble.transform, CurrentNearPotionEnsemble,
                CurrentNearPotionEnsemble.GetRightTransformLocalPosition(potion));
            CharacterAnimManager.instance.animator.SetTrigger(DoThrow);
        }
        else
        {
            CharacterAnimManager.instance.animator.SetTrigger(DoNo);
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
                if (CurrentNearIngredientBaskets[index].IngredientCount == 0) continue;
                
                if ((CollectedIngredientBehaviour)collectedStack[0].StackableItem &&
                    ((CollectedIngredientBehaviour)collectedStack[0].StackableItem).IngredientValuesSo == CurrentNearIngredientBaskets[index].ingredient)
                {
                    AddToPile(CurrentNearIngredientBaskets[index].InstantiateCollectedIngredient());
                    break;
                }
            }
            
            for (index = 0; index < CurrentNearIngredientBaskets.Count; index++)
            {
                CurrentNearIngredientBaskets[index].DoesNeedToCheckAvailability = true;
            }

            return;
        }

        (int index, float distance) lowestDistance = (-1, float.MaxValue);
        
        for (int i = 0; i < CurrentNearIngredientBaskets.Count; i++)
        {
            if (CurrentNearIngredientBaskets[i].IngredientCount == 0) continue;
            
            float distance = Vector3.Distance(transform.position, CurrentNearIngredientBaskets[i].transform.position);
            
            if (distance < lowestDistance.distance)
            {
                lowestDistance = (i, distance);
            }
        }

        if (lowestDistance.index < 0) return;
        
        AddToPile(CurrentNearIngredientBaskets[lowestDistance.index].InstantiateCollectedIngredient());
            
        for (int i = 0; i < CurrentNearIngredientBaskets.Count; i++)
        {
            CurrentNearIngredientBaskets[i].DoesNeedToCheckAvailability = true;
        }
    }
    
    private void ChoosePotionBasket()
    {
        (int index, float distance) lowestDistance = (-1, float.MaxValue);
        
        for (int i = 0; i < CurrentNearPotionBaskets.Count; i++)
        {
            if (CurrentNearPotionBaskets[i].IsFulfilled) continue;
            
            if (!CurrentNearPotionBaskets[i].CheckPotion(((CollectedPotionBehaviour)collectedStack[0].StackableItem).PotionValuesSo)) continue;
            
            float distance = Vector3.Distance(transform.position, CurrentNearPotionBaskets[i].transform.position);
            
            if (distance < lowestDistance.distance)
            {
                lowestDistance = (i, distance);
            }
        }
        
        if (lowestDistance.index < 0) return;

        ShoveStackInTarget(CurrentNearPotionBaskets[lowestDistance.index].transform,
            CurrentNearPotionBaskets[lowestDistance.index]);

        foreach (PotionCrateBehaviour potionBasket in CurrentNearPotionBaskets)
        {
            potionBasket.DoesNeedToCheckAvailability = true;
        }
    }
    
    public void DropIngredientsInChoppingCountertop()
    {
        if (!CurrentNearChoppingCountertop || collectedStack.Count == 0 ||
            collectedStack[0].StackableItem is not CollectedIngredientBehaviour collectedIngredientBehaviour ||
            collectedIngredientBehaviour.CookedForm is ChoppingHapticChallengeListSo) return;
        
        CurrentNearChoppingCountertop.DisableInteract();
        ShoveStackInTarget(CurrentNearChoppingCountertop.transform, CurrentNearChoppingCountertop, choppingOffset);
        
        ChoppingHapticChallengeManager.Instance.StartChoppingChallenge();
    }
    
    public void DropIngredientsInGrindingCountertop()
    {
        if (!CurrentNearGrindingCountertop || collectedStack.Count == 0 ||
            collectedStack[0].StackableItem is not CollectedIngredientBehaviour collectedIngredientBehaviour ||
            collectedIngredientBehaviour.CookedForm is GrindingHapticChallengeSo) return;
        
        CurrentNearGrindingCountertop.DisableInteract();
        ShoveStackInTarget(CurrentNearGrindingCountertop.transform, CurrentNearGrindingCountertop, grindingOffset);
        
        GrindingHapticChallengeManager.Instance.StartGrindingChallenge();
    }


    public void AddToPile(StackableItem stackableItem)
    {
        if (collectedStack.Count > 0 && collectedStack.Count < maxStackSize)
        {
            if (collectedStack[0].StackableItem.GetStackableValuesSo() != stackableItem.GetStackableValuesSo())
                return;
        }
        
        stackableItem.GrabMethod(true);
        stackableItem.GetTransform().SetParent(transform);
        collectedStack.Add(new CollectedStack(stackableItem));
        RemoveCollectedStackable(stackableItem);
        
        AreHandsFull = true;
    }

    private void ShoveStackInTarget(Transform targetTransform, IIngredientAddable targetBehaviour, Vector3 offset = default)
    {
        for (int i = 0; i < collectedStack.Count; i++)
        {
            collectedStack[i].StackableItem.GetTransform().SetParent(targetTransform);
            targetBehaviour.AddIngredient((CollectedIngredientBehaviour)collectedStack[i].StackableItem);
            collectedStack[i].StackableItem.DropInTarget(targetBehaviour.EndPoint,targetBehaviour.UseEndPoint , offset);
        }
        
        collectedStack.Clear();
        AreHandsFull = false;
    }

    private void ShoveStackInTarget(Transform targetTransform, IPotionAddable targetBehaviour, Vector3 offset = default)
    {
        for (int i = 0; i < collectedStack.Count; i++)
        {
            collectedStack[i].StackableItem.GetTransform().SetParent(targetTransform);
            targetBehaviour.AddPotion((CollectedPotionBehaviour)collectedStack[i].StackableItem);
            collectedStack[i].StackableItem.DropInTarget(targetBehaviour.EndPoint, targetBehaviour.UseEndPoint, offset);
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
            var stackable = collectedStack[i].StackableItem;

            if (collectedStack[i].isPickedUp)
            {
                //y lerp
                stackable.GetTransform().localPosition = Vector3.Lerp(stackable.GetTransform().localPosition,
                    new Vector3(stackable.GetTransform().localPosition.x, stackable.StackHeight * i,
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
                new Vector3(stackable.GetTransform().position.x, stackPlacement.position.y + stackable.StackHeight * i,
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
    
