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
    public UnityEvent<bool> OnSelectRecipe { get; set; } = new();
    
    [BoxGroup("Debug")]public bool showCodex;


    #region Unity Callbacks

    private void Awake()
    {
        Instance = this;
        _inputs = new InputSystem_Actions();
    }
    
    private void Start()
    {
        movementController = GetComponent<CharacterMovementController>();
        characterInteractController = GetComponent<CharacterInteractController>();
        codexController = AutoFlip.instance;
        
        SetupInputs();
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

    private void OnDestroy()
    {
        DisableInputs();
    }

    #endregion


    #region Input Setup

    public void SetupInputs()
    {
        _inputs.Player.Move.performed += MoveOnPerformed;
        _inputs.Player.Interact.performed += InteractOnPerformed;
        _inputs.Player.Cancel.performed += CancelOnPerformed;
        _inputs.Player.PreviousBasketSet.performed += PreviousBasketSetOnPerformed;
        _inputs.Player.NextBasketSet.performed += NextBasketSetOnPerformed;
        _inputs.Player.HapticChallenge.performed += HapticChallengeOnPerformed;
        _inputs.Player.HapticChallenge.canceled += HapticChallengeOnCanceled;
        _inputs.Player.HapticChallengeSecond.performed += HapticChallengeSecondOnPerformed;
        _inputs.Player.ChoppingHapticChallenge1.performed += ChoppingHapticChallenge1OnPerformed;
        _inputs.Player.ChoppingHapticChallenge2.performed += ChoppingHapticChallenge2OnPerformed;
        _inputs.Player.ChoppingHapticChallenge3.performed += ChoppingHapticChallenge3OnPerformed;
        _inputs.Player.HapticChallengeJoystick.performed += HapticChallengeJoystickOnPerformed;
        _inputs.Player.HapticChallengeJoystick.canceled += HapticChallengeJoystickOnPerformed;
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.performed += HapticChallengeJoystickHorizontalAxisOnPerformed;
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.canceled += HapticChallengeJoystickHorizontalAxisOnPerformed;
        _inputs.Player.HapticChallengeJoystickVerticalAxis.performed += HapticChallengeJoystickVerticalAxisOnPerformed;
        _inputs.Player.HapticChallengeJoystickVerticalAxis.canceled += HapticChallengeJoystickVerticalAxisOnPerformed;
        _inputs.Player.Codex.performed += CodexOnPerformed;
        _inputs.Player.BookMarkLeft.performed += BookMarkLeftOnPerformed;
        _inputs.Player.BookMarkRight.performed += BookMarkRightOnPerformed;
        _inputs.Player.StartPageNavigation.performed += StartPageNavigationOnPerformed;
        _inputs.Player.ExitPageNavigation.performed += ExitPageNavigationOnPerformed;
        _inputs.Player.PinLeft.performed += PinLeftOnPerformed;
        _inputs.Player.PinRight.performed += PinRightOnPerformed;
        _inputs.Player.PassLetters.performed += PassLettersOnPerformed;
    }

    public void DestroyInputs()
    {
    }

    #endregion


    #region Enable Inputs

    public void EnableInputs()
    {
        _inputs.Player.Enable();
        EnableMoveInputs();
        EnableInteractInputs();
        EnableHapticChallengeInputs();
        EnableCodexInputs();
    }

    public void EnableMoveInputs()
    {
        _inputs.Player.Move.Enable();
    }
    
    public void EnableInteractInputs()
    {
        _inputs.Player.Interact.Enable();
        _inputs.Player.Cancel.Enable();
        _inputs.Player.PreviousBasketSet.Enable();
        _inputs.Player.NextBasketSet.Enable();
    }

    public void EnableHapticChallengeInputs()
    {
        _inputs.Player.HapticChallenge.Enable();
        _inputs.Player.HapticChallengeSecond.Enable();
        EnableChoppingHapticChallengeInputs();
        EnableHapticChallengeJoystickInputs();
    }
    
    public void EnableChoppingHapticChallengeInputs()
    {
        _inputs.Player.ChoppingHapticChallenge1.Enable();
        _inputs.Player.ChoppingHapticChallenge2.Enable();
        _inputs.Player.ChoppingHapticChallenge3.Enable();
    }
    
    public void EnableHapticChallengeJoystickInputs()
    {
        _inputs.Player.HapticChallengeJoystick.Enable();
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.Enable();
        _inputs.Player.HapticChallengeJoystickVerticalAxis.Enable();
    }

    private void EnableCodexInputs()
    {
        _inputs.Player.Codex.Enable();
        _inputs.Player.BookMarkLeft.Enable();
        _inputs.Player.BookMarkRight.Enable();
        _inputs.Player.StartPageNavigation.Enable();
        _inputs.Player.ExitPageNavigation.Enable();
        _inputs.Player.PinLeft.Enable();
        _inputs.Player.PinRight.Enable();
    }

    public void EnableMailInputs()
    {
        _inputs.Player.PassLetters.Enable();
    }

    #endregion

    
    #region Disable Inputs

    public void DisableInputs()
    {
        DisableMoveInputs();
        DisableInteractInputs();
        DisableHapticChallengeInputs();
        DisableCodexInputs();
        DisableMailInputs();
        _inputs.Player.Disable();
    }
    
    public void DisableMoveInputs()
    {
        _inputs.Player.Move.Disable();
    }
    
    public void DisableInteractInputs()
    {
        _inputs.Player.Interact.Disable();
        _inputs.Player.Cancel.Disable();
        _inputs.Player.PreviousBasketSet.Disable();
        _inputs.Player.NextBasketSet.Disable();
    }
    
    public void DisableHapticChallengeInputs()
    {
        _inputs.Player.HapticChallenge.Disable();
        _inputs.Player.HapticChallengeSecond.Disable();
        DisableChoppingHapticChallengeInputs();
        DisableHapticChallengeJoystickInputs();
    }
    
    public void DisableChoppingHapticChallengeInputs()
    {
        _inputs.Player.ChoppingHapticChallenge1.Disable();
        _inputs.Player.ChoppingHapticChallenge2.Disable();
        _inputs.Player.ChoppingHapticChallenge3.Disable();
    }
    
    public void DisableHapticChallengeJoystickInputs()
    {
        _inputs.Player.HapticChallengeJoystick.Disable();
        _inputs.Player.HapticChallengeJoystickHorizontalAxis.Disable();
        _inputs.Player.HapticChallengeJoystickVerticalAxis.Disable();
    }
    
    private void DisableCodexInputs()
    {
        _inputs.Player.Codex.Disable();
        _inputs.Player.BookMarkLeft.Disable();
        _inputs.Player.BookMarkRight.Disable();
        _inputs.Player.StartPageNavigation.Disable();
        _inputs.Player.ExitPageNavigation.Disable();
        _inputs.Player.PinLeft.Disable();
        _inputs.Player.PinRight.Disable();
    }

    public void DisableMailInputs()
    {
        _inputs.Player.PassLetters.Disable();
    }

    #endregion


    #region Main Input Callbacks

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
    
    private void PreviousBasketSetOnPerformed(InputAction.CallbackContext obj)
    {
        if (!BasketInputManager.Instance) return;
        
        BasketInputManager.Instance.PreviousBasketSet();
    }
    
    private void NextBasketSetOnPerformed(InputAction.CallbackContext obj)
    {
        if (!BasketInputManager.Instance) return;
        
        BasketInputManager.Instance.NextBasketSet();
    }

    #endregion


    #region Haptic Challenge Input Callbacks

    private void HapticChallengeOnPerformed(InputAction.CallbackContext obj)
    {
        TemperatureHapticChallengeManager.Instance.StartTemperatureChallenge();
        CollectHapticChallengeManager.Instance.ActivateHarvestHapticChallenge();
        characterInteractController.DropIngredientsInChoppingCountertop();
    }
    
    private void HapticChallengeOnCanceled(InputAction.CallbackContext obj)
    {
        CollectHapticChallengeManager.Instance.StopTurnHarvestChallenge();
    }

    private void HapticChallengeSecondOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.StartStirChallenge();
    }
    
    private void ChoppingHapticChallenge1OnPerformed(InputAction.CallbackContext obj)
    {
        ChoppingHapticChallengeManager.Instance.NextChoppingTurn(1);
    }
    
    private void ChoppingHapticChallenge2OnPerformed(InputAction.CallbackContext obj)
    {
        ChoppingHapticChallengeManager.Instance.NextChoppingTurn(2);
    }
    
    private void ChoppingHapticChallenge3OnPerformed(InputAction.CallbackContext obj)
    {
        ChoppingHapticChallengeManager.Instance.NextChoppingTurn(3);
    }
    
    private void HapticChallengeJoystickOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.JoystickInputValue = obj.ReadValue<Vector2>();
        TemperatureHapticChallengeManager.Instance.JoystickInputValue = obj.ReadValue<Vector2>();
        CollectHapticChallengeManager.Instance.JoystickInputValue = obj.ReadValue<Vector2>();
    }
    
    private void HapticChallengeJoystickHorizontalAxisOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.JoystickInputValue = new Vector2(obj.ReadValue<float>(), StirHapticChallengeManager.Instance.JoystickInputValue.y);
        TemperatureHapticChallengeManager.Instance.JoystickInputValue = new Vector2(obj.ReadValue<float>(), TemperatureHapticChallengeManager.Instance.JoystickInputValue.y);
        CollectHapticChallengeManager.Instance.JoystickInputValue = new Vector2(obj.ReadValue<float>(), CollectHapticChallengeManager.Instance.JoystickInputValue.y);
    }
    
    private void HapticChallengeJoystickVerticalAxisOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.JoystickInputValue = new Vector2(StirHapticChallengeManager.Instance.JoystickInputValue.x, obj.ReadValue<float>());
        TemperatureHapticChallengeManager.Instance.JoystickInputValue = new Vector2(TemperatureHapticChallengeManager.Instance.JoystickInputValue.x, obj.ReadValue<float>());
        CollectHapticChallengeManager.Instance.JoystickInputValue = new Vector2(CollectHapticChallengeManager.Instance.JoystickInputValue.x, obj.ReadValue<float>());
    }

    #endregion


    #region Codex Input Callbacks

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
    
    private void BookMarkLeftOnPerformed(InputAction.CallbackContext obj)
    {
        if (!showCodex)return;
        
        codexController.PlayerInputNavigateBookmarks(true);
    }
    
    private void BookMarkRightOnPerformed(InputAction.CallbackContext obj)
    {
        if (!showCodex)return;
        
        codexController.PlayerInputNavigateBookmarks(false);
    }

    private void PinRightOnPerformed(InputAction.CallbackContext obj)
    {
        if (OnSelectRecipe != null)
            OnSelectRecipe.Invoke(true);
    }

    private void PinLeftOnPerformed(InputAction.CallbackContext obj)
    {
        if (OnSelectRecipe != null)
            OnSelectRecipe.Invoke(false);
    }
    private void StartPageNavigationOnPerformed(InputAction.CallbackContext obj)
    {
        if (!showCodex)return;
        if (OnCodexShow != null)
            OnNavigationChange.Invoke(true);
    }
    
    private void ExitPageNavigationOnPerformed(InputAction.CallbackContext obj)
    {
        if (!showCodex)return;
        if (OnNavigationChange != null)
            OnNavigationChange.Invoke(false);
    }
    
    private void PassLettersOnPerformed(InputAction.CallbackContext obj)
    {
        if (!MailBox.instance) 
            return;
        MailBox.instance.PassToNextLetter();
    }

    #endregion
}
