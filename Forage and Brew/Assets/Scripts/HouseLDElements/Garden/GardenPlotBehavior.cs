using System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GardenPlotBehavior : PurchasableHouseItemBehaviour, ISeedAddable
{
    [field: SerializeField] public bool NeedsWatering { get; set; }
    [field: SerializeField] public int PlantGrowthProgression { get; set; }
    [field: SerializeField] public int RequiredProgressionToMature { get; set; }
    [field: SerializeField] public bool IsWeed { get; set; }

    [field: SerializeField]
    [field: Range(0, 1)]
    public float BaseWeedSpawnChance { get; set; }
    
    [field: SerializeField]
    [field: Range(0, 1)]
    public float WeedSpawnChanceIncrease { get; set; }

    [field: SerializeField] public SeedValuesSo PlantedSeed { get; set; }
    public MeshFilter sproutMeshParent;
    public WeedBehaviour weedBehaviour;
    public Sprite wateredSprite;
    public Sprite needsWaterSprite;
    public Image wateringIndicator;
    public Image seedIndicator;

    [field: SerializeField] public bool UseEndPoint { get; set; }
    [field: SerializeField] public Transform EndPoint { get; set; }
    [field: SerializeField] public float heightShove { get; set; }

    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private float effectiveWeedSpawnChance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        InitPurchasableHouseItem();
        DisableInteract();
        SceneTransitionManager.instance.OnSleep.AddListener(ProgressDay);
        if (!GameDontDestroyOnLoadManager.Instance.codexIsUnlocked)
        {
            sproutMeshParent.gameObject.SetActive(false);
            EnableWeed();
        }
    }

    private void ProgressDay()
    {
        if (PlantedSeed)
        {
            if (NeedsWatering == false)
            {
                PlantGrowthProgression++;
            }
            NeedsWatering = true;
        }
        if (IsWeed)
            return;

        if (Random.value < effectiveWeedSpawnChance)
        {
            effectiveWeedSpawnChance = BaseWeedSpawnChance;
            EnableWeed();
        }
        else
        {
            effectiveWeedSpawnChance += WeedSpawnChanceIncrease;
        }
    }


    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }

    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }

    public void UpdateVisuals()
    {
        seedIndicator.sprite = PlantedSeed ? PlantedSeed.SeedIcon : null;
        wateringIndicator.sprite = NeedsWatering ? needsWaterSprite : wateredSprite;
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

        if (!Unlocked) return;

        var data = GardenManager.instance.plotsData[selfIndex + 1];
        NeedsWatering = data.NeedsWatering;
        PlantGrowthProgression = data.PlantGrowthProgression;
        RequiredProgressionToMature = data.RequiredProgressionToMature;
        PlantedSeed = data.PlantedSeed;
        IsWeed = data.IsWeed;
        BaseWeedSpawnChance = data.WeedSpawnChance;
        
    }

    public override void PurchaseItem()
    {
        base.PurchaseItem();

        //GardenManager.instance.plotsData[selfIndex].;
    }

    public void WaterPlot()
    {
        NeedsWatering = false;
        UpdateVisuals();

        GardenManager.instance.plotsData[selfIndex + 1].UpdateData(this);
    }

    public void AddSeed(CollectedSeedBehavior collectedSeedBehaviour)
    {
        PlantedSeed = collectedSeedBehaviour.SeedValuesSo;
        NeedsWatering = true;
        BaseWeedSpawnChance = 0;
        UpdateVisuals();

        GardenManager.instance.plotsData[selfIndex + 1].UpdateData(this);
    }


    public void EnableWeed()
    {
        weedBehaviour.EnableWeed();
        IsWeed = true;
        GardenManager.instance.plotsData[selfIndex + 1].UpdateData(this);
    }

    public void DisableWeed()
    {
        weedBehaviour.DisableWeed();
        IsWeed = false;
        GardenManager.instance.plotsData[selfIndex + 1].UpdateData(this);
    }

    public void HandlePlayerInput()
    {
        if (CanPurchase)
        {
            PurchaseItem();
            return;
        }

        if (IsWeed)
            return;
        
        if (!PlantedSeed)
        {
            if (CharacterInteractController.Instance.collectedStack.Count == 0)
                return;

            if (CharacterInteractController.Instance.collectedStack[0].StackableItem is not CollectedSeedBehavior)
                return;

            CharacterAnimManager.instance.CatThrow();

            AddSeed((CollectedSeedBehavior)CharacterInteractController.Instance.collectedStack[^1].StackableItem);
            if (CharacterInteractController.Instance.collectedStack.Count == 0)
                CharacterInteractController.Instance.AreHandsFull = false;

            CharacterInteractController.Instance.ShovePartialStackInTarget(transform, this, new []{CharacterInteractController.Instance.collectedStack[^1]});
            CharacterInteractController.Instance.collectedStack.RemoveAt(CharacterInteractController.Instance.collectedStack.Count - 1);
            
            return;
        }

        if (PlantGrowthProgression == RequiredProgressionToMature)
        {
            //TODO: Vegetable harvest haptic challenge
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

                if (IsWeed)
                {
                    weedBehaviour.EnableCollect(true);   
                }
                else
                {
                    EnableInteract();
                }
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

        if (IsWeed)
        {
            weedBehaviour.DisableUI();   
        }
        else
        {
            DisableInteract();
        }
        pricePopUpBehaviour.HidePrice();
    }

}