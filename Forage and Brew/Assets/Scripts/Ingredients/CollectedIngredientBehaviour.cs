using System.Linq;
using DG.Tweening;
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
    
    // Shader hashes
    private static readonly int CutoffHeight = Shader.PropertyToID("_CutoffHeight");


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
        localCanvasGameObject.SetActive(false);
        chopIconGameObject.SetActive(false);
        grindIconGameObject.SetActive(false);
    }
    

    public void SetCutMeshGameObject()
    {
        if (IngredientValuesSo.CutMeshGameObject)
        {
            Destroy(meshParentTransform.GetChild(0).gameObject); // Remove original mesh
            Instantiate(IngredientValuesSo.CutMeshGameObject, meshParentTransform).transform.localScale =
                Vector3.one * collectedIngredientGlobalValuesSo.CutMeshScale;
            OnIngredientDropEnd.AddListener(SetFirstCutMeshPositionAndRotation);
        }
        else
        {
            Debug.LogWarning("No Cut Mesh GameObject set for " + IngredientValuesSo.name);
        }
    }
    
    private void SetFirstCutMeshPositionAndRotation(CollectedIngredientBehaviour collectedIngredientBehaviour)
    {
        SetCutMeshPositionAndRotation(0);
    }
    
    public void SetCutMeshPositionAndRotation(int index)
    {
        meshParentTransform.GetChild(0).DOLocalMove(IngredientValuesSo.CutMeshPositions[index], 
                collectedIngredientGlobalValuesSo.CutMeshMoveDuration)
            .SetEase(collectedIngredientGlobalValuesSo.CutMeshMoveCurve);
        meshParentTransform.GetChild(0).DOLocalRotate(IngredientValuesSo.CutMeshRotations[index], 
                collectedIngredientGlobalValuesSo.CutMeshMoveDuration)
            .SetEase(collectedIngredientGlobalValuesSo.CutMeshMoveCurve);

        for (int i = 0; i < index; i++)
        {
            if (i < IngredientValuesSo.CutPieceCount)
            {
                meshParentTransform.GetChild(0).GetChild(i).DOMove(meshParentTransform.position +
                    IngredientValuesSo.CutMeshEndPositions[i], collectedIngredientGlobalValuesSo.CutMeshMoveDuration)
                    .SetEase(collectedIngredientGlobalValuesSo.CutMeshMoveCurve);
                meshParentTransform.GetChild(0).GetChild(i).DORotate((meshParentTransform.rotation *
                        Quaternion.Euler(IngredientValuesSo.CutMeshEndRotations[i])).eulerAngles,
                        collectedIngredientGlobalValuesSo.CutMeshMoveDuration)
                    .SetEase(collectedIngredientGlobalValuesSo.CutMeshMoveCurve);
            }
            else
            {
                if (meshParentTransform.GetChild(0).GetChild(i).gameObject.activeSelf)
                {
                    int trashIndex = i;
                    // Dissolve trash pieces
                    meshParentTransform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().material
                        .DOFloat(1f, CutoffHeight, collectedIngredientGlobalValuesSo.CutMeshMoveDuration)
                        .OnComplete(() => meshParentTransform.GetChild(0).GetChild(trashIndex).gameObject.SetActive(false));
                }
            }
        }
    }
    
    public void SetFinalCutMeshPositionAndRotation()
    {
        Transform meshTransform = meshParentTransform.GetChild(0);
        
        meshParentTransform.localPosition = Vector3.zero;
        meshTransform.localPosition = Vector3.zero;
        meshTransform.localRotation = Quaternion.identity;
        meshTransform.localScale = Vector3.one;
        
        Sequence cutMeshSequence = DOTween.Sequence();
        
        float minXEndPosition = IngredientValuesSo.CutMeshEndPositions.Min(pos => pos.x);
        float maxXEndPosition = IngredientValuesSo.CutMeshEndPositions.Max(pos => pos.x);
        float averageXEndPosition = (minXEndPosition + maxXEndPosition) * 0.5f;
        float minYEndPosition = IngredientValuesSo.CutMeshEndPositions.Min(pos => pos.y);
        float maxYEndPosition = IngredientValuesSo.CutMeshEndPositions.Max(pos => pos.y);
        float averageYEndPosition = (minYEndPosition + maxYEndPosition) * 0.5f;
        float minZEndPosition = IngredientValuesSo.CutMeshEndPositions.Min(pos => pos.z);
        float maxZEndPosition = IngredientValuesSo.CutMeshEndPositions.Max(pos => pos.z);
        float averageZEndPosition = (minZEndPosition + maxZEndPosition) * 0.5f;
        Vector3 averageEndPosition = new(averageXEndPosition, averageYEndPosition, averageZEndPosition);
        
        for (int i = 0; i < IngredientValuesSo.CutPieceCount + IngredientValuesSo.CutTrashCount; i++)
        {
            if (i < IngredientValuesSo.CutPieceCount)
            {
                cutMeshSequence.Join(meshTransform.GetChild(i).DOLocalMove((IngredientValuesSo.CutMeshEndPositions[i] -
                            averageEndPosition) / collectedIngredientGlobalValuesSo.CutMeshScale,
                        collectedIngredientGlobalValuesSo.CutMeshMoveDuration)
                    .SetEase(collectedIngredientGlobalValuesSo.CutMeshMoveCurve));
                cutMeshSequence.Join(meshTransform.GetChild(i).DOLocalRotate(
                        IngredientValuesSo.CutMeshEndRotations[i],
                        collectedIngredientGlobalValuesSo.CutMeshMoveDuration)
                    .SetEase(collectedIngredientGlobalValuesSo.CutMeshMoveCurve));
            }
            else
            {
                if (meshTransform.GetChild(i).gameObject.activeSelf)
                {
                    int trashIndex = i;
                    // Dissolve trash pieces
                    meshTransform.GetChild(i).GetComponent<MeshRenderer>().material
                        .DOFloat(1f, CutoffHeight, collectedIngredientGlobalValuesSo.CutMeshMoveDuration)
                        .OnComplete(() => meshTransform.GetChild(trashIndex).gameObject.SetActive(false));
                }
            }
        }
    }
    

    public void SetUpMeshForGrinding()
    {
        meshParentTransform.GetChild(0).DOScale(collectedIngredientGlobalValuesSo.GroundMeshScale, 
                collectedIngredientGlobalValuesSo.GroundMeshScaleDuration)
            .SetEase(collectedIngredientGlobalValuesSo.GroundMeshScaleCurve);
        meshParentTransform.DOLocalMove(Vector3.zero, collectedIngredientGlobalValuesSo.GroundMeshScaleDuration)
            .SetEase(collectedIngredientGlobalValuesSo.GroundMeshScaleCurve);
    }

    public void SetGroundMeshGameObject()
    {
        if (IngredientValuesSo.Type.GroundMeshGameObject)
        {
            Destroy(meshParentTransform.GetChild(0).gameObject); // Remove original mesh
            meshParentTransform.localPosition = Vector3.zero;
            Instantiate(IngredientValuesSo.Type.GroundMeshGameObject, meshParentTransform);
        }
        else
        {
            Debug.LogWarning("No Ground Mesh GameObject set for " + IngredientValuesSo.name);
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
