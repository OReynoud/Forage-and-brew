using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterInputManager : MonoBehaviour
{
    // Singleton
    public static CharacterInputManager Instance { get; private set; }

    private InputSystem_Actions _inputs;

    [SerializeField] private CharacterMovementController movementController;
    [SerializeField] private CharacterInteractController characterInteractController;
    [SerializeField] private AutoFlip codexController;
    public UnityEvent OnCodexShow;
    public bool showCodex;


    #region Unity Callbacks

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        _inputs = new InputSystem_Actions();
        EnableInputs();
    }

    private void Update()
    {
        if (showCodex)
        {
            codexController.PlayerInputFlipPages(_inputs.Player.Move.ReadValue<Vector2>());
        }
        else
        {
            movementController.Move(_inputs.Player.Move.ReadValue<Vector2>());
        }
        
    }

    #endregion

    
    public void EnableInputs()
    {
        _inputs.Player.Enable();
        EnableMoveInputs();
        EnableInteractInputs();
        EnableHapticChallengeInputs();
        EnableCodexInputs();
    }

    private void EnableCodexInputs()
    {
        _inputs.Player.Codex.Enable();
        _inputs.Player.Codex.performed += CodexOnPerformed;
        _inputs.Player.BookMarkLeft.performed += BookMarkLeftOnPerformed;
        _inputs.Player.BookMarkRight.performed += BookMarkRightOnPerformed;
    }
    private void DisableCodexInputs()
    {
        _inputs.Player.Codex.Disable();
        _inputs.Player.Codex.performed -= CodexOnPerformed;
        _inputs.Player.BookMarkLeft.performed -= BookMarkLeftOnPerformed;
        _inputs.Player.BookMarkRight.performed -= BookMarkRightOnPerformed;
    }

    private void BookMarkRightOnPerformed(InputAction.CallbackContext obj)
    {
        if (!showCodex)return;
        
        codexController.PlayerInputNavigateBookmarks(false);
    }
    private void BookMarkLeftOnPerformed(InputAction.CallbackContext obj)
    {
        if (!showCodex)return;
        
        codexController.PlayerInputNavigateBookmarks(true);
    }

    private void CodexOnPerformed(InputAction.CallbackContext obj)
    {
        movementController.Move(Vector2.zero);
        showCodex = !showCodex;
        
        if (OnCodexShow != null)
            OnCodexShow.Invoke();
        
    }

    public void EnableMoveInputs()
    {
        _inputs.Player.Move.Enable();
        _inputs.Player.Move.performed += MoveOnPerformed;
    }
    
    public void EnableInteractInputs()
    {
        _inputs.Player.Interact.Enable();
        _inputs.Player.Interact.performed += InteractOnPerformed;
    }
    
    public void EnableHapticChallengeInputs()
    {
        _inputs.Player.HapticChallenge.Enable();
        _inputs.Player.HapticChallenge.performed += HapticChallengeOnPerformed;
    }

    public void DisableInputs()
    {
        _inputs.Player.Disable();
        DisableMoveInputs();
        DisableInteractInputs();
        DisableHapticChallengeInputs();
    }
    
    public void DisableMoveInputs()
    {
        _inputs.Player.Move.Disable();
        _inputs.Player.Move.performed -= MoveOnPerformed;
    }
    
    public void DisableInteractInputs()
    {
        _inputs.Player.Interact.Disable();
        _inputs.Player.Interact.performed -= InteractOnPerformed;
    }
    
    public void DisableHapticChallengeInputs()
    {
        _inputs.Player.HapticChallenge.Disable();
        _inputs.Player.HapticChallenge.performed -= HapticChallengeOnPerformed;
    }

    
    private void MoveOnPerformed(InputAction.CallbackContext obj)
    {
        // Debug.Log(obj.ReadValue<Vector2>());
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        characterInteractController.Interact();
    }

    private void HapticChallengeOnPerformed(InputAction.CallbackContext obj)
    {
        HapticChallengeManager.Instance.StopHapticChallenge();
    }
}
