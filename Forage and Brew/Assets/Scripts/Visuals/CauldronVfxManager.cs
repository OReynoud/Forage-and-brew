using UnityEngine;

public class CauldronVfxManager : MonoBehaviour
{
    // Singleton
    public static CauldronVfxManager Instance { get; private set; }

    [Header("Smoke")]
    [SerializeField] private ParticleSystem smokeVfxParent;

    [Header("Flames")]
    [SerializeField] private ParticleSystem flameVfxParent;
    [SerializeField] private ParticleSystem flameVfx;
    [SerializeField] private ParticleSystemRenderer flameVfxRenderer;
    [SerializeField] [ColorUsage(true, true)] private Color lowHeatFlameColor;
    [SerializeField] private float lowHeatFlameRadius;
    [SerializeField] private float lowHeatFlameDonutRadius;
    [SerializeField] [ColorUsage(true, true)] private Color mediumHeatFlameColor;
    [SerializeField] private float mediumHeatFlameRadius;
    [SerializeField] private float mediumHeatFlameDonutRadius;
    [SerializeField] [ColorUsage(true, true)] private Color highHeatFlameColor;
    [SerializeField] private float highHeatFlameRadius;
    [SerializeField] private float highHeatFlameDonutRadius;
    
    // Shader properties
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    
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
        ParticleSystem.ShapeModule flameVfxShape = flameVfx.shape;
        
        switch (temperature)
        {
            case Temperature.LowHeat:
                flameVfxParent.Play();
                flameVfxRenderer.material.SetColor(EmissionColor, lowHeatFlameColor);
                flameVfxShape.radius = lowHeatFlameRadius;
                flameVfxShape.donutRadius = lowHeatFlameDonutRadius;
                break;
            case Temperature.MediumHeat:
                flameVfxParent.Play();
                flameVfxRenderer.material.SetColor(EmissionColor, mediumHeatFlameColor);
                flameVfxShape.radius = mediumHeatFlameRadius;
                flameVfxShape.donutRadius = mediumHeatFlameDonutRadius;
                break;
            case Temperature.HighHeat:
                flameVfxParent.Play();
                flameVfxRenderer.material.SetColor(EmissionColor, highHeatFlameColor);
                flameVfxShape.radius = highHeatFlameRadius;
                flameVfxShape.donutRadius = highHeatFlameDonutRadius;
                break;
            case Temperature.None:
                flameVfxParent.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                break;
        }
    }


    public void ChangeSmokeVfx(bool hasSmoke)
    {
        if (hasSmoke)
        {
            smokeVfxParent.Play();
        }
        else
        {
            smokeVfxParent.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
