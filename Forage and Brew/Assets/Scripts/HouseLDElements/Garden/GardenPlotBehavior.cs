using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GardenPlotBehavior : PurchasableHouseItemBehaviour, ISeedAddable
{

    public int plotIndex;
    [field: SerializeField] public bool NeedsWatering { get; set; }
    [field: SerializeField] public int PlantGrowthProgression { get; set; }
    [field: SerializeField] public int RequiredProgressionToMature { get; set; }
    [field: SerializeField] public SeedValuesSo PlantedSeed { get; set; }
    public Sprite wateredSprite;
    public Sprite needsWaterSprite;
    public Image wateringIndicator;
    public Image seedIndicator;
    
    [field: SerializeField] public bool UseEndPoint { get; set; }
    [field: SerializeField] public Transform EndPoint { get; set; }
    [field: SerializeField] public float heightShove { get; set; }
    
    [SerializeField] private GameObject interactInputCanvasGameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        InitPurchasableHouseItem();
        DisableInteract();
    }
    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
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
            if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
                other.TryGetComponent(out ChoppingHapticChallengeManager choppingHapticChallengeManager) &&
                characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].StackableItem is CollectedIngredientBehaviour
                    { CookedForm: null } &&
                ((CollectedIngredientBehaviour)characterInteractController.collectedStack[0].StackableItem)
                .IngredientValuesSo.Type.IsChoppable)
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



    public override void InitPurchasableHouseItem()
    {
        if (GameDontDestroyOnLoadManager.Instance.WorkshopProgressionIndex == selfIndex)
        {
            CanPurchase = true;
        }
        else if (GameDontDestroyOnLoadManager.Instance.WorkshopProgressionIndex > selfIndex)
        {
            Unlocked = true;
        }

        if (!Unlocked)return;
        
        var data = GardenManager.instance.plotsData[plotIndex];
        NeedsWatering = data.NeedsWatering;
        PlantGrowthProgression = data.PlantGrowthProgression;
        RequiredProgressionToMature = data.RequiredProgressionToMature;
        PlantedSeed = data.PlantedSeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WaterPlot()
    {
        PlantGrowthProgression++;
        wateringIndicator.sprite = wateredSprite;
    }

    public void AddSeed(CollectedSeedBehavior collectedSeedBehaviour)
    {
        PlantedSeed = collectedSeedBehaviour.SeedValuesSo;
    }


}
