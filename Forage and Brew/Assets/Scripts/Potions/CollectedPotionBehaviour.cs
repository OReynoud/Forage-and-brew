using UnityEngine;
using UnityEngine.Events;

public class CollectedPotionBehaviour : MonoBehaviour, IStackable
{
    [Header("Dependencies")]
    [SerializeField] private CollectedPotionGlobalValuesSo collectedPotionGlobalValuesSo;
    [field: SerializeField] public PotionValuesSo PotionValuesSo { get; set; }
    [SerializeField] private SphereCollider grabTrigger;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider potionCollider;
    [SerializeField] private Transform meshParentTransform;
    
    public float StackHeight { get; private set; }
    public UnityEvent<CollectedPotionBehaviour> OnPotionDropEnd { get; private set; } = new();
    
    private bool _isBeingDroppedInTarget;
    private Vector3 _startControl;
    private Vector3 _endControl;
    private Vector3 _originControl;
    private Vector3 _startRotation;
    private Vector3 _endRotation;
    private float _dropInTargetLerp;
    private Transform _dropTarget;
    private Vector3 _dropTargetOffset;
    private float _lerp;

    [Header("UI")]
    [SerializeField] private GameObject grabInputCanvasGameObject;

    public Transform GetTransform() => transform;
    public StackableValuesSo GetStackableValuesSo() => PotionValuesSo;
    public float GetStackHeight() => StackHeight;
    
    [field: SerializeField] public AnimationCurve ShoveRotationCurve { get; set; } = AnimationCurve.EaseInOut(0,0,1,1);


    private void Start()
    {
        PotionLiquidColorManager potionLiquidColorManager = Instantiate(PotionValuesSo.PotionDifficulty
            .MeshGameObjectLiquidColorManager, meshParentTransform);
        potionLiquidColorManager.SetLiquidColor(PotionValuesSo);
        grabInputCanvasGameObject.SetActive(false);
        StackHeight = collectedPotionGlobalValuesSo.StackHeight;
        _dropInTargetLerp = Random.Range(collectedPotionGlobalValuesSo.MinDropInTargetLerp,
            collectedPotionGlobalValuesSo.MaxDropInTargetLerp);
    }

    private void Update()
    {
        if (!_isBeingDroppedInTarget) return;

        if (_lerp >= 1f)
        {
            _isBeingDroppedInTarget = false;
            _lerp = 0f;
            OnPotionDropEnd.Invoke(this);
            OnPotionDropEnd.RemoveAllListeners();
            return;
        }
        
        _lerp += Time.deltaTime * _dropInTargetLerp;
        transform.position =
            Mathf.Pow(1 - _lerp, 3) * _originControl +
            3 * Mathf.Pow(1 - _lerp, 2) * _lerp * _startControl +
            3 * (1 - _lerp) * Mathf.Pow(_lerp, 2) * _endControl +
            Mathf.Pow(_lerp, 3) * _dropTarget.position + _dropTargetOffset;
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(_startRotation),Quaternion.Euler(_endRotation), ShoveRotationCurve.Evaluate(_lerp) );
    }


    public void EnableGrab()
    {
        grabInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableGrab()
    {
        grabInputCanvasGameObject.SetActive(false);
    }


    public void GrabMethod(bool grab)
    {
        rb.isKinematic = grab;
        grabTrigger.enabled = !grab;
        potionCollider.enabled = !grab;
        rb.AddForce(Random.insideUnitSphere,ForceMode.Impulse);

        if (grab)
        {
            DisableGrab();
        }
    }

    public void EnablePhysics()
    {
        rb.isKinematic = false;
        potionCollider.enabled = true;
    }

    public void DisableInteraction()
    {
        grabTrigger.enabled = false;
        DisableGrab();
    }

    
    public void DropInTarget(Transform target, bool useEndPoint, Vector3 offset = default)
    {
        _dropTarget = target;
        _dropTargetOffset = offset;
        _originControl = transform.position;
        _startRotation = transform.eulerAngles;
        if (useEndPoint)
        {
            _startControl = _originControl + offset;
            _endControl = target.position;
        }
        else
        {
            _startControl = _originControl + Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.value, Random.Range(-1f, 1f));
            _endControl = target.position + _dropTargetOffset + Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.value, Random.Range(-1f, 1f));
        }
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
        if (collectedPotionGlobalValuesSo && grabTrigger)
        {
            grabTrigger.radius = collectedPotionGlobalValuesSo.GrabRadius;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, grabTrigger.radius);
        }
    }

    #endregion
}
