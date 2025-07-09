using UnityEngine;

public class BundlePageCodexDisplay : PageBehavior
{
    public BundleContainerCodexDisplay[] allContainers;
    private int activationIndex;


    public override void InitBundlesPage(PotionEnsembleSo bundleToDisplay)
    {
        foreach (var container in allContainers)
        {
            container.gameObject.SetActive(false);
        }
        
        allContainers[0].gameObject.SetActive(true);
        
        InitNewBundle(bundleToDisplay);
    }

    public void InitNewBundle(PotionEnsembleSo bundleToDisplay)
    {
        allContainers[activationIndex].InitBundle(bundleToDisplay);
        activationIndex++;
    }
}
