using UnityEngine;

public class CharacterInteractController : MonoBehaviour
{
    public IngredientToCollectBehaviour CurrentIngredientToCollectBehaviour { get; private set; }
    public bool IsNearBed { get; set; }


    public void SetNewIngredientToCollect(IngredientToCollectBehaviour newIngredientToCollectBehaviour)
    {
        if (CurrentIngredientToCollectBehaviour)
        {
            CurrentIngredientToCollectBehaviour.DisableCollect();
        }
        
        CurrentIngredientToCollectBehaviour = newIngredientToCollectBehaviour;
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
        else if (IsNearBed)
        {
            CharacterDontDestroyOnLoadManager.Instance.CurrentTimeOfDay = TimeOfDay.Daytime;
            Debug.Log("It's daytime now");
        }
    }
}
