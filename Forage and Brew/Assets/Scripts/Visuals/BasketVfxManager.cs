using UnityEngine;

public class BasketVfxManager : MonoBehaviour
{
    [Header("Smokescreen")]
    [SerializeField] private ParticleSystem smokescreenParticleSystem;
    
    
    public void PlaySmokescreen()
    {
        smokescreenParticleSystem.Play();
    }
}
