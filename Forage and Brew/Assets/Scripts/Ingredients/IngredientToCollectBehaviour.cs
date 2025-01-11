using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IngredientToCollectBehaviour : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private IngredientToCollectGlobalValuesSo ingredientToCollectGlobalValuesSo;
    [field: SerializeField] public IngredientToCollectVfxManagerBehaviour IngredientToCollectVfxManagerBehaviour { get; private set; }
    [SerializeField] private IngredientToCollectSpawnManager ingredientToCollectSpawnManager;
    [SerializeField] private SphereCollider collectTrigger;
    [SerializeField] private Transform meshParentTransform;
    [field: SerializeField] public IngredientValuesSo IngredientValuesSo { get; set; }
    
    [Header("Data")]
    [field: SerializeField] public SpawnLocation SpawnLocation { get; private set; }

    [Header("Ingredient Types")]
    [SerializeField] private IngredientTypeSo scythingIngredientType;
    [SerializeField] private IngredientTypeSo unearthingIngredientType;
    [SerializeField] private IngredientTypeSo scrapingIngredientType;
    [SerializeField] private IngredientTypeSo harvestIngredientType;
    
    [Header("Obtaining Feedback")]
    [SerializeField] private RectTransform obtainingFeedbackRectTransform;
    [SerializeField] private Image obtainingFeedbackImage;
    private Vector2 _obtainingFeedbackStartPosition;
    private bool _isPlayingObtainingFeedback;
    private float _obtainingFeedbackCurrentTime;
    
    [Header("UI")]
    [SerializeField] private bool isUiRight;
    [SerializeField] private GameObject collectInputCanvasGameObject;
    [SerializeField] private GameObject scythingInputLeftGameObject;
    [SerializeField] private GameObject scythingInputRightGameObject;
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
    [SerializeField] private GameObject scrapingInputLeftGameObject;
    [SerializeField] private GameObject scrapingInputRightGameObject;
    [SerializeField] private GameObject harvestInputLeftGameObject;
    [SerializeField] private GameObject harvestArrowLeftGameObject;
    [SerializeField] private GameObject harvestReleaseLeftGameObject;
    [SerializeField] private GameObject harvestInputRightGameObject;
    [SerializeField] private GameObject harvestArrowRightGameObject;
    [SerializeField] private GameObject harvestReleaseRightGameObject;
    public bool DoesNeedToShowUi { get; set; }
    private float _currentTriggerTime;
    
    // Unity Events
    public UnityEvent OnNewIngredientCollected { get; private set; } = new();
    

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
        obtainingFeedbackRectTransform.gameObject.SetActive(false);
        _obtainingFeedbackStartPosition = obtainingFeedbackRectTransform.anchoredPosition;

        if (GameDontDestroyOnLoadManager.Instance.DayPassed == 0)
        {
            DoesNeedToShowUi = true;
        }

        if (!ingredientToCollectSpawnManager && ingredientToCollectGlobalValuesSo)
        {
            SpawnMesh();
        }
    }

    private void Update()
    {
        UpdateObtainingFeedback();
    }


    public void SpawnMesh()
    {
        Instantiate(IngredientValuesSo.MeshGameObject, meshParentTransform);
    }


    private void EnableCollect()
    {
        collectInputCanvasGameObject.SetActive(true);
        
        if (IngredientValuesSo.Type == scythingIngredientType)
        {
            if (isUiRight)
            {
                scythingInputRightGameObject.SetActive(true);
            }
            else
            {
                scythingInputLeftGameObject.SetActive(true);
            }
        }
        else if (IngredientValuesSo.Type == unearthingIngredientType)
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
        else if (IngredientValuesSo.Type == scrapingIngredientType)
        {
            if (isUiRight)
            {
                scrapingInputRightGameObject.SetActive(true);
            }
            else
            {
                scrapingInputLeftGameObject.SetActive(true);
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
        scythingInputLeftGameObject.SetActive(false);
        scythingInputRightGameObject.SetActive(false);
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
        scrapingInputLeftGameObject.SetActive(false);
        scrapingInputRightGameObject.SetActive(false);
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
    
    
    public void PlayObtainingFeedback()
    {
        _isPlayingObtainingFeedback = true;
        obtainingFeedbackRectTransform.gameObject.SetActive(true);
        obtainingFeedbackRectTransform.anchoredPosition = _obtainingFeedbackStartPosition;
        obtainingFeedbackImage.color = new Color(obtainingFeedbackImage.color.r, obtainingFeedbackImage.color.g,
            obtainingFeedbackImage.color.b, ingredientToCollectGlobalValuesSo.ObtainingFeedbackFadeCurve.Evaluate(0f));
        collectInputCanvasGameObject.SetActive(true);
    }
    
    private void UpdateObtainingFeedback()
    {
        if (!_isPlayingObtainingFeedback) return;
        
        _obtainingFeedbackCurrentTime += Time.deltaTime;
        
        obtainingFeedbackRectTransform.anchoredPosition = Vector2.Lerp(_obtainingFeedbackStartPosition,
            _obtainingFeedbackStartPosition + ingredientToCollectGlobalValuesSo.ObtainingFeedbackDistance * Vector2.up,
            ingredientToCollectGlobalValuesSo.ObtainingFeedbackMoveCurve.Evaluate(_obtainingFeedbackCurrentTime /
                ingredientToCollectGlobalValuesSo.ObtainingFeedbackDuration));
        obtainingFeedbackImage.color = new Color(obtainingFeedbackImage.color.r, obtainingFeedbackImage.color.g,
            obtainingFeedbackImage.color.b, ingredientToCollectGlobalValuesSo.ObtainingFeedbackFadeCurve
                .Evaluate(_obtainingFeedbackCurrentTime / ingredientToCollectGlobalValuesSo.ObtainingFeedbackDuration));
        
        if (_obtainingFeedbackCurrentTime >= ingredientToCollectGlobalValuesSo.ObtainingFeedbackDuration)
        {
            obtainingFeedbackRectTransform.gameObject.SetActive(false);
            _isPlayingObtainingFeedback = false;
            _obtainingFeedbackCurrentTime = 0f;
            collectInputCanvasGameObject.SetActive(false);
        }
    }
    

    public void Collect()
    {
        GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Add(IngredientValuesSo);
        PinnedRecipe.instance.UpdateIngredientCounter();

        if (IsNewIngredient())
        {
            GameDontDestroyOnLoadManager.Instance.UnlockedIngredients.Add(IngredientValuesSo);
            OnNewIngredientCollected.Invoke();
        }
        
        DisableCollect();
        PlayObtainingFeedback();
        
        meshParentTransform.gameObject.SetActive(false);
        IngredientToCollectVfxManagerBehaviour.StopAllLunarCycleVfx();
        collectTrigger.enabled = false;
    }
    
    
    private bool IsNewIngredient()
    {
        return !GameDontDestroyOnLoadManager.Instance.UnlockedIngredients.Contains(IngredientValuesSo);
    }
    

    #region Trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CollectHapticChallengeManager collectHapticChallengeManager))
        {
            collectHapticChallengeManager.CurrentIngredientToCollectBehaviours.Add(this);
            
            if (!DoesNeedToShowUi) return;
            
            EnableCollect();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (DoesNeedToShowUi) return;
        
        if (other.CompareTag("Player"))
        {
            _currentTriggerTime += Time.deltaTime;
            
            if (_currentTriggerTime >= ingredientToCollectGlobalValuesSo.AfkTriggerTime)
            {
                EnableCollect();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CollectHapticChallengeManager collectHapticChallengeManager) &&
            collectHapticChallengeManager.CurrentIngredientToCollectBehaviours.Contains(this))
        {
            collectHapticChallengeManager.CurrentIngredientToCollectBehaviours.Remove(this);
            DisableCollect();
            _currentTriggerTime = 0f;
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
