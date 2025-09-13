using UnityEngine;

public class BedroomTriggerBehavior : MonoBehaviour
{

    public bool toBedroom;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {

        
        PinnedRecipe.instance.isInBedroom = toBedroom;
        PinnedRecipe.instance.InverseChangePos(toBedroom);
    }
}
