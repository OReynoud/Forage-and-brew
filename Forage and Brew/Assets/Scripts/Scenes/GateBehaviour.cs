using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GateBehaviour : MonoBehaviour
{
    [SerializeField] private List<ChargedBiomeAreaSo> chargedBiomeAreaSos;
    [SerializeField] private PricePopUpBehaviour pricePopUpBehaviour;
    [SerializeField] private List<Renderer> gateMeshRenderers;
    [SerializeField] private DissolveSettingsSo unlockDissolveSettingsSo;
    
    public bool Unlocked { get; private set; }
    
    private static readonly int CutoffHeight = Shader.PropertyToID("_CutoffHeight");
    private static readonly int CutoffHeight2 = Shader.PropertyToID("_Cutoff_Height");
    private static readonly int CatNo = Animator.StringToHash("DoNo");
    
    
    private void Start()
    {
        InitChargedGate();

        if (chargedBiomeAreaSos.Count == 0)
        {
            Unlocked = true;
        
            gameObject.SetActive(false);
        }
        else
        {
            pricePopUpBehaviour.SetPrice(chargedBiomeAreaSos[0].PurchaseCost);
            pricePopUpBehaviour.HidePrice();
        
            GateManager.Instance.OnAreaPurchased.AddListener(InitChargedGate);
            InitChargedGate();
        }
    }
    
    
    public void InitChargedGate()
    {
        foreach (ChargedBiomeAreaSo chargedBiomeAreaSo in chargedBiomeAreaSos.ToList())
        {
            if (GameDontDestroyOnLoadManager.Instance.UnlockedChargedBiomeAreas.Contains(chargedBiomeAreaSo))
            {
                chargedBiomeAreaSos.Remove(chargedBiomeAreaSo);
            }
        }
    }
    

    public void Purchase()
    {
        if (MoneyManager.Instance.MoneyAmount < chargedBiomeAreaSos[0].PurchaseCost)
        {
            CharacterAnimManager.instance.animator.SetTrigger(CatNo);
            return;
        }

        MoneyManager.Instance.SubtractMoney(chargedBiomeAreaSos[0].PurchaseCost);
        Unlocked = true;
        GameDontDestroyOnLoadManager.Instance.UnlockedChargedBiomeAreas.Add(chargedBiomeAreaSos[0]);
        GateManager.Instance.OnAreaPurchased.RemoveListener(InitChargedGate);
        GateManager.Instance.OnAreaPurchased.Invoke();
        pricePopUpBehaviour.HidePrice();
        
        foreach (Renderer gateMeshRenderer in gateMeshRenderers)
        {
            gateMeshRenderer.material.DOFloat(gateMeshRenderer is MeshRenderer ? 0f : 1f,
                    gateMeshRenderer is MeshRenderer ? CutoffHeight : CutoffHeight2, 
                    unlockDissolveSettingsSo.AnimationDuration).SetEase(unlockDissolveSettingsSo.AnimationCurve)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!Unlocked)
        {
            if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
                characterInteractController.collectedStack.Count == 0)
            {
                pricePopUpBehaviour.ShowPrice(chargedBiomeAreaSos[0].PurchaseCost);
                
                characterInteractController.CurrentNearChargedGate = this;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.CurrentNearChargedGate == this)
        {
            characterInteractController.CurrentNearChargedGate = null;
        }
        
        pricePopUpBehaviour.HidePrice();
    }
}
