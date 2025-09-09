using UnityEngine;
using UnityEngine.UI;

public class OutfitButtonBehaviour : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button outfitButton;
    [SerializeField] private Image outfitImage;
    [SerializeField] private GameObject currentSelector;
    [SerializeField] private LockBehaviour lockBehaviour;
    [SerializeField] private GameObject selectCheckmark;
    
    public MirrorBehaviour MirrorBehaviour { get; set; }
    public CharacterOutfitSo OutfitSo { get; set; }


    public void Initialize(MirrorBehaviour mirrorBehaviour, CharacterOutfitSo outfit)
    {
        MirrorBehaviour = mirrorBehaviour;
        OutfitSo = outfit;
        outfitImage.sprite = outfit.OutfitSprite;
        
        DisableSelector();
        DisableCheckmark();
        DisableLock();
    }
    
    public void EnableSelector()
    {
        currentSelector.SetActive(true);
        outfitButton.Select();
    }
    
    public void DisableSelector()
    {
        currentSelector.SetActive(false);
    }

    public void EnableCheckmark()
    {
        selectCheckmark.SetActive(true);
    }
    
    public void DisableCheckmark()
    {
        selectCheckmark.SetActive(false);
    }

    public void EnableLock()
    {
        lockBehaviour.Enable();
    }
    
    public void DisableLock()
    {
        lockBehaviour.Disable();
    }
    
    public void UnlockOutfit()
    {
        lockBehaviour.Unlock();
    }
}
