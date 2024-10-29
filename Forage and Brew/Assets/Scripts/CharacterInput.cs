using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{
    public InputSystem_Actions inputs;

    public CharacterMovement movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
            inputs.Player.Move.performed += MoveOnPerformed;
        }
        else
        {
            inputs.Player.Disable();
            inputs.Player.Move.Disable();
        }
        
    }

    private void MoveOnPerformed(InputAction.CallbackContext obj)
    {
        Debug.Log(obj.ReadValue<Vector2>());
    }

    // Update is called once per frame
    void Update()
    {
        movement.Move(inputs.Player.Move.ReadValue<Vector2>());
    }
}
