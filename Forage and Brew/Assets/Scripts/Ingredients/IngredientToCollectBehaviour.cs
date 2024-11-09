using UnityEngine;

public class IngredientToCollectBehaviour : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private IngredientToCollectGlobalValuesSo ingredientToCollectGlobalValuesSo;
    [SerializeField] private IngredientToCollectSpawnManager ingredientToCollectSpawnManager;
    [SerializeField] private SphereCollider collectTrigger;
    [SerializeField] private Transform meshParentTransform;
    
    [Header("Data")]
    [field: SerializeField] public SpawnLocation SpawnLocation { get; private set; }
    
    [Header("UI")]
    [SerializeField] private GameObject collectInputCanvasGameObject;

    public IngredientValuesSo IngredientValuesSo { get; set; }
    

    private void Awake()
    {
        ingredientToCollectSpawnManager.IngredientToCollectBehaviours.Add(this);
    }

    private void Start()
    {
        collectInputCanvasGameObject.SetActive(false);
    }
    
    
    public void SpawnMesh()
    {
        Instantiate(IngredientValuesSo.MeshGameObject, meshParentTransform);
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
