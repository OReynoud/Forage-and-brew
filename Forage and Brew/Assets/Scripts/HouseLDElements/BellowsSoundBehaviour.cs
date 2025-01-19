using UnityEngine;

public class BellowsSoundBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource blowSoundAudioSource;
    
    
    public void PlayBlowSound()
    {
        blowSoundAudioSource.Play();
    }
}
