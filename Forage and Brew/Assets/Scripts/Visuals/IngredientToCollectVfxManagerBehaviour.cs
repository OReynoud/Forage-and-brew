using UnityEngine;

public class IngredientToCollectVfxManagerBehaviour : MonoBehaviour
{
    [Header("Lunar Cycle")]
    [SerializeField] private LunarCycleStateSo shootingStarLunarCycleState;
    [SerializeField] private GameObject shootingStarVfxGameObject;
    [SerializeField] private LunarCycleStateSo newMoonLunarCycleState;
    [SerializeField] private GameObject newMoonVfxGameObject;
    [SerializeField] private LunarCycleStateSo halfMoonLunarCycleState;
    [SerializeField] private GameObject halfMoonVfxGameObject;
    [SerializeField] private LunarCycleStateSo fullMoonLunarCycleState;
    [SerializeField] private GameObject fullMoonVfxGameObject;


    private void Start()
    {
        SetRightVfx();
    }


    public void SetRightVfx()
    {
        if (LunarCycleManager.Instance.CurrentLunarCycleState == shootingStarLunarCycleState)
        {
            PlayShootingStarVfx();
        }
        else if (LunarCycleManager.Instance.CurrentLunarCycleState == newMoonLunarCycleState)
        {
            PlayNewMoonVfx();
        }
        else if (LunarCycleManager.Instance.CurrentLunarCycleState == halfMoonLunarCycleState)
        {
            PlayHalfMoonVfx();
        }
        else if (LunarCycleManager.Instance.CurrentLunarCycleState == fullMoonLunarCycleState)
        {
            PlayFullMoonVfx();
        }
    }
    
    public void PlayShootingStarVfx()
    {
        shootingStarVfxGameObject.SetActive(true);
        newMoonVfxGameObject.SetActive(false);
        halfMoonVfxGameObject.SetActive(false);
        fullMoonVfxGameObject.SetActive(false);
    }
    
    public void PlayNewMoonVfx()
    {
        shootingStarVfxGameObject.SetActive(false);
        newMoonVfxGameObject.SetActive(true);
        halfMoonVfxGameObject.SetActive(false);
        fullMoonVfxGameObject.SetActive(false);
    }
    
    public void PlayHalfMoonVfx()
    {
        shootingStarVfxGameObject.SetActive(false);
        newMoonVfxGameObject.SetActive(false);
        halfMoonVfxGameObject.SetActive(true);
        fullMoonVfxGameObject.SetActive(false);
    }
    
    public void PlayFullMoonVfx()
    {
        shootingStarVfxGameObject.SetActive(false);
        newMoonVfxGameObject.SetActive(false);
        halfMoonVfxGameObject.SetActive(false);
        fullMoonVfxGameObject.SetActive(true);
    }
}
