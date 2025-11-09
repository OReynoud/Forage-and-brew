using UnityEngine;

[CreateAssetMenu(fileName = "D_SeedValues", menuName = "Seeds/SeedValuesSo")]
public class SeedValuesSo : StackableValuesSo
{
    [field: SerializeField] public IngredientTypeSo RequiredIngredientType { get; private set; }
    
    [field: SerializeField] public int DaysToMature { get; private set; }
    [field: SerializeField] public int RequiredIngredientTypeAmount { get; private set; }
    [field: SerializeField] public IngredientValuesSo IngredientToGrowSo { get; private set; }
}
