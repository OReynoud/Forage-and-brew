using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterInteractController : MonoBehaviour
{
    public IngredientToCollectBehaviour CurrentIngredientToCollectBehaviour { get; private set; }

    public CollectedIngredientBehaviour CurrentCollectedIngredientBehaviour { get; private set; }
    public List<CollectedIngredientStack> collectedIngredientStack = new();

    public class CollectedIngredientStack
    {
        public CollectedIngredientBehaviour ingredient { get; set; }
        public bool isPickedUp { get; set; }

        public CollectedIngredientStack(CollectedIngredientBehaviour Ingredient)
        {
            ingredient = Ingredient;
            isPickedUp = false;
        }
    }

    public BedBehaviour CurrentNearBed { get; set; }
    public CauldronBehaviour CurrentNearCauldron { get; set; }
    public bool AreHandsFull { get; private set; }

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

    public void SetNewCollectedIngredient(CollectedIngredientBehaviour newCollectedIngredientBehaviour)
    {
        if (CurrentCollectedIngredientBehaviour)
        {
            CurrentCollectedIngredientBehaviour.DisableGrab();
        }

        CurrentCollectedIngredientBehaviour = newCollectedIngredientBehaviour;
    }

    public void Interact()
    {
        if (CurrentNearCauldron && collectedIngredientStack.Count > 0)
        {
            CurrentNearCauldron.DisableInteract(true);
            ShoveStackInCauldron();
        }
        if (CurrentIngredientToCollectBehaviour)
        {
            CharacterInputManager.Instance.DisableMoveInputs();
            CurrentIngredientToCollectBehaviour.DisableCollect();
            CollectHapticChallengeManager.Instance.StartCollectHapticChallenge(CurrentIngredientToCollectBehaviour);
            CurrentIngredientToCollectBehaviour = null;
        }
        else if (CurrentCollectedIngredientBehaviour)
        {
            AddToPile(CurrentCollectedIngredientBehaviour);
        }
        else if (CurrentNearBed && collectedIngredientStack.Count == 0)
        {
            CurrentNearBed.Sleep();
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


    void AddToPile(CollectedIngredientBehaviour ingredient)
    {
        if (collectedIngredientStack.Count > 0 && collectedIngredientStack.Count < maxStackSize)
        {
            if (collectedIngredientStack[0].ingredient.IngredientValuesSo.Type != ingredient.IngredientValuesSo.Type)
                return;
        }
        
        CurrentCollectedIngredientBehaviour.DisableGrab();
        CurrentCollectedIngredientBehaviour.GrabMethod(true);
        CurrentCollectedIngredientBehaviour.transform.SetParent(transform);
        collectedIngredientStack.Add(new CollectedIngredientStack(CurrentCollectedIngredientBehaviour));
        CurrentCollectedIngredientBehaviour = null;
        //CurrentCollectedIngredientBehaviour.transform.SetParent(stackPlacement);
        AreHandsFull = true;
    }

    void ShoveStackInCauldron()
    {
        for (int i = 0; i < collectedIngredientStack.Count; i++)
        {
            //collectedIngredientStack[i].ingredient.GrabMethod(false);
            collectedIngredientStack[i].ingredient.transform.SetParent(CauldronBehaviour.instance.transform);
            
            collectedIngredientStack[i].ingredient.originControl = collectedIngredientStack[i].ingredient.transform.position;
            collectedIngredientStack[i].ingredient.startControl = collectedIngredientStack[i].ingredient.originControl + Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.value, Random.Range(-1f, 1f));
            collectedIngredientStack[i].ingredient.endControl = Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.value, Random.Range(-1f, 1f));
            collectedIngredientStack[i].ingredient.isPutInCauldron = true;
            CauldronBehaviour.instance.Ingredients.Add(collectedIngredientStack[i].ingredient.IngredientValuesSo);
        }
        collectedIngredientStack.Clear();
        AreHandsFull = false;
    }

    private void FixedUpdate()
    {
        UpdateStack();
    }

    float clampedDisplacement;

    void UpdateStack()
    {
        for (var i = 0; i < collectedIngredientStack.Count; i++)
        {
            clampedDisplacement = Mathf.Clamp(rb.linearVelocity.magnitude, 0, stackDisplacementClamp);
            var ingredient = collectedIngredientStack[i].ingredient;

            if (collectedIngredientStack[i].isPickedUp)
            {
                //y lerp
                ingredient.transform.localPosition = Vector3.Lerp(ingredient.transform.localPosition,
                    new Vector3(ingredient.transform.localPosition.x, ingredient.stackHeight * i,
                        ingredient.transform.localPosition.z), pickupLerp);
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
                new Vector3(ingredient.transform.position.x, stackPlacement.position.y + ingredient.stackHeight * i,
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
    
