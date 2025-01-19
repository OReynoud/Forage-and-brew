using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class CollectedIngredientBehaviour : MonoBehaviour, IStackable
{
    [Header("Dependencies")]
    [SerializeField] private CollectedIngredientGlobalValuesSo collectedIngredientGlobalValuesSo;
    [field: SerializeField] public IngredientValuesSo IngredientValuesSo { get; set; }
    public CookHapticChallengeSo CookedForm { get; set; }
    [SerializeField] private SphereCollider grabTrigger;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider ingredientCollider;
    [SerializeField] private Transform meshParentTransform;
    
    public float StackHeight { get; private set; }
    public UnityEvent<CollectedIngredientBehaviour> OnIngredientDropEnd { get; private set; } = new();
    
    private bool _isBeingDroppedInTarget;
    private Vector3 _startControl;
    private Vector3 _endControl;
    private Vector3 _originControl;
    private float _dropInTargetLerp;
    private Transform _dropTarget;
    private Vector3 _dropTargetOffset;
    private float _lerp;

    [Header("UI")]
    [SerializeField] private GameObject localCanvasGameObject;
    [SerializeField] private GameObject grabInputGameObject;
    [SerializeField] private GameObject chopIconGameObject;
    [SerializeField] private GameObject grindIconGameObject;

    public Transform GetTransform() => transform;
    public StackableValuesSo GetStackableValuesSo() => IngredientValuesSo;
    public float GetStackHeight() => StackHeight;


    private void Start()
    {
        Instantiate(IngredientValuesSo.MeshGameObject, meshParentTransform);
        grabInputGameObject.SetActive(false);
        StackHeight = collectedIngredientGlobalValuesSo.StackHeight;
        _dropInTargetLerp = Random.Range(collectedIngredientGlobalValuesSo.MinDropInTargetLerp,
            collectedIngredientGlobalValuesSo.MaxDropInTargetLerp);
        
        UpdateCookedForm();
    }

    private void Update()
    {
        if (!_isBeingDroppedInTarget) return;

        if (_lerp >= 1f)
        {
            _isBeingDroppedInTarget = false;
            _lerp = 0f;
            OnIngredientDropEnd.Invoke(this);
            OnIngredientDropEnd.RemoveAllListeners();
            return;
        }
        
        _lerp += Time.deltaTime * _dropInTargetLerp;
        transform.position =
            Mathf.Pow(1 - _lerp, 3) * _originControl +
            3 * Mathf.Pow(1 - _lerp, 2) * _lerp * _startControl +
            3 * (1 - _lerp) * Mathf.Pow(_lerp, 2) * _endControl +
            Mathf.Pow(_lerp, 3) * _dropTarget.position + _dropTargetOffset;
    }


    public void EnableGrab()
    {
        localCanvasGameObject.SetActive(true);
        grabInputGameObject.SetActive(true);
    }
    
    public void DisableGrab()
    {
        grabInputGameObject.SetActive(false);
        localCanvasGameObject.SetActive(grabInputGameObject.activeSelf || chopIconGameObject.activeSelf ||
                                        grindIconGameObject.activeSelf);
    }
    
    
    public void SetCookedForm(CookHapticChallengeSo cookedForm)
    {
        CookedForm = cookedForm;
        UpdateCookedForm();
    }
    
    private void UpdateCookedForm()
    {
        if (CookedForm is ChoppingHapticChallengeListSo)
        {
            localCanvasGameObject.SetActive(true);
            chopIconGameObject.SetActive(true);
            grindIconGameObject.SetActive(false);
        }
        else if (CookedForm is GrindingHapticChallengeSo)
        {
            localCanvasGameObject.SetActive(true);
            chopIconGameObject.SetActive(false);
            grindIconGameObject.SetActive(true);
        }
        else
        {
            localCanvasGameObject.SetActive(false);
            chopIconGameObject.SetActive(false);
            grindIconGameObject.SetActive(false);
        }
    }


    public void GrabMethod(bool grab)
    {
        rb.isKinematic = grab;
        grabTrigger.enabled = !grab;
        ingredientCollider.enabled = !grab;
        rb.AddForce(Random.insideUnitSphere,ForceMode.Impulse);

        if (grab)
        {
            DisableGrab();
        }
    }

    public void DropInTarget(Transform target, Vector3 offset = default)
    {
        _dropTarget = target;
        _dropTargetOffset = offset;
        _originControl = transform.position;
        _startControl = _originControl + Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.value, Random.Range(-1f, 1f));
        _endControl = target.position + _dropTargetOffset + Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.value, Random.Range(-1f, 1f));
        _lerp = 0f;
        _isBeingDroppedInTarget = true;
    }

    #region Trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.AddNewCollectedStackable(this);
            EnableGrab();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) /*&&
            characterInteractController.CurrentIngredientToCollectBehaviour == this*/)
        {
            characterInteractController.RemoveCollectedStackable(this);
            DisableGrab(); // Temporary
        }
    }

    #endregion

    
    #region Gizmos

    private void OnDrawGizmos()
    {
        if (collectedIngredientGlobalValuesSo && grabTrigger)
        {
            grabTrigger.radius = collectedIngredientGlobalValuesSo.GrabRadius;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, grabTrigger.radius);
        }
    }

    #endregion
}
