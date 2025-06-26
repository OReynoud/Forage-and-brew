using System;
using UnityEngine;

public class CodexPickUpBehaviour : MonoBehaviour, ICinematicInteraction
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator animator;
    public Transform LocationToWalk;
    public GameObject localCanvas;
    
    void Start()
    {
        if (GameDontDestroyOnLoadManager.Instance.codexIsUnlocked)
            gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = this;
        localCanvas.SetActive(true);
        animator.SetBool("IsOpen",true);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        localCanvas.SetActive(false);
        animator.SetBool("IsOpen",false);
    }

    public void StartInteraction()
    {
        CharacterMovementController.Instance.TriggerWalkTransition(LocationToWalk.position);
        CharacterMovementController.Instance.FinishWalkToLocation.AddListener(PickupCodex);
        CharacterInputManager.Instance.DisableInputs();
    }

    private void PickupCodex()
    {
        GameDontDestroyOnLoadManager.Instance.codexIsUnlocked = true;
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(PickupCodex);
        CharacterInputManager.Instance.EnableInputs();
        CharacterMovementController.Instance.transform.rotation = LocationToWalk.rotation;
        CharacterInputManager.Instance.EnterCodexMethod();
        gameObject.SetActive(false);
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        
        CharacterInputManager.Instance.EnableMoveInputs();

    }
}
