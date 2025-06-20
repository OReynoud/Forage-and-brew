using System;
using UnityEngine;

public class WallBigNope : MonoBehaviour, ICinematicInteraction
{
    public Animator animator;
    public Transform lookLocation;
    public Transform walkLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (CodexContentManager.instance.codexIsUnlocked)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CodexContentManager.instance.codexIsUnlocked)
            return;
        StartInteraction();
    }

    public void StartInteraction()
    {
        CharacterInputManager.Instance.DisableInputs();
        animator.SetTrigger("DoNo");
        
        CharacterMovementController.Instance.FinishWalkToLocation.AddListener(GiveControlsBack);
        NoEndBehavior.OnFinish.AddListener(WalkToBook);
        
    }

    public void WalkToBook()
    {
        CharacterMovementController.Instance.TriggerWalkTransition(walkLocation.position + Vector3.forward);
        NoEndBehavior.OnFinish.RemoveListener(WalkToBook);
    }

    public void GiveControlsBack()
    {
        CharacterInputManager.Instance.EnableInputs();
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(GiveControlsBack);
        CharacterMovementController.Instance.transform.LookAt(new Vector3(lookLocation.position.x,CharacterMovementController.Instance.transform.position.y,lookLocation.position.z));
    } 
}
