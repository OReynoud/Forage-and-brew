using System;
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
    [SerializeField] private CanvasGroup tutorialBackground;
    [SerializeField] private float timeBeforeLerp;
    [SerializeField] private float lerpTime;
    [SerializeField] private float timer;
    [SerializeField] private bool startDelay;
    [SerializeField] private bool startBackGroundLerp;
    public static bool doTutorialPages = true;
    public static UnityEvent codexTutorialEvent;
    
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    
    private void Start()
    {
        if (GameDontDestroyOnLoadManager.Instance.codexIsUnlocked)
        {
            doTutorialPages = false;
            gameObject.SetActive(false);
            Destroy(tutorialBackground.gameObject);
        }
        else
        {
            doTutorialPages = true;
            AutoFlip.instance.ControledBook.OnFlip.AddListener(FlipListener);
            CharacterInputManager.Instance.OnCodexUse.AddListener(DestroyTutorialBackground);
        }
    }

    void DestroyTutorialBackground(bool state)
    {
        if (!state)
        {
            Destroy(tutorialBackground.gameObject, 0.4f);
            CharacterInputManager.Instance.OnCodexUse.RemoveListener(DestroyTutorialBackground);
        }
    }

    private void Update()
    {
        if (startBackGroundLerp)
        {
            timer += Time.deltaTime;
            tutorialBackground.alpha = Mathf.Lerp(0, 1, timer / lerpTime);
            if (timer > lerpTime)
            {
                startDelay = false;
                startBackGroundLerp = false;
            }
            return;
        }
        if (startDelay)
        {
            timer += Time.deltaTime;
            if (timer > timeBeforeLerp)
            {
                doTutorialPages = false;
                InfoDisplayManager.instance.canShowCodex = true;
                CharacterInputManager.Instance.EnableCodexInputs();
                CharacterInputManager.Instance.EnableCodexExit();
                startBackGroundLerp = true;
                timer = 0;
            }
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
        AutoFlip.instance.TutorialPagesDiscoveryStart();
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(PickupCodex);
        CharacterMovementController.Instance.transform.rotation = locationToWalk.rotation;

        animator.gameObject.SetActive(false);
        GameDontDestroyOnLoadManager.Instance.codexIsUnlocked = true;
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        CharacterInputManager.Instance.DisableInputs();
        CharacterInputManager.Instance.EnterCodexMethod();
    }

    public void FlipListener()
    {
        if (CodexContentManager.instance.tutorialDissolvesToCheck.Count == 0)
        {
            startDelay = true;
            timer = 0;
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
