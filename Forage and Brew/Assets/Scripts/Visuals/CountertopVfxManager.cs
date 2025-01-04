using UnityEngine;

public class CountertopVfxManager : MonoBehaviour
{
    [Header("Chopping")]
    [SerializeField] private ParticleSystem chopVfx;
    
    [Header("Grinding")]
    [SerializeField] private ParticleSystem crushVfx;
    
    
    public void PlayChopVfx()
    {
        chopVfx.Play();
    }
    
    
    public void PlayCrushVfx()
    {
        crushVfx.Play();
    }
}
