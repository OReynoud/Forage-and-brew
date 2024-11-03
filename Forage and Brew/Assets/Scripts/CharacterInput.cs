using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{
    public InputSystem_Actions inputs;

    public CharacterMovement movement;
    [SerializeField] private CharacterInteractController characterInteractController;

    private void Start()
    {
        inputs = new InputSystem_Actions();
        EnableInputs(true);
    }

    public void EnableInputs(bool enable)
    {
        if (enable)
        {
            inputs.Player.Enable();
            inputs.Player.Move.Enable();
            inputs.Player.Interact.Enable();
            inputs.Player.Move.performed += MoveOnPerformed;
            inputs.Player.Interact.performed += InteractOnPerformed;
        }
        else
        {
            inputs.Player.Disable();
            inputs.Player.Move.Disable();
            inputs.Player.Interact.Disable();
            inputs.Player.Move.performed -= MoveOnPerformed;
            inputs.Player.Interact.performed -= InteractOnPerformed;
        }
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
        movement.Move(inputs.Player.Move.ReadValue<Vector2>());
    }
}
