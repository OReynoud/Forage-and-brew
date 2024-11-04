using UnityEngine;

public class CharacterInteractController : MonoBehaviour
{
    public IngredientToCollectBehaviour CurrentIngredientToCollectBehaviour { get; private set; }
    
    private bool _isHapticChallengeActive;


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
            _isHapticChallengeActive = true;
        }
        else if (_isHapticChallengeActive)
        {
            HapticChallengeManager.Instance.StopHapticChallenge();
            CharacterInputManager.Instance.EnableMoveInputs();
        }
    }
}
