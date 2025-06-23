using UnityEngine;

public class CouchBehavior : MonoBehaviour, ICinematicInteraction
{
    public Animator animator;
    private bool usingCouch;

    public Transform locationToWalk;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }    
    private void OnTriggerEnter(Collider other)
    {
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = this;
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
    }

    public void StartInteraction()
    {
        usingCouch = !usingCouch;
        CharacterInputManager.Instance.DisableInputs();
        if (usingCouch)
        {
            CharacterMovementController.Instance.TriggerWalkTransition(locationToWalk.position);
            CharacterMovementController.Instance.FinishWalkToLocation.AddListener(SitOnCouch);
        }
        else
        {
            animator.SetTrigger("DoStand");
        }
    }

    private void SitOnCouch()
    {
        CharacterAnimManager.instance.transform.LookAt(new Vector3(transform.position.x,CharacterAnimManager.instance.transform.position.y,transform.position.z));
        CharacterAnimManager.instance.animator.SetTrigger("DoSit");
        CharacterAnimManager.instance.UseCouch();
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(SitOnCouch);
    }
}
