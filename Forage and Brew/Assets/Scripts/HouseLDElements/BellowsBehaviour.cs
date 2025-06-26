using UnityEngine;

public class BellowsBehaviour : PurchasableHouseItemBehaviour
{
    [SerializeField] private Animator bellowsAnimator;
    
    [SerializeField] private float pushSpeedOutsideTemperatureChallenge = 0.5f;
    [SerializeField] private float pushSpeedInsideTemperatureChallenge = 2f;
    
    [SerializeField] private GameObject interactInputCanvasGameObject;
    
    // Animator Hashes
    private static readonly int DoPushBellows = Animator.StringToHash("DoPushBellows");
    private static readonly int IsInTemperatureChallenge = Animator.StringToHash("IsInTemperatureChallenge");
    private static readonly int PushSpeed = Animator.StringToHash("PushSpeed");


    protected override void Start()
    {
        base.Start();
        
        DisableInteract();
        
        bellowsAnimator.SetFloat(PushSpeed, pushSpeedOutsideTemperatureChallenge);

        if (Unlocked)
        {
            bellowsAnimator.SetBool(IsInTemperatureChallenge, false);
        }
    }


    public void EnableInteract()
    {
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract()
    {
        interactInputCanvasGameObject.SetActive(false);
    }


    public override void PurchaseItem()
    {
        base.PurchaseItem();
        
        bellowsAnimator.SetBool(IsInTemperatureChallenge, false);
    }


    public void PlayBellowsAnimation()
    {
        bellowsAnimator.SetTrigger(DoPushBellows);
    }
    
    public void EnterTemperatureHapticChallenge()
    {
        bellowsAnimator.SetBool(IsInTemperatureChallenge, true);
        bellowsAnimator.SetFloat(PushSpeed, pushSpeedInsideTemperatureChallenge);
    }
    
    public void ExitTemperatureHapticChallenge()
    {
        bellowsAnimator.SetBool(IsInTemperatureChallenge, false);
        bellowsAnimator.SetFloat(PushSpeed, pushSpeedOutsideTemperatureChallenge);
    }
    

    protected override void ManageCharacterNear(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.collectedStack.Count == 0)
        {
            characterInteractController.CurrentNearBellows = this;
            
            LastTriggeredCollider = other;
            
            if (CanPurchase)
            {
                ShowPrice();
            }
            else if (Unlocked)
            {
                if (other.TryGetComponent(out TemperatureHapticChallengeManager temperatureHapticChallengeManager))
                {
                    temperatureHapticChallengeManager.CurrentBellows = this;

                    EnableInteract();
                }
            }
        }
    }

    protected override void ManageCharacterFar(Collider other)
    {
        if (LastTriggeredCollider == other)
        {
            LastTriggeredCollider = null;
        }
        
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out TemperatureHapticChallengeManager temperatureHapticChallengeManager) &&
            characterInteractController.CurrentNearBellows == this)
        {
            characterInteractController.CurrentNearBellows = null;
            temperatureHapticChallengeManager.CurrentBellows = null;
        }
            
        DisableInteract();
        HidePrice();
    }
}
