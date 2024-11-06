using UnityEngine;

public class CollectedIngredientSpawnBehaviour : MonoBehaviour
{
    [SerializeField] private IngredientType ingredientType;
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
