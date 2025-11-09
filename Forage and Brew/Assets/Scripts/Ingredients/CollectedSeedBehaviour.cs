using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectedSeedBehaviour : StackableItem
{
    [Header("Dependencies")]
    [SerializeField] private CollectedIngredientGlobalValuesSo collectedIngredientGlobalValuesSo;
    [field: SerializeField] public SeedValuesSo SeedValuesSo { get; set; }
    [SerializeField] private SphereCollider grabTrigger;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider ingredientCollider;
    [SerializeField] private List<SpriteRenderer> vegetableSpriteRenderers;
    
    
    public override StackableValuesSo GetStackableValuesSo() => SeedValuesSo;

    [Header("UI")]
    [SerializeField] private GameObject localCanvasGameObject;
    [SerializeField] private GameObject grabInputGameObject;
    


    public void Start()
    {
        foreach (SpriteRenderer vegetableSpriteRenderer in vegetableSpriteRenderers)
        {
            vegetableSpriteRenderer.sprite = SeedValuesSo.IngredientToGrowSo.iconLow;
        }
        grabInputGameObject.SetActive(false);
        StackHeight = collectedIngredientGlobalValuesSo.StackHeight;
        dropInTargetLerp = Random.Range(collectedIngredientGlobalValuesSo.MinDropInTargetLerp,
            collectedIngredientGlobalValuesSo.MaxDropInTargetLerp);
    }




    public override void StackableDropped()
    {
        isBeingDroppedInTarget = false;
        lerp = 0f;
    }
    


    public override void EnableGrab()
    {
        localCanvasGameObject.SetActive(true);
        grabInputGameObject.SetActive(true);
    }
    
    public override void DisableGrab()
    {
        grabInputGameObject.SetActive(false);
        localCanvasGameObject.SetActive(grabInputGameObject.activeSelf);
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
