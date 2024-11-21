using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterInputManager : MonoBehaviour
{
    // Singleton
    public static CharacterInputManager Instance { get; private set; }

    private InputSystem_Actions _inputs;

    private CharacterMovementController movementController;
    private CharacterInteractController characterInteractController;
    private AutoFlip codexController;

    public UnityEvent OnCodexShow { get; set; } = new();
    public UnityEvent<bool> OnNavigationChange { get; set; } = new();
    
    

    [BoxGroup("Debug")]public bool showCodex;


    #region Unity Callbacks

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        _inputs = new InputSystem_Actions();
        EnableInputs();
        movementController = GetComponent<CharacterMovementController>();
        characterInteractController = GetComponent<CharacterInteractController>();
        codexController = AutoFlip.instance;
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
        _inputs.Player.StartPageNavigation.performed += StartPageNavigationOnPerformed;
        _inputs.Player.ExitPageNavigation.performed += ExitPageNavigationOnPerformed;
    }



    private void DisableCodexInputs()
    {
        _inputs.Player.Codex.Disable();
        _inputs.Player.Codex.performed -= CodexOnPerformed;
        _inputs.Player.BookMarkLeft.performed -= BookMarkLeftOnPerformed;
        _inputs.Player.BookMarkRight.performed -= BookMarkRightOnPerformed;
        _inputs.Player.StartPageNavigation.performed -= StartPageNavigationOnPerformed;
        _inputs.Player.ExitPageNavigation.performed -= ExitPageNavigationOnPerformed;
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

        if (!showCodex)
        {
            OnNavigationChange.Invoke(false);
        }
        
        if (OnCodexShow != null)
            OnCodexShow.Invoke();
    }
    private void ExitPageNavigationOnPerformed(InputAction.CallbackContext obj)
    {
        if (!showCodex)return;
        if (OnNavigationChange != null)
            OnNavigationChange.Invoke(false);
    }

    private void StartPageNavigationOnPerformed(InputAction.CallbackContext obj)
    {
        if (!showCodex)return;
        if (OnCodexShow != null)
            OnNavigationChange.Invoke(true);
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
        _inputs.Player.Cancel.Enable();
        _inputs.Player.Cancel.performed += CancelOnPerformed;
    }



    public void EnableHapticChallengeInputs()
    {
        _inputs.Player.HapticChallenge.Enable();
        _inputs.Player.HapticChallenge.performed += HapticChallengeOnPerformed;
        _inputs.Player.HapticChallengeSecond.Enable();
        _inputs.Player.HapticChallengeSecond.performed += HapticChallengeSecondOnPerformed;
        EnableHapticChallengeJoystickInputs();
    }
    
    public void EnableHapticChallengeJoystickInputs()
    {
        _inputs.Player.HapticChallengeJoystick.Enable();
        _inputs.Player.HapticChallengeJoystick.performed += HapticChallengeJoystickOnPerformed;
        _inputs.Player.HapticChallengeJoystick.canceled += HapticChallengeJoystickOnPerformed;
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.Enable();
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.performed += HapticChallengeJoystickHorizontalAxisOnPerformed;
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.canceled += HapticChallengeJoystickHorizontalAxisOnPerformed;
        _inputs.Player.HapticChallengeJoystickVerticalAxis.Enable();
        _inputs.Player.HapticChallengeJoystickVerticalAxis.performed += HapticChallengeJoystickVerticalAxisOnPerformed;
        _inputs.Player.HapticChallengeJoystickVerticalAxis.canceled += HapticChallengeJoystickVerticalAxisOnPerformed;
    }

    public void DisableInputs()
    {
        DisableMoveInputs();
        DisableInteractInputs();
        DisableHapticChallengeInputs();
        DisableCodexInputs();
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
        _inputs.Player.Cancel.Disable();
        _inputs.Player.Cancel.performed -= CancelOnPerformed;
    }
    
    public void DisableHapticChallengeInputs()
    {
        _inputs.Player.HapticChallenge.Disable();
        _inputs.Player.HapticChallenge.performed -= HapticChallengeOnPerformed;
        _inputs.Player.HapticChallengeSecond.Disable();
        _inputs.Player.HapticChallengeSecond.performed -= HapticChallengeSecondOnPerformed;
        DisableHapticChallengeJoystickInputs();
    }
    
    public void DisableHapticChallengeJoystickInputs()
    {
        _inputs.Player.HapticChallengeJoystick.Disable();
        _inputs.Player.HapticChallengeJoystick.performed -= HapticChallengeJoystickOnPerformed;
        _inputs.Player.HapticChallengeJoystick.canceled -= HapticChallengeJoystickOnPerformed;
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.Disable();
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.performed -= HapticChallengeJoystickHorizontalAxisOnPerformed;
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.canceled -= HapticChallengeJoystickHorizontalAxisOnPerformed;
        _inputs.Player.HapticChallengeJoystickVerticalAxis.Disable();
        _inputs.Player.HapticChallengeJoystickVerticalAxis.performed -= HapticChallengeJoystickVerticalAxisOnPerformed;
        _inputs.Player.HapticChallengeJoystickVerticalAxis.canceled -= HapticChallengeJoystickVerticalAxisOnPerformed;
    }

    
    private void MoveOnPerformed(InputAction.CallbackContext obj)
    {
        // Debug.Log(obj.ReadValue<Vector2>());
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        characterInteractController.Interact();
    }
    private void CancelOnPerformed(InputAction.CallbackContext obj)
    {
        characterInteractController.Cancel();
    }

    private void HapticChallengeOnPerformed(InputAction.CallbackContext obj)
    {
        CollectHapticChallengeManager.Instance.StopCollectHapticChallenge();
        TemperatureHapticChallengeManager.Instance.StartTemperatureChallenge();
    }

    private void HapticChallengeSecondOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.StartStirChallenge();
    }
    
    private void HapticChallengeJoystickOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.JoystickInputValue = obj.ReadValue<Vector2>();
        TemperatureHapticChallengeManager.Instance.JoystickInputValue = obj.ReadValue<Vector2>();
    }
    
    private void HapticChallengeJoystickHorizontalAxisOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.JoystickInputValue = new Vector2(obj.ReadValue<float>(), StirHapticChallengeManager.Instance.JoystickInputValue.y);
        TemperatureHapticChallengeManager.Instance.JoystickInputValue = new Vector2(obj.ReadValue<float>(), TemperatureHapticChallengeManager.Instance.JoystickInputValue.y);
    }
    
    private void HapticChallengeJoystickVerticalAxisOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.JoystickInputValue = new Vector2(StirHapticChallengeManager.Instance.JoystickInputValue.x, obj.ReadValue<float>());
        TemperatureHapticChallengeManager.Instance.JoystickInputValue = new Vector2(TemperatureHapticChallengeManager.Instance.JoystickInputValue.x, obj.ReadValue<float>());
    }
}
