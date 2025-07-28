using UnityEngine;
using UnityEngine.Events;

public class CollectedPotionBehaviour : StackableItem
{
    [Header("Dependencies")]
    [SerializeField] private CollectedPotionGlobalValuesSo collectedPotionGlobalValuesSo;
    [field: SerializeField] public PotionValuesSo PotionValuesSo { get; set; }
    [SerializeField] private SphereCollider grabTrigger;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider potionCollider;
    [SerializeField] private Transform meshParentTransform;
    
    public UnityEvent<CollectedPotionBehaviour> OnPotionDropEnd { get; private set; } = new();
    


    [Header("UI")]
    [SerializeField] private GameObject grabInputCanvasGameObject;

    public override StackableValuesSo GetStackableValuesSo() => PotionValuesSo;



    private void Start()
    {
        PotionLiquidColorManager potionLiquidColorManager = Instantiate(PotionValuesSo.PotionDifficulty
            .MeshGameObjectLiquidColorManager, meshParentTransform);
        potionLiquidColorManager.SetLiquidColor(PotionValuesSo);
        grabInputCanvasGameObject.SetActive(false);
        StackHeight = collectedPotionGlobalValuesSo.StackHeight;
        dropInTargetLerp = Random.Range(collectedPotionGlobalValuesSo.MinDropInTargetLerp,
            collectedPotionGlobalValuesSo.MaxDropInTargetLerp);
    }


    public override void StackableDropped()
    {
        isBeingDroppedInTarget = false;
        lerp = 0f;
        OnPotionDropEnd.Invoke(this);
        OnPotionDropEnd.RemoveAllListeners();
    }


    public override void EnableGrab()
    {
        grabInputCanvasGameObject.SetActive(true);
    }
    
    public override void DisableGrab()
    {
        grabInputCanvasGameObject.SetActive(false);
    }


    public override void GrabMethod(bool grab)
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
