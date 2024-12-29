using UnityEngine;

public class CountertopVfxManager : MonoBehaviour
{
    [Header("Chopping")]
    [SerializeField] private ParticleSystem chopVfx;
    
    
    public void PlayChopVfx()
    {
        chopVfx.Play();
    }
}
