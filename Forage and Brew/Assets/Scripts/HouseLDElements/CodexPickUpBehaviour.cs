using System.Collections.Generic;
using UnityEngine;

public class CodexPickUpBehaviour : MonoBehaviour, ICinematicInteraction
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform locationToWalk;
    [SerializeField] private GameObject localCanvas;
    [SerializeField] private List<ParticleSystem> sparkleEffects;
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioSource audioPages;
    
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    
    private void Start()
    {
        if (GameDontDestroyOnLoadManager.Instance.codexIsUnlocked)
        {
            gameObject.SetActive(false);
        }
    }
    

    public void StartInteraction()
    {
        CharacterMovementController.Instance.TriggerWalkTransition(locationToWalk.position);
        CharacterMovementController.Instance.FinishWalkToLocation.AddListener(PickupCodex);
        CharacterInputManager.Instance.DisableInputs();
    }

    private void PickupCodex()
    {
        GameDontDestroyOnLoadManager.Instance.codexIsUnlocked = true;
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(PickupCodex);
        CharacterMovementController.Instance.transform.rotation = locationToWalk.rotation;
        CharacterInputManager.Instance.EnterCodexMethod();
        gameObject.SetActive(false);
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        CharacterInputManager.Instance.DisableInputs();
        //CharacterInputManager.Instance.EnableMoveInputs();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = this;
        localCanvas.SetActive(true);
        animator.SetBool(IsOpen, true);
        
        foreach (ParticleSystem sparkleEffect in sparkleEffects)
        {
            sparkleEffect.Play();
        }
        audio.Stop();
        audio.Play();
        audioPages.Stop();
        audioPages.PlayDelayed(0.4f);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        localCanvas.SetActive(false);
        animator.SetBool(IsOpen, false);
        
        foreach (ParticleSystem sparkleEffect in sparkleEffects)
        {
            sparkleEffect.Stop();
        }
        audio.Stop();
        audio.PlayDelayed(0.8f);
        audioPages.Stop();
        audioPages.PlayDelayed(0.6f);
    }
}
