using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterInteractController : MonoBehaviour
{
    public IngredientToCollectBehaviour CurrentIngredientToCollectBehaviour { get; private set; }
    
    public CollectedIngredientBehaviour CurrentCollectedIngredientBehaviour { get; private set; }
    public Stack<CollectedIngredientBehaviour> CollectedIngredientStack = new Stack<CollectedIngredientBehaviour>();
    public bool IsNearBed { get; set; }
    public bool handsFull { get; set; }


    [BoxGroup("Collected ingredients stack variables")] public Transform stackPlacement;
    [BoxGroup("Collected ingredients stack variables")] [field: Min(0f)] public int maxStackSize;
    [BoxGroup("Collected ingredients stack variables")] [field: Min(0f)] public float timeToAddToStack;


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
        if (CurrentIngredientToCollectBehaviour)
        {
            CharacterInputManager.Instance.DisableMoveInputs();
            CurrentIngredientToCollectBehaviour.DisableCollect();
            HapticChallengeManager.Instance.StartHapticChallenge(CurrentIngredientToCollectBehaviour);
            CurrentIngredientToCollectBehaviour = null;
        }
        else if(CurrentCollectedIngredientBehaviour)
        {
            CurrentCollectedIngredientBehaviour.DisableGrab();

            AddToPile(CurrentCollectedIngredientBehaviour);
        }
        else if (IsNearBed && CollectedIngredientStack.Count == 0)
        {
            CharacterDontDestroyOnLoadManager.Instance.CurrentTimeOfDay = TimeOfDay.Daytime;
            Debug.Log("It's daytime now");
        } 
    }


    void AddToPile(CollectedIngredientBehaviour ingredient)
    {
        CollectedIngredientStack.Push(CurrentCollectedIngredientBehaviour);
        CurrentCollectedIngredientBehaviour.GrabMethod(true);
        CurrentCollectedIngredientBehaviour.transform.SetParent(transform);
    }
}
