using System.Collections.Generic;
using UnityEngine;

public class PotionEnsembleManager : MonoBehaviour
{
    public static PotionEnsembleManager Instance { get; private set; }
    
    [SerializeField] private List<PotionEnsembleBehaviour> potionEnsembles = new();


    private void Awake()
    {
        Instance = this;
    }


    public void StorePotionEnsembles()
    {
        foreach (PotionEnsembleBehaviour potionEnsemble in potionEnsembles)
        {
            GameDontDestroyOnLoadManager.Instance.UnlockedPotionEnsembles[potionEnsemble.PotionEnsembleSo] =
                potionEnsemble.IsDiscovered ? 1 : 0 | potionEnsemble.ContainedPotions;
        }
    }
}
