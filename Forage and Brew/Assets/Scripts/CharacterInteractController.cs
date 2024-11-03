using UnityEngine;

public class CharacterInteractController : MonoBehaviour
{
    public IngredientToCollectBehaviour CurrentIngredientToCollectBehaviour { get; private set; }


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
            Debug.Log("Interacting with " + CurrentIngredientToCollectBehaviour.name);
        }
    }
}
