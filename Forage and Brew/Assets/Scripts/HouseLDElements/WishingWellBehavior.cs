using UnityEngine;

public class WishingWellBehavior : MonoBehaviour, ICinematicInteraction
{
    private static readonly int DoWish = Animator.StringToHash("DoWish");
    public float distance;
    public GameObject localCanvas;
    private bool doneWish = false;

    private void OnTriggerEnter(Collider other)
    {
        if (doneWish)
        {
            return;
        }
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.CurrentNearCinematicInteraction = this;
            localCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {        
        if (doneWish)
        {
            return;
        }
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            if (characterInteractController.CurrentNearCinematicInteraction == (ICinematicInteraction)this)
            {
                characterInteractController.CurrentNearCinematicInteraction = null;
                localCanvas.SetActive(false);
            }
        }
    }

    public void StartInteraction()
    {         
        if (doneWish)
        {
            return;
        }
        Vector3 posToLook = new Vector3(transform.position.x,
            CharacterAnimManager.instance.transform.position.y, transform.position.z);
        CharacterAnimManager.instance.transform.LookAt(posToLook);
        CharacterMovementController.Instance.TriggerWalkTransition(posToLook - CharacterAnimManager.instance.transform.forward * distance);
        CharacterAnimManager.instance.animator.SetTrigger(DoWish);
        CharacterMovementController.Instance.FinishWalkToLocation.AddListener(Wish);
        localCanvas.SetActive(false);
        doneWish = true;
        this.enabled = false;
    }

    void Wish()
    {
        CharacterAnimManager.instance.animator.SetTrigger(DoWish);
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(Wish);
    }
}
