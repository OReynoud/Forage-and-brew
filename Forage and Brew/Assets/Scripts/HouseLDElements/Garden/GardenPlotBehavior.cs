using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class GardenPlotBehavior : PurchasableHouseItemBehaviour, ISeedAddable
{
    private static readonly int DoWater = Animator.StringToHash("DoWatering");
    [SerializeField] private IngredientToCollectBehaviour ingredientToCollect;
    [SerializeField] private int gardenPlotSelfIndex;
    [field: BoxGroup("Plot Data")] [field: SerializeField] public bool NeedsWatering { get; set; }
    [field: BoxGroup("Plot Data")] [field: SerializeField] public int PlantGrowthProgression { get; set; }
    [field: BoxGroup("Plot Data")] [field: SerializeField] public int RequiredProgressionToMature { get; set; }
    [field: BoxGroup("Plot Data")] [field: SerializeField] public SeedValuesSo PlantedSeed { get; set; }
    
    [BoxGroup("Refs")]public MeshRenderer parcelMesh;
    [BoxGroup("Refs")]public Material dryParcelMat;
    [BoxGroup("Refs")]public Material wetParcelMat;
    [BoxGroup("Refs")]public GameObject sproutMesh;
    [BoxGroup("Refs")]public GameObject wateringCheckMark;
    [BoxGroup("Refs")]private GameObject fullyGrownPlantMesh;
    [BoxGroup("Refs")] public GameObject seedInformationBubble;
    [BoxGroup("Refs")]public Image seedIndicator;
    [BoxGroup("Refs")]public GameObject buttonAObject;

    [field: SerializeField] public bool UseEndPoint { get; set; }
    [field: SerializeField] public Transform EndPoint { get; set; }
    [field: SerializeField] public float heightShove { get; set; }

    [SerializeField] private GameObject interactInputCanvasGameObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
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
            if (NeedsWatering == false)
            {
                PlantGrowthProgression++;
                Debug.Log("Plant has grown!");
            }
            NeedsWatering = true;
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
            seedInformationBubble.SetActive(true);
            seedIndicator.sprite = PlantedSeed.IngredientToGrowSo.iconLow;
            wateringCheckMark.SetActive(!NeedsWatering);
            parcelMesh.material = NeedsWatering ? dryParcelMat : wetParcelMat;
            if (PlantGrowthProgression == PlantedSeed.DaysToMature && fullyGrownPlantMesh == null)
            {
                ingredientToCollect.gameObject.SetActive(true);
            }
            else if(PlantGrowthProgression > 0 && PlantGrowthProgression < PlantedSeed.DaysToMature)
            {
                sproutMesh.gameObject.SetActive(true);
            }
        }
        else
        {
            seedInformationBubble.SetActive(false);
            seedIndicator.sprite = null;
        }
    }

    public override void InitPurchasableHouseItem()
    {
        if (GameDontDestroyOnLoadManager.Instance.GardenProgressionIndex == selfIndex)
        {
            CanPurchase = true;
        }
        else if (GameDontDestroyOnLoadManager.Instance.GardenProgressionIndex > selfIndex)
        {
            Unlocked = true;
        }

        if (!Unlocked) return;

        var data = GardenManager.instance.plotsData[gardenPlotSelfIndex];
        NeedsWatering = data.NeedsWatering;
        PlantGrowthProgression = data.PlantGrowthProgression;
        RequiredProgressionToMature = data.RequiredProgressionToMature;
        PlantedSeed = data.PlantedSeed;
    }
    
    public void WaterPlot()
    {
        Debug.Log("Watered plant");
        NeedsWatering = false;
        UpdateVisuals();
        CharacterAnimManager.instance.animator.SetTrigger(DoWater);
        Vector3 posToLook = new(transform.position.x,
            CharacterAnimManager.instance.transform.position.y, transform.position.z);
        CharacterAnimManager.instance.transform.LookAt(posToLook);

        GardenManager.instance.plotsData[gardenPlotSelfIndex].UpdateData(this);
    }

    public void AddSeed(CollectedSeedBehaviour collectedSeedBehaviour)
    {
        PlantedSeed = collectedSeedBehaviour.SeedValuesSo;
        RequiredProgressionToMature = PlantedSeed.DaysToMature;
        ingredientToCollect.IngredientValuesSo = PlantedSeed.IngredientToGrowSo;
        ingredientToCollect.SpawnMesh();
        NeedsWatering = true;
        UpdateVisuals();
        GardenManager.instance.plotsData[gardenPlotSelfIndex].UpdateData(this);
        
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
            
            Debug.Log("Planted new seed");

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
            Debug.Log("Ingredient collect haptic challenge");
            //TODO: Vegetable harvest haptic challenge
        }
        
        if (NeedsWatering)
        {
            WaterPlot();
        }
    }
    
    protected override void ManageCharacterNear(Collider other)
    {
        if (CanPurchase)
        {
            if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
                characterInteractController.collectedStack.Count == 0)
            {
                LastTriggeredCollider = other;

                pricePopUpBehaviour.ShowPrice(purchaseCost);

                characterInteractController.CurrentNearPlot = this;
            }
        }
        else if (Unlocked)
        {
            if (other.TryGetComponent(out CharacterInteractController characterInteractController))
            {
                LastTriggeredCollider = other;

                characterInteractController.CurrentNearPlot = this;

                EnableInteract();
            }
        }
    }


    protected override void ManageCharacterFar(Collider other)
    {
        //if (IsCharacterOnCountertop) return;

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