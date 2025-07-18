using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CodexPickUpBehaviour : MonoBehaviour, ICinematicInteraction
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform locationToWalk;
    [SerializeField] private GameObject localCanvas;
    [SerializeField] private List<ParticleSystem> sparkleEffects;
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioSource audioPages;
    public static bool doTutorialPages = true;
    public static UnityEvent codexTutorialEvent;
    
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    
    private void Start()
    {
        if (GameDontDestroyOnLoadManager.Instance.codexIsUnlocked)
        {
            doTutorialPages = false;
            gameObject.SetActive(false);
        }
        else
        {
            doTutorialPages = true;
            AutoFlip.instance.ControledBook.OnFlip.AddListener(FlipListener);
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
        AutoFlip.instance.TutorialPagesDiscoveryStart();
        gameObject.SetActive(false);
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        CharacterInputManager.Instance.DisableInputs();
    }

    void FlipListener()
    {
        if (AutoFlip.instance.ControledBook.currentPage == AutoFlip.instance.ControledBook.bookPages.Count - 2)
        {
            doTutorialPages = false;
            InfoDisplayManager.instance.canShowCodex = true;
            CharacterInputManager.Instance.EnableCodexInputs();
            CharacterInputManager.Instance.EnableCodexExit();
            AutoFlip.instance.ControledBook.OnFlip.RemoveListener(FlipListener);
        }
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
        if ((ICinematicInteraction)this == CharacterInteractController.Instance.CurrentNearCinematicInteraction)
        {
            CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        }
        
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
