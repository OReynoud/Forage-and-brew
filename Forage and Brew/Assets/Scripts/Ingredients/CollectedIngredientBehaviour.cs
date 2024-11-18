using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectedIngredientBehaviour : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CollectedIngredientGlobalValuesSo collectedIngredientGlobalValuesSo;
    [field: SerializeField] public IngredientValuesSo IngredientValuesSo { get; set; }
    [SerializeField] private SphereCollider grabTrigger;
    private Rigidbody rb;
    [SerializeField] private Collider ingredientCollider;
    [SerializeField] private Transform meshParentTransform;
    public float stackHeight { get; set; }
    public bool isPutInCauldron { get; set; }
    
    public Vector3 middlePoint { get; set; }
    
    [Header("UI")]
    [SerializeField] private GameObject grabInputCanvasGameObject;


    private void Start()
    {
        Instantiate(IngredientValuesSo.MeshGameObject, meshParentTransform);
        grabInputCanvasGameObject.SetActive(false);
        stackHeight = collectedIngredientGlobalValuesSo.StackHeight;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isPutInCauldron) return;
        
        //transform.position = Vector3.Slerp(transform.position,Vector3.zero, ) + middlePoint;
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
        
    }

    #region Trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.SetNewCollectedIngredient(this);
            EnableGrab();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) /*&&
            characterInteractController.CurrentIngredientToCollectBehaviour == this*/)
        {
            characterInteractController.SetNewCollectedIngredient(null);
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
