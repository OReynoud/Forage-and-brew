using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class CollectedIngredientBehaviour : StackableItem
{
    [Header("Dependencies")]
    [SerializeField] private CollectedIngredientGlobalValuesSo collectedIngredientGlobalValuesSo;
    [field: SerializeField] public IngredientValuesSo IngredientValuesSo { get; set; }
    public CookHapticChallengeSo CookedForm { get; set; }
    [SerializeField] private SphereCollider grabTrigger;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider ingredientCollider;
    [SerializeField] private Transform meshParentTransform;
    
    
    public override StackableValuesSo GetStackableValuesSo() => IngredientValuesSo;
    public UnityEvent<CollectedIngredientBehaviour> OnIngredientDropEnd { get; private set; } = new();

    [Header("UI")]
    [SerializeField] private GameObject localCanvasGameObject;
    [SerializeField] private GameObject grabInputGameObject;
    [SerializeField] private GameObject chopIconGameObject;
    [SerializeField] private GameObject grindIconGameObject;
    


    private void Start()
    {
        Instantiate(IngredientValuesSo.MeshGameObject, meshParentTransform);
        grabInputGameObject.SetActive(false);
        StackHeight = collectedIngredientGlobalValuesSo.StackHeight;
        dropInTargetLerp = Random.Range(collectedIngredientGlobalValuesSo.MinDropInTargetLerp,
            collectedIngredientGlobalValuesSo.MaxDropInTargetLerp);
        
        UpdateCookedForm();
    }




    public override void StackableDropped()
    {
        Debug.Log("used overriden method");
        isBeingDroppedInTarget = false;
        lerp = 0f;
        OnIngredientDropEnd.Invoke(this);
        OnIngredientDropEnd.RemoveAllListeners();
    }
    


    public override void EnableGrab()
    {
        localCanvasGameObject.SetActive(true);
        grabInputGameObject.SetActive(true);
    }
    
    public override void DisableGrab()
    {
        grabInputGameObject.SetActive(false);
        localCanvasGameObject.SetActive(grabInputGameObject.activeSelf || chopIconGameObject.activeSelf ||
                                        grindIconGameObject.activeSelf);
    }
    
    public override void GrabMethod(bool grab)
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
