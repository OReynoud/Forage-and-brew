using UnityEngine;

public class CauldronVfxManager : MonoBehaviour
{
    // Singleton
    public static CauldronVfxManager Instance { get; private set; }

    [Header("Flames")]
    [SerializeField] private ParticleSystem flameVfx;
    [SerializeField] private ParticleSystemRenderer flameVfxRenderer;
    [SerializeField] [ColorUsage(true, true)] private Color lowHeatFlameColor = new(191/255f, 16/255f, 0/255f, 1);

    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }


    public void ChangeTemperatureVfx(Temperature temperature)
    {
        switch (temperature)
        {
            case Temperature.LowHeat:
                flameVfxRenderer.material.SetColor("_EmissionColor", lowHeatFlameColor);
                // flameVfxRenderer.material.GetColor("
                break;
        }
    }
}
