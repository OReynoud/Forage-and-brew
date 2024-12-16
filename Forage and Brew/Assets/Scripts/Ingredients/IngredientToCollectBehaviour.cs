using UnityEngine;

public class IngredientToCollectBehaviour : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private IngredientToCollectGlobalValuesSo ingredientToCollectGlobalValuesSo;
    [SerializeField] private IngredientToCollectSpawnManager ingredientToCollectSpawnManager;
    [SerializeField] private SphereCollider collectTrigger;
    [SerializeField] private Transform meshParentTransform;
    [field: SerializeField] public IngredientValuesSo IngredientValuesSo { get; set; }
    
    [Header("Data")]
    [field: SerializeField] public SpawnLocation SpawnLocation { get; private set; }
    
    [Header("UI")]
    [SerializeField] private GameObject collectInputCanvasGameObject;
    

    private void Awake()
    {
        if (ingredientToCollectSpawnManager)
        {
            ingredientToCollectSpawnManager.IngredientToCollectBehaviours.Add(this);
        }
        else
        {
            Debug.LogWarning("IngredientToCollectSpawnManager is not assigned in " + name + ".\n" +
                             "The ingredient won't spawn according to the cycles and the spawn locations.");
        }
    }

    private void Start()
    {
        for (int i = 0; i < meshParentTransform.childCount; i++)
        {
            Destroy(meshParentTransform.GetChild(i).gameObject);
        }
        
        collectInputCanvasGameObject.SetActive(false);

        if (!ingredientToCollectSpawnManager && ingredientToCollectGlobalValuesSo)
        {
            SpawnMesh();
        }
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
        if (other.TryGetComponent(out CollectHapticChallengeManager collectHapticChallengeManager))
        {
            collectHapticChallengeManager.CurrentIngredientToCollectBehaviours.Add(this);
            EnableCollect();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CollectHapticChallengeManager collectHapticChallengeManager) &&
            collectHapticChallengeManager.CurrentIngredientToCollectBehaviours.Contains(this))
        {
            collectHapticChallengeManager.CurrentIngredientToCollectBehaviours.Remove(this);
            DisableCollect();
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
