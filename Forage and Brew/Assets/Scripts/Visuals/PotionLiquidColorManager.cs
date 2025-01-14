using UnityEngine;

public class PotionLiquidColorManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer liquidMeshRenderer;
    
    public void SetLiquidColor(PotionValuesSo potionValuesSo)
    {
        liquidMeshRenderer.material.SetColor("_BottomColor", potionValuesSo.MeshLiquidMainColor);
        liquidMeshRenderer.material.SetColor("_TopColor", potionValuesSo.MeshLiquidTopColor);
        liquidMeshRenderer.material.SetColor("_FoamColor", potionValuesSo.MeshLiquidFoamColor);
        liquidMeshRenderer.material.SetColor("_Rim_Color", potionValuesSo.MeshLiquidRimColor);
    }
}
