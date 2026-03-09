using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class GardenPlotBehavior : PurchasableHouseItemBehaviour, ISeedAddable
{
    [SerializeField] private IngredientToCollectBehaviour ingredientToCollect;
    [SerializeField] private int gardenPlotSelfIndex;
    
    [Header("Growth Settings")]
    [SerializeField] private List<Vector3> sproutPositions = new();
    [SerializeField] private List<Vector3> sproutRotations = new();
    [SerializeField] private float growDuration;
    [SerializeField] private AnimationCurve growCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private Tweener _growthPositionTweener;
    private Tweener _growthRotationTweener;
        
    [field: BoxGroup("Plot Data")] [field: SerializeField] public bool NeedsWatering { get; set; }
    [field: BoxGroup("Plot Data")] [field: SerializeField] public int PlantGrowthProgression { get; set; }
    [field: BoxGroup("Plot Data")] [field: SerializeField] public int RequiredProgressionToMature { get; set; }
    [field: BoxGroup("Plot Data")] [field: SerializeField] public SeedValuesSo PlantedSeed { get; set; }
    
    [BoxGroup("Refs")] public MeshRenderer parcelMesh;
    [BoxGroup("Refs")] public Material dryParcelMat;
    [BoxGroup("Refs")] public Material wetParcelMat;
    [BoxGroup("Refs")] public GameObject sproutMesh;
    [BoxGroup("Refs")] public GameObject wateringCheckMark;
    [BoxGroup("Refs")] public GameObject seedInformationCanvas;
    [BoxGroup("Refs")] public Image seedIndicator;
    [BoxGroup("Refs")] public GameObject buttonAObject;

    [field: SerializeField] public bool UseEndPoint { get; set; }
    [field: SerializeField] public Transform EndPoint { get; set; }
    [field: SerializeField] public float heightShove { get; set; }

    [SerializeField] private GameObject interactInputCanvasGameObject;
    
    // Animator hashes
    private static readonly int DoWater = Animator.StringToHash("DoWatering");

    
    protected override void Start()
    {
        for (int i = GameDontDestroyOnLoadManager.Instance.plotsData.Count; i < gardenPlotSelfIndex + 1; i++)
        {
            GameDontDestroyOnLoadManager.Instance.plotsData.Add(new GardenPlotData());
        }
        
        base.Start();
        DisableInteract();
        SceneTransitionManager.instance.OnSleep.AddListener(ProgressDay);
        if (!GameDontDestroyOnLoadManager.Instance.codexIsUnlocked)
        {
            sproutMesh.SetActive(false);
        }
        UpdateVisuals();
    }

    private void ProgressDay()
    {
        if (PlantedSeed)
        {
            if (PlantGrowthProgression < RequiredProgressionToMature && !NeedsWatering)
            {
                PlantGrowthProgression++;
            }

            if (PlantGrowthProgression < RequiredProgressionToMature)
            {
                NeedsWatering = true;
            }
            
            UpdateVisuals();
        }
    }

    public void EnableInteract()
    {
        buttonAObject.SetActive(true);
        interactInputCanvasGameObject.SetActive(true);
    }

    public void DisableInteract()
    {
        buttonAObject.SetActive(false);
        interactInputCanvasGameObject.SetActive(false);
    }

    public void UpdateVisuals()
    {
        if (PlantedSeed)
        {
            seedInformationCanvas.SetActive(true);
            seedIndicator.sprite = PlantedSeed.IngredientToGrowSo.iconLow;
            wateringCheckMark.SetActive(!NeedsWatering);
            parcelMesh.material = NeedsWatering ? dryParcelMat : wetParcelMat;
            int totalProgression = PlantGrowthProgression + (NeedsWatering ? 0 : 1);
            
            if (PlantGrowthProgression >= PlantedSeed.DaysToMature)
            {
                ingredientToCollect.gameObject.SetActive(true);
                sproutMesh.SetActive(false);
            }
            else if (totalProgression == 0)
            {
                ingredientToCollect.gameObject.SetActive(false);
                sproutMesh.SetActive(false);
                sproutMesh.transform.localPosition = sproutPositions[0];
                sproutMesh.transform.localRotation = Quaternion.Euler(sproutRotations[0]);
            }
            else
            {
                sproutMesh.SetActive(true);
                
                if (sproutMesh.transform.localPosition != sproutPositions[totalProgression] &&
                    (_growthPositionTweener == null || !_growthPositionTweener.IsActive() || !_growthPositionTweener.IsPlaying()))
                {
                    _growthPositionTweener = sproutMesh.transform.DOLocalMove(sproutPositions[totalProgression],
                        growDuration).SetEase(growCurve);
                }

                if (sproutMesh.transform.localRotation != Quaternion.Euler(sproutRotations[totalProgression]) &&
                    (_growthRotationTweener == null || !_growthRotationTweener.IsActive() || !_growthRotationTweener.IsPlaying()))
                {
                    _growthRotationTweener = sproutMesh.transform.DOLocalRotate(sproutRotations[totalProgression],
                        growDuration).SetEase(growCurve);
                }
            }
        }
        else
        {
            seedInformationCanvas.SetActive(false);
            seedIndicator.sprite = null;
        }
    }

    public override void InitPurchasableHouseItem()
    {
        base.InitPurchasableHouseItem();

        if (!Unlocked) return;

        GardenPlotData data = GameDontDestroyOnLoadManager.Instance.plotsData[gardenPlotSelfIndex];
        NeedsWatering = data.NeedsWatering;
        PlantGrowthProgression = data.PlantGrowthProgression;
        RequiredProgressionToMature = data.RequiredProgressionToMature;
        PlantedSeed = data.PlantedSeed;
        
        if (PlantedSeed)
        {
            ingredientToCollect.IngredientValuesSo = PlantedSeed.IngredientToGrowSo;
            ingredientToCollect.SpawnMesh();
        }
    }
    
    public void WaterPlot()
    {
        // Debug.Log("Watered plant");
        NeedsWatering = false;
        UpdateVisuals();
        CharacterAnimManager.instance.animator.SetTrigger(DoWater);
        Vector3 posToLook = new(transform.position.x,
            CharacterAnimManager.instance.transform.position.y, transform.position.z);
        CharacterAnimManager.instance.transform.LookAt(posToLook);

        GameDontDestroyOnLoadManager.Instance.plotsData[gardenPlotSelfIndex].UpdateData(this);
    }

    public void AddSeed(CollectedSeedBehaviour collectedSeedBehaviour)
    {
        PlantedSeed = collectedSeedBehaviour.SeedValuesSo;
        RequiredProgressionToMature = PlantedSeed.DaysToMature;
        ingredientToCollect.IngredientValuesSo = PlantedSeed.IngredientToGrowSo;
        NeedsWatering = true;
        UpdateVisuals();
        GameDontDestroyOnLoadManager.Instance.plotsData[gardenPlotSelfIndex].UpdateData(this);
        
        TutorialManager.instance.NotifyFromRecipeReceived("WaterSeeds");
    }

    public void HandlePlayerInput()
    {
        if (CanPurchase)
        {
            PurchaseItem();
            return;
        }

        if (!Unlocked)
            return;
        
        if (!PlantedSeed)
        {
            if (CharacterInteractController.Instance.collectedStack.Count == 0)
                return;

            if (CharacterInteractController.Instance.collectedStack[0].StackableItem is not CollectedSeedBehaviour)
                return;
            
            // Debug.Log("Planted new seed");

            CharacterAnimManager.instance.CatThrow();

            CharacterInteractController.Instance.ShovePartialStackInTarget(transform, this, new []{CharacterInteractController.Instance.collectedStack[^1]});

            AddSeed((CollectedSeedBehaviour)CharacterInteractController.Instance.collectedStack[^1].StackableItem);
            
            CharacterInteractController.Instance.collectedStack.RemoveAt(CharacterInteractController.Instance.collectedStack.Count - 1);
            
            if (CharacterInteractController.Instance.collectedStack.Count == 0)
                CharacterInteractController.Instance.AreHandsFull = false;
            return;
        }
        
        if (PlantGrowthProgression == RequiredProgressionToMature)
        {
            // Debug.Log("Ingredient collect haptic challenge");
            //TODO: Vegetable harvest haptic challenge
        }
        
        if (NeedsWatering)
        {
            WaterPlot();
        }
    }
    
    
    protected override void ManageCharacterNear(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            if (CanPurchase)
            {
                if (characterInteractController.collectedStack.Count == 0)
                {
                    LastTriggeredCollider = other;

                    pricePopUpBehaviour.ShowPrice(purchaseCost);

                    characterInteractController.CurrentNearPlot = this;
                }
            }
            else if (Unlocked)
            {
                if (characterInteractController.collectedStack.Count == 0 && NeedsWatering ||
                    !PlantedSeed && characterInteractController.collectedStack.Count > 0 &&
                    characterInteractController.collectedStack[0].StackableItem is CollectedSeedBehaviour)
                {
                    LastTriggeredCollider = other;
                    
                    characterInteractController.CurrentNearPlot = this;
                    
                    EnableInteract();
                }
            }
        }
    }

    protected override void ManageCharacterFar(Collider other)
    {
        if (LastTriggeredCollider == other)
        {
            LastTriggeredCollider = null;
        }

        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.CurrentNearPlot == this)
        {
            characterInteractController.CurrentNearPlot = null;
        }

        DisableInteract();
        pricePopUpBehaviour.HidePrice();
    }
}
