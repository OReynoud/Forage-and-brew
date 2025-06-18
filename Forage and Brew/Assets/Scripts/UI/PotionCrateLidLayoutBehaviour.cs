using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionCrateLidLayoutBehaviour : MonoBehaviour
{
    [SerializeField] private Image letterImage;
    [SerializeField] private List<PotionCheckBehaviour> potionCheckBehaviours;
    
    
    public void EnablePotionCheckMark(int index)
    {
        if (index < 0 || index >= potionCheckBehaviours.Count)
        {
            Debug.LogWarning("Index out of range for potion check marks.");
            return;
        }
        
        potionCheckBehaviours[index].EnableCheckMark();
    }
    
    public void DisablePotionCheckMarks()
    {
        foreach (PotionCheckBehaviour potionCheckBehaviour in potionCheckBehaviours)
        {
            potionCheckBehaviour.DisableCheckMark();
        }
    }
    
    
    public void SetLetterImageColor(Color color)
    {
        letterImage.color = color;
    }
}
