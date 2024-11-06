using UnityEngine;

public class IngredientToCollectBehaviour : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private IngredientToCollectGlobalValuesSo ingredientToCollectGlobalValuesSo;
    [field: SerializeField] public IngredientValuesSo IngredientValuesSo { get; set; }
    [SerializeField] private SphereCollider collectTrigger;
    [SerializeField] private Transform meshParentTransform;
    
    [Header("UI")]
    [SerializeField] private GameObject collectInputCanvasGameObject;


    private void Start()
    {
        Instantiate(IngredientValuesSo.MeshGameObject, meshParentTransform);
        collectInputCanvasGameObject.SetActive(false);
    }


    private void EnableCollect()
    {
        collectInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableCollect()
    {
        collectInputCanvasGameObject.SetActive(false);
    }
    

    public void Collect()
    {
        GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Add(IngredientValuesSo);
        Destroy(gameObject);
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
