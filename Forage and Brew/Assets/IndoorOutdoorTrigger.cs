using UnityEngine;
using UnityEngine.Audio;

public class IndoorOutdoorTrigger : MonoBehaviour
{
    //public float transitionTime = 1;

    public AudioSource musicSource;

    public AudioResource musicToPlay;

    public bool inHouse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (PinnedRecipe.instance.isInHouse  == inHouse) return;
        
        PinnedRecipe.instance.isInHouse = inHouse;

        if (PinnedRecipe.instance.isPinned)
        {
            PinnedRecipe.instance.PinRecipe();
        }
        MusicManager.Instance.PlaySceneMucic(Scene.HouseOutdoor);

    }
}
