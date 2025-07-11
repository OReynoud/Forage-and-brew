using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PotionEnsembleBehaviour : MonoBehaviour, IPotionAddable
{
    [Header("Dependencies")]
    [SerializeField] private List<Transform> meshParentTransforms;
    [field: SerializeField] public PotionEnsembleSo PotionEnsembleSo { get; private set; }
    [SerializeField] private GameObject environmentRewardObject;
    
    public PotionEnsembleManager PotionEnsembleManager => PotionEnsembleManager.Instance;
    
    public bool IsDiscovered { get; private set; }
    public int ContainedPotions { get; private set; }
    
    [Header("UI")]
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private GameObject popupCanvasGameObject;
    [SerializeField] private TMP_Text potionEnsembleNameText;
    [SerializeField] private PotionDemandElementBehaviour potionElementPrefab;
    [SerializeField] private Transform potionElementParentTransform;
    private readonly List<PotionDemandElementBehaviour> _potionElements = new();
    [SerializeField] private TMP_Text priceText;
    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
        DisablePopup();
        
        environmentRewardObject.SetActive(false);
        
        InitEnsemble();
    }
    
    private void OnDisable()
    {
        if (CharacterInteractController.Instance.CurrentNearPotionEnsemble == this)
        {
            CharacterInteractController.Instance.CurrentNearPotionEnsemble = null;
        }
        
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
    
    
    public void EnablePopup()
    {
        popupCanvasGameObject.SetActive(true);
    }
    
    public void DisablePopup()
    {
        popupCanvasGameObject.SetActive(false);
    }


    public void InitEnsemble()
    {
        potionEnsembleNameText.text = PotionEnsembleSo.Name;
        
        for (int i = 0; i < PotionEnsembleSo.Potions.Count; i++)
        {
            PotionDemandElementBehaviour elementBehaviour = Instantiate(potionElementPrefab, potionElementParentTransform);
            
            elementBehaviour.SetImage(PotionEnsembleSo.Potions[i]);
            
            _potionElements.Add(elementBehaviour);
        }

        priceText.text = PotionEnsembleSo.MoneyReward.ToString();
        
        IsDiscovered = GameDontDestroyOnLoadManager.Instance.UnlockedPotionEnsembles.ContainsKey(PotionEnsembleSo) &&
                        GameDontDestroyOnLoadManager.Instance.UnlockedPotionEnsembles[PotionEnsembleSo] > 0;
        
        if (IsDiscovered)
        {
            for (int i = 0; i < PotionEnsembleSo.Potions.Count; i++)
            {
                if ((GameDontDestroyOnLoadManager.Instance.UnlockedPotionEnsembles[PotionEnsembleSo] & (1 << (i + 1))) == 0) continue;

                PotionLiquidColorManager potionLiquidColorManager = Instantiate(PotionEnsembleSo.Potions[i]
                    .PotionDifficulty.MeshGameObjectLiquidColorManager, meshParentTransforms[i]);
                potionLiquidColorManager.SetLiquidColor(PotionEnsembleSo.Potions[i]);
                
                _potionElements[i].EnableCheckMark();
            }
        }
        
        CheckCompletion();
    }

    
    public void AddPotion(CollectedPotionBehaviour collectedPotionBehaviour)
    {
        int potionIndex = PotionEnsembleSo.Potions.IndexOf(collectedPotionBehaviour.PotionValuesSo);
        
        ContainedPotions |= 1 << (potionIndex + 1);
        GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Remove(collectedPotionBehaviour);
        collectedPotionBehaviour.OnPotionDropEnd.AddListener(DestroyPotion);
        
        PotionLiquidColorManager potionLiquidColorManager = Instantiate(PotionEnsembleSo.Potions[potionIndex]
            .PotionDifficulty.MeshGameObjectLiquidColorManager, meshParentTransforms[potionIndex]);
        potionLiquidColorManager.SetLiquidColor(PotionEnsembleSo.Potions[potionIndex]);
        _potionElements[potionIndex].EnableCheckMark();
        // TODO: Add new checkmark in codex here

        foreach (var bundlePage in CodexContentManager.instance.bundlesPages)
        {
            foreach (var bundle in bundlePage.allContainers)
            {
                if (!bundle.isInitialized)continue;
                if (bundle.displayedBundle == PotionEnsembleSo)
                {
                    bundle.UpdateCheckmarks(potionIndex);
                    break;
                }
            }
        }
        
        
        CheckCompletion();
        
        DisableInteract();
    }
    
    private void DestroyPotion(CollectedPotionBehaviour collectedPotionBehaviour)
    {
        Destroy(collectedPotionBehaviour.gameObject);
    }
    

    public bool CheckPotion(PotionValuesSo collectedPotionSo)
    {
        int mask = 0;
        
        for (int i = 0; i < PotionEnsembleSo.Potions.Count; i++)
        {
            mask |= 1 << (i + 1);
        }
        
        if ((ContainedPotions & mask) == mask) return false;

        for (int i = 0; i < PotionEnsembleSo.Potions.Count; i++)
        {
            if (PotionEnsembleSo.Potions[i] == collectedPotionSo)
            {
                if ((ContainedPotions & (1 << (i + 1))) != 0) return false;
                
                return true;
            }
        }

        return false;
    }

    public Vector3 GetRightTransformLocalPosition(PotionValuesSo collectedPotionSo)
    {
        return meshParentTransforms[PotionEnsembleSo.Potions.IndexOf(collectedPotionSo)].localPosition;
    }
    
    public void CheckCompletion(bool justDropped = false)
    {
        if ((ContainedPotions & 1) == 0) return;
        
        int mask = 0;
        
        for (int i = 0; i < PotionEnsembleSo.Potions.Count; i++)
        {
            mask |= 1 << (i + 1);
        }

        if ((ContainedPotions & mask) == mask)
        {
            environmentRewardObject.SetActive(true);
            
            if (justDropped)
            {
                MoneyManager.Instance.AddMoney(PotionEnsembleSo.MoneyReward);
            }
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            EnablePopup();
            
            IsDiscovered = true;
            // TODO: Add discover behaviour here
            CodexContentManager.instance.AddNewBundleToCodex(PotionEnsembleSo);
            
            if (characterInteractController.collectedStack.Count > 0 &&
                characterInteractController.collectedStack[0].stackable is CollectedPotionBehaviour)
            {
                characterInteractController.CurrentNearPotionEnsemble = this;
                
                if (CheckPotion(((CollectedPotionBehaviour)characterInteractController.collectedStack[0].stackable).PotionValuesSo))
                {
                    EnableInteract();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            DisablePopup();
            DisableInteract();
            
            if (characterInteractController.CurrentNearPotionEnsemble == this)
            {
                characterInteractController.CurrentNearPotionEnsemble = null;
            }
        }
    }
}
