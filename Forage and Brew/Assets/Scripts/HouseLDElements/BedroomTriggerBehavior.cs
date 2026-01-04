using UnityEngine;

public class BedroomTriggerBehavior : MonoBehaviour
{
    public bool toBedroom;
    
    
    private void OnTriggerEnter(Collider other)
    {
        PinnedRecipe.instance.isInBedroom = toBedroom;
        PinnedRecipe.instance.InverseChangePos(toBedroom);
        
        if (toBedroom)
        {
            MusicManager.Instance.PlaySceneMucic(Scene.HouseOutdoor);
        }
    }
}
