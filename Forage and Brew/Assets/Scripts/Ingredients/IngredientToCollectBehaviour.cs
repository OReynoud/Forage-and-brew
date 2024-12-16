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

    [Header("Ingredient Types")]
    [SerializeField] private IngredientType scythingIngredientType = IngredientType.Herb;
    [SerializeField] private IngredientType unearthingIngredientType = IngredientType.Mushroom;
    [SerializeField] private IngredientType scrapingIngredientType = IngredientType.Moss;
    [SerializeField] private IngredientType harvestIngredientType = IngredientType.Berry;
    
    [Header("UI")]
    [SerializeField] private bool isUiRight;
    [SerializeField] private GameObject collectInputCanvasGameObject;
    [SerializeField] private GameObject unearthingLeftInputLeftGameObject;
    [SerializeField] private GameObject unearthingLeftArrowLeftGameObject;
    [SerializeField] private GameObject unearthingLeftReleaseLeftGameObject;
    [SerializeField] private GameObject unearthingRightInputLeftGameObject;
    [SerializeField] private GameObject unearthingRightArrowLeftGameObject;
    [SerializeField] private GameObject unearthingRightReleaseLeftGameObject;
    [SerializeField] private GameObject unearthingLeftInputRightGameObject;
    [SerializeField] private GameObject unearthingLeftArrowRightGameObject;
    [SerializeField] private GameObject unearthingLeftReleaseRightGameObject;
    [SerializeField] private GameObject unearthingRightInputRightGameObject;
    [SerializeField] private GameObject unearthingRightArrowRightGameObject;
    [SerializeField] private GameObject unearthingRightReleaseRightGameObject;
    [SerializeField] private GameObject harvestInputLeftGameObject;
    [SerializeField] private GameObject harvestArrowLeftGameObject;
    [SerializeField] private GameObject harvestReleaseLeftGameObject;
    [SerializeField] private GameObject harvestInputRightGameObject;
    [SerializeField] private GameObject harvestArrowRightGameObject;
    [SerializeField] private GameObject harvestReleaseRightGameObject;
    

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
        
        // UI
        DisableCanvas();

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
        
        if (IngredientValuesSo.Type == unearthingIngredientType)
        {
            if (isUiRight)
            {
                unearthingLeftInputRightGameObject.SetActive(true);
                unearthingLeftArrowRightGameObject.SetActive(true);
                unearthingRightInputRightGameObject.SetActive(true);
                unearthingRightArrowRightGameObject.SetActive(true);
            }
            else
            {
                unearthingLeftInputLeftGameObject.SetActive(true);
                unearthingLeftArrowLeftGameObject.SetActive(true);
                unearthingRightInputLeftGameObject.SetActive(true);
                unearthingRightArrowLeftGameObject.SetActive(true);
            }
        }
        else if (IngredientValuesSo.Type == harvestIngredientType)
        {
            if (isUiRight)
            {
                harvestInputRightGameObject.SetActive(true);
                harvestArrowRightGameObject.SetActive(true);
            }
            else
            {
                harvestInputLeftGameObject.SetActive(true);
                harvestArrowLeftGameObject.SetActive(true);
            }
        }
    }
    
    public void DisableCollect()
    {
        DisableCanvas();
    }
    
    
    private void DisableCanvas()
    {
        collectInputCanvasGameObject.SetActive(false);
        unearthingLeftInputLeftGameObject.SetActive(false);
        unearthingLeftArrowLeftGameObject.SetActive(false);
        unearthingLeftReleaseLeftGameObject.SetActive(false);
        unearthingRightInputLeftGameObject.SetActive(false);
        unearthingRightArrowLeftGameObject.SetActive(false);
        unearthingRightReleaseLeftGameObject.SetActive(false);
        unearthingLeftInputRightGameObject.SetActive(false);
        unearthingLeftArrowRightGameObject.SetActive(false);
        unearthingLeftReleaseRightGameObject.SetActive(false);
        unearthingRightInputRightGameObject.SetActive(false);
        unearthingRightArrowRightGameObject.SetActive(false);
        unearthingRightReleaseRightGameObject.SetActive(false);
        harvestInputLeftGameObject.SetActive(false);
        harvestArrowLeftGameObject.SetActive(false);
        harvestReleaseLeftGameObject.SetActive(false);
        harvestInputRightGameObject.SetActive(false);
        harvestArrowRightGameObject.SetActive(false);
        harvestReleaseRightGameObject.SetActive(false);
    }

    public void PressUnearthing()
    {
        if (isUiRight)
        {
            unearthingLeftArrowRightGameObject.SetActive(true);
            unearthingRightArrowRightGameObject.SetActive(true);
            unearthingLeftReleaseRightGameObject.SetActive(false);
            unearthingRightReleaseRightGameObject.SetActive(false);
        }
        else
        {
            unearthingLeftArrowLeftGameObject.SetActive(true);
            unearthingRightArrowLeftGameObject.SetActive(true);
            unearthingLeftReleaseLeftGameObject.SetActive(false);
            unearthingRightReleaseLeftGameObject.SetActive(false);
        }
    }

    public void ReleaseUnearthing()
    {
        if (isUiRight)
        {
            unearthingLeftArrowRightGameObject.SetActive(false);
            unearthingRightArrowRightGameObject.SetActive(false);
            unearthingLeftReleaseRightGameObject.SetActive(true);
            unearthingRightReleaseRightGameObject.SetActive(true);
        }
        else
        {
            unearthingLeftArrowLeftGameObject.SetActive(false);
            unearthingRightArrowLeftGameObject.SetActive(false);
            unearthingLeftReleaseLeftGameObject.SetActive(true);
            unearthingRightReleaseLeftGameObject.SetActive(true);
        }
    }
    
    public void ReleaseHarvest()
    {
        if (isUiRight)
        {
            harvestArrowRightGameObject.SetActive(false);
            harvestReleaseRightGameObject.SetActive(true);
        }
        else
        {
            harvestArrowLeftGameObject.SetActive(false);
            harvestReleaseLeftGameObject.SetActive(true);
        }
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
