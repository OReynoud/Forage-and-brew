using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputManager : MonoBehaviour
{
    // Singleton
    public static CharacterInputManager Instance { get; private set; }

    private InputSystem_Actions _inputs;

    [SerializeField] private CharacterMovementController movementController;
    [SerializeField] private CharacterInteractController characterInteractController;


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

    #endregion

    
    public void EnableInputs()
    {
        _inputs.Player.Enable();
        EnableMoveInputs();
        EnableInteractInputs();
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
    
    public void DisableInputs()
    {
        _inputs.Player.Disable();
        DisableMoveInputs();
        DisableInteractInputs();
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

    
    private void MoveOnPerformed(InputAction.CallbackContext obj)
    {
        // Debug.Log(obj.ReadValue<Vector2>());
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        characterInteractController.Interact();
    }

    private void Update()
    {
        movementController.Move(_inputs.Player.Move.ReadValue<Vector2>());
    }
}
