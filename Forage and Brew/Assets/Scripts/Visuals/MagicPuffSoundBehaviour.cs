using UnityEngine;

public class MagicPuffSoundBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource magicPuffAudioSource;
    
    public void PlayMagicPuffSound()
    {
        magicPuffAudioSource.Play();
    }
}
