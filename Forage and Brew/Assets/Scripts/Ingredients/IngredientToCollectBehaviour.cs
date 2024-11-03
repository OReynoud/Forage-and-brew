using UnityEngine;

public class IngredientToCollectBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private IngredientToCollectGlobalValuesSo ingredientToCollectGlobalValuesSo;
    [SerializeField] private SphereCollider collectTrigger;
    
    [Header("UI")]
    [SerializeField] private GameObject collectInputCanvasGameObject;


    private void EnableCollect()
    {
        collectInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableCollect()
    {
        collectInputCanvasGameObject.SetActive(false);
    }
    

    #region Trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.SetNewIngredientToCollect(this);
            EnableCollect();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.CurrentIngredientToCollectBehaviour == this)
        {
            characterInteractController.SetNewIngredientToCollect(null);
        }
    }

    #endregion

    
    #region Gizmos

    private void OnDrawGizmos()
    {
        if (ingredientToCollectGlobalValuesSo && collectTrigger)
        {
            collectTrigger.radius = ingredientToCollectGlobalValuesSo.CollectRadius;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, collectTrigger.radius);
        }
    }

    #endregion
}
