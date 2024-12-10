using UnityEngine;
using Random = UnityEngine.Random;

public class CollectedIngredientBehaviour : MonoBehaviour
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
    
    private bool _isBeingDroppedInTarget;
    private Vector3 _startControl;
    private Vector3 _endControl;
    private Vector3 _originControl;
    private float _dropInTargetLerp;
    private Transform _dropTarget;
    private Vector3 _dropTargetOffset;
    private float _lerp;

    [Header("UI")]
    [SerializeField] private GameObject grabInputCanvasGameObject;


    private void Start()
    {
        Instantiate(IngredientValuesSo.MeshGameObject, meshParentTransform);
        grabInputCanvasGameObject.SetActive(false);
        StackHeight = collectedIngredientGlobalValuesSo.StackHeight;
        _dropInTargetLerp = Random.Range(collectedIngredientGlobalValuesSo.MinDropInTargetLerp,
            collectedIngredientGlobalValuesSo.MaxDropInTargetLerp);
    }

    private void Update()
    {
        if (!_isBeingDroppedInTarget || _lerp > 1) return;
        _lerp += Time.deltaTime * _dropInTargetLerp;
        transform.position =
            Mathf.Pow(1 - _lerp, 3) * _originControl +
            3 * Mathf.Pow(1 - _lerp, 2) * _lerp * _startControl +
            3 * (1 - _lerp) * Mathf.Pow(_lerp, 2) * _endControl +
            Mathf.Pow(_lerp, 3) * _dropTarget.position + _dropTargetOffset;
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
            characterInteractController.AddNewCollectedIngredient(this);
            EnableGrab();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) /*&&
            characterInteractController.CurrentIngredientToCollectBehaviour == this*/)
        {
            characterInteractController.RemoveCollectedIngredient(this);
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
