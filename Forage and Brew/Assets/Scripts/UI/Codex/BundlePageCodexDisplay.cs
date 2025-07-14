using System;
using System.Collections.Generic;
using UnityEngine;

public class BundlePageCodexDisplay : PageBehavior
{
    public List<BundleContainerCodexDisplay> allContainers = new();
    private int activationIndex;

    public void Awake()
    {
        foreach (var container in allContainers)
        {
            container.gameObject.SetActive(false);
        }
    }

    public override void InitBundlesPage(PotionEnsembleSo bundleToDisplay)
    {
        
        InitNewBundle(bundleToDisplay);
    }

    public void InitNewBundle(PotionEnsembleSo bundleToDisplay)
    {
        allContainers[activationIndex].gameObject.SetActive(true);
        allContainers[activationIndex].InitBundle(bundleToDisplay);
        activationIndex++;


        
    }
}
