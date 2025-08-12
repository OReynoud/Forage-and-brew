using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GardenPlotBehavior : MonoBehaviour
{
    [field: SerializeField] public bool NeedsWatering { get; set; }
    [field: SerializeField] public int PlantGrowthProgression { get; set; }
    [field: SerializeField] public int RequiredProgressionToMature { get; set; }
    [field: SerializeField] public SeedValuesSo PlantedSeed { get; set; }
    public Sprite wateredSprite;
    public Sprite needsWaterSprite;
    public Image wateringIndicator;
    public Image seedIndicator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
