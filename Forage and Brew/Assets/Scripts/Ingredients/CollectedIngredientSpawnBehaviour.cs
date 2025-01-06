using UnityEngine;

public class CollectedIngredientSpawnBehaviour : MonoBehaviour
{
    [SerializeField] private IngredientTypeSo ingredientType;
    [SerializeField] private CollectedIngredientBehaviour collectedIngredientBehaviourPrefab;
    
    private void Start()
    {
        foreach (IngredientValuesSo collectedIngredient in GameDontDestroyOnLoadManager.Instance.CollectedIngredients)
        {
            if (collectedIngredient.Type == ingredientType)
            {
                CollectedIngredientBehaviour collectedIngredientBehaviour =
                    Instantiate(collectedIngredientBehaviourPrefab, transform);
                collectedIngredientBehaviour.IngredientValuesSo = collectedIngredient;
            }
        }
    }
}
