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
    
    public Vector3 startControl { get; set; }
    public Vector3 endControl { get; set; }
    
    public Vector3 originControl{ get; set; }
    public float cauldronLerp { get; set; }
    
    [Header("UI")]
    [SerializeField] private GameObject grabInputCanvasGameObject;


    private void Start()
    {
        Instantiate(IngredientValuesSo.MeshGameObject, meshParentTransform);
        grabInputCanvasGameObject.SetActive(false);
        stackHeight = collectedIngredientGlobalValuesSo.StackHeight;
        rb = GetComponent<Rigidbody>();
        cauldronLerp = Random.Range(collectedIngredientGlobalValuesSo.MinCauldronLerp, collectedIngredientGlobalValuesSo.MaxCauldronLerp);

    }

    private float lerp = 0;

    private void Update()
    {
        if (!isPutInCauldron || lerp > 1) return;
        lerp += Time.deltaTime * cauldronLerp;
        transform.position =
            Mathf.Pow(1 - lerp, 3) * originControl +
            3 * Mathf.Pow(1 - lerp, 2) * lerp * startControl +
            3 * (1 - lerp) * Mathf.Pow(lerp, 2) * endControl +
            Mathf.Pow(lerp, 3) * Vector3.zero; //Last line is obsolete but for understanding purposes ill leave it in

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

    public void CauldronMethod()
    {
        originControl = transform.position;
        startControl = originControl + Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.value, Random.Range(-1f, 1f));
        endControl = Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.value, Random.Range(-1f, 1f));
        isPutInCauldron = true;
        CauldronBehaviour.instance.Ingredients.Add(IngredientValuesSo);
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
