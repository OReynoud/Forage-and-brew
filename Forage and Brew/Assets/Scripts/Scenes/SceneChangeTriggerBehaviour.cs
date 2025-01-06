using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTriggerBehaviour : MonoBehaviour
{
    [SerializeField] private SceneListSo sceneListSo;
    [SerializeField] private Scene scene;
    [SerializeField] private bool doesNeedToBeDaytime;
    [SerializeField] private bool doesMakeItNighttime;
    
    private void OnTriggerEnter(Collider other)
    {
        if (CharacterInteractController.Instance.AreHandsFull) return;
        
        if (doesNeedToBeDaytime && GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay != TimeOfDay.Daytime) return;
        
        if (other.CompareTag("Player") && GameDontDestroyOnLoadManager.Instance.PreviousScene != scene)
        {
            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Scene == scene)
                {
                    SceneTransitionManager.instance.HandleSceneChange(sceneName.Name);
                    break;
                }
            }

            foreach (CollectedIngredientBehaviour collectedIngredientBehaviour in GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.ToList())
            {
                GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Remove(collectedIngredientBehaviour);
                GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.Add((collectedIngredientBehaviour.IngredientValuesSo,
                    collectedIngredientBehaviour.transform.position, collectedIngredientBehaviour.transform.rotation));
            }

            foreach (CollectedPotionBehaviour collectedPotionBehaviour in GameDontDestroyOnLoadManager.Instance.OutCookedPotions.ToList())
            {
                GameDontDestroyOnLoadManager.Instance.OutCookedPotions.Remove(collectedPotionBehaviour);
                GameDontDestroyOnLoadManager.Instance.FloorCookedPotions.Add((collectedPotionBehaviour.PotionValuesSo,
                    collectedPotionBehaviour.transform.position, collectedPotionBehaviour.transform.rotation));
            }
            
            if (doesMakeItNighttime)
            {
                GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay = TimeOfDay.Nighttime;
                Debug.Log("It's nighttime now");
            }
        }
    }
}
