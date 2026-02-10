using System.Collections.Generic;
using UnityEngine;

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
        
        if (other.CompareTag("Player") && GameDontDestroyOnLoadManager.Instance.CurrentScene != scene)
        {
            foreach (SceneName sceneName in sceneListSo.SceneNames)
            {
                if (sceneName.Scene == scene)
                {
                    SceneTransitionManager.instance.HandleSceneChange(sceneName.Name);
                    break;
                }
            }

            OutStackableManager.Instance?.StoreOutCollectedIngredients();
            OutStackableManager.Instance?.StoreOutCookedPotions(GameDontDestroyOnLoadManager.Instance.FloorCookedPotions,
                GameDontDestroyOnLoadManager.Instance.OutCookedPotions);
            
            if (PotionCrateManager.Instance)
            {
                for (int i = 0; i < PotionCrateManager.Instance.PotionCrates.Count; i++)
                {
                    if (OrderManager.Instance.CurrentOrders[i] == null) continue;
                    if (OrderManager.Instance.CurrentOrders[i].OrderContent == null) continue;
                    
                    GameDontDestroyOnLoadManager.Instance.OrderPotions[i] = new ClientOrderPotions(
                        OrderManager.Instance.CurrentOrders[i].OrderContent,
                        OrderManager.Instance.CurrentOrders[i].RelatedLetter.Client,
                        new List<FloorCookedPotion>());
                    OutStackableManager.Instance?.StoreOutCookedPotions(
                        GameDontDestroyOnLoadManager.Instance.OrderPotions[i].Potions,
                        PotionCrateManager.Instance.PotionCrates[i].ContainedPotions);
                }
            }

            PotionEnsembleManager.Instance?.StorePotionEnsembles();
            
            if (doesMakeItNighttime)
            {
                GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay = TimeOfDay.Nighttime;
                // Debug.Log("It's nighttime now");
            }
        }
    }
}
