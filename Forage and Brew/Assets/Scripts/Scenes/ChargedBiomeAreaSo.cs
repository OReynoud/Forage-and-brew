using UnityEngine;

[CreateAssetMenu(fileName = "D_ChargedBiomeArea", menuName = "Scenes/ChargedBiomeAreaSo")]
public class ChargedBiomeAreaSo : ScriptableObject
{
    [field: SerializeField] public int PurchaseCost { get; private set; }
}
