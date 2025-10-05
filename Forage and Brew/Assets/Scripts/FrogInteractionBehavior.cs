using UnityEngine;

public class FrogInteractionBehavior : MonoBehaviour, ICinematicInteraction
{

    public Animator animator;

    public float distance;
    public ParticleSystem particleSystem;

    public GameObject localCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController))
        {
            characterInteractController.CurrentNearCinematicInteraction = this;
            localCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {        
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
        CharacterInputManager.Instance.DisableMoveInputs();
        CharacterInputManager.Instance.DisableInteractInputs();
        CharacterInputManager.Instance.DisableCodexInputs();
        
        Vector3 posToLook = new Vector3(transform.position.x,
            CharacterAnimManager.instance.transform.position.y, transform.position.z);
        CharacterAnimManager.instance.transform.LookAt(posToLook);
        CharacterMovementController.Instance.TriggerWalkTransition(posToLook - CharacterAnimManager.instance.transform.forward * distance);
        CharacterMovementController.Instance.FinishWalkToLocation.AddListener(PlayerPet);
        localCanvas.SetActive(false);
    }

    void PlayerPet()
    {
        CharacterAnimManager.instance.animator.SetTrigger(CharacterAnimManager.DoPet);
        particleSystem.Stop();
        particleSystem.Play();
        animator.SetTrigger(CharacterAnimManager.DoPet);
        CharacterMovementController.Instance.FinishWalkToLocation.RemoveListener(PlayerPet);
    }
}
