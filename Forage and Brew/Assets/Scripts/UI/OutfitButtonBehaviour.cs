using UnityEngine;
using UnityEngine.UI;

public class OutfitButtonBehaviour : MonoBehaviour
{
    [Header("UI")]
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
    }
    
    public void OnClickSelectOutfit()
    {
        CharacterSelectionManager.Instance.SetCurrentOutfit(OutfitSo);
        UpdateSelectedOutline();
    }

    public void UpdateSelectedOutline()
    {
        if (CharacterSelectionManager.Instance.CurrentOutfit == outfitSo)
        {
            selectedOutline.SetActive(true);
        }
        else
        {
            selectedOutline.SetActive(false);
        }
    }
}
