using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterInputManager : MonoBehaviour
{
    // Singleton
    public static CharacterInputManager Instance { get; private set; }

    private InputSystem_Actions _inputs;
    

    public UnityEvent OnCodexShow { get; set; } = new();
    public UnityEvent<bool> OnNavigationChange { get; set; } = new();
    public UnityEvent<bool> OnSelectRecipe { get; set; } = new();
    public UnityEvent<bool> OnInputsEnabled { get; set; } = new();
    
    [BoxGroup("Debug")]public bool showCodex;


    #region Unity Callbacks

    private void Awake()
    {
        Instance = this;
        _inputs = new InputSystem_Actions();
    }
    
    private void Start()
    {
        SetupInputs();
        EnableInputs();
    }

    private void Update()
    {
        if (showCodex)
        {
            AutoFlip.instance.PlayerInputFlipPages(_inputs.Player.Move.ReadValue<Vector2>());
        }
        else
        {
            CharacterMovementController.Instance.Move(_inputs.Player.Move.ReadValue<Vector2>());
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
        _inputs.Player.HapticChallengeSecond.performed += HapticChallengeSecondOnPerformed;
        _inputs.Player.Scythe.performed += ScytheOnPerformed;
        _inputs.Player.Unearth1.performed += Unearth1OnPerformed;
        _inputs.Player.Unearth2.performed += Unearth2OnPerformed;
        _inputs.Player.Unearth1.canceled += Unearth1OnCanceled;
        _inputs.Player.Unearth2.canceled += Unearth2OnCanceled;
        _inputs.Player.Harvest.performed += HarvestOnPerformed;
        _inputs.Player.Harvest.canceled += HarvestOnCanceled;
        _inputs.Player.ChoppingHapticChallenge1.performed += ChoppingHapticChallenge1OnPerformed;
        _inputs.Player.ChoppingHapticChallenge2.performed += ChoppingHapticChallenge2OnPerformed;
        _inputs.Player.ChoppingHapticChallenge3.performed += ChoppingHapticChallenge3OnPerformed;
        _inputs.Player.ChoppingHapticChallenge4.performed += ChoppingHapticChallenge4OnPerformed;
        _inputs.Player.ChoppingHapticChallenge5.performed += ChoppingHapticChallenge5OnPerformed;
        _inputs.Player.GrindingHapticChallenge1.performed += GrindingHapticChallenge1OnPerformed;
        _inputs.Player.GrindingHapticChallenge2.performed += GrindingHapticChallenge2OnPerformed;
        _inputs.Player.PushBellows.performed += PushBellowsOnPerformed;
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
        _inputs.Player.ToggleRun.performed += ToggleRunOnPerformed;
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
        if (OnInputsEnabled != null)
            OnInputsEnabled.Invoke(true);
    }

    public void EnableMoveInputs()
    {
        _inputs.Player.Move.Enable();
        _inputs.Player.ToggleRun.Enable();
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
        _inputs.Player.Scythe.Enable();
        _inputs.Player.Unearth1.Enable();
        _inputs.Player.Unearth2.Enable();
        _inputs.Player.Harvest.Enable();
        EnableChoppingHapticChallengeInputs();
        EnableGrindingHapticChallengeInputs();
        EnableTemperatureHapticChallengeInputs();
    }
    
    public void EnableChoppingHapticChallengeInputs()
    {
        _inputs.Player.ChoppingHapticChallenge1.Enable();
        _inputs.Player.ChoppingHapticChallenge2.Enable();
        _inputs.Player.ChoppingHapticChallenge3.Enable();
        _inputs.Player.ChoppingHapticChallenge4.Enable();
        _inputs.Player.ChoppingHapticChallenge5.Enable();
    }
    
    public void EnableGrindingHapticChallengeInputs()
    {
        _inputs.Player.GrindingHapticChallenge1.Enable();
        _inputs.Player.GrindingHapticChallenge2.Enable();
        EnableHapticChallengeJoystickInputs();
    }
    
    public void EnableTemperatureHapticChallengeInputs()
    {
        _inputs.Player.PushBellows.Enable();
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
        if (OnInputsEnabled != null)
            OnInputsEnabled.Invoke(false);
    }
    
    public void DisableMoveInputs()
    {
        _inputs.Player.Move.Disable();
        _inputs.Player.ToggleRun.Disable();
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
        _inputs.Player.Scythe.Disable();
        _inputs.Player.Unearth1.Disable();
        _inputs.Player.Unearth2.Disable();
        _inputs.Player.Harvest.Disable();
        DisableChoppingHapticChallengeInputs();
        DisableGrindingHapticChallengeInputs();
        DisableTemperatureHapticChallengeInputs();
    }
    
    public void DisableChoppingHapticChallengeInputs()
    {
        _inputs.Player.ChoppingHapticChallenge1.Disable();
        _inputs.Player.ChoppingHapticChallenge2.Disable();
        _inputs.Player.ChoppingHapticChallenge3.Disable();
        _inputs.Player.ChoppingHapticChallenge4.Disable();
        _inputs.Player.ChoppingHapticChallenge5.Disable();
    }
    
    public void DisableGrindingHapticChallengeInputs()
    {
        _inputs.Player.GrindingHapticChallenge1.Disable();
        _inputs.Player.GrindingHapticChallenge2.Disable();
        DisableHapticChallengeJoystickInputs();
    }
    
    public void DisableTemperatureHapticChallengeInputs()
    {
        _inputs.Player.PushBellows.Disable();
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
    private void ToggleRunOnPerformed(InputAction.CallbackContext obj)
    {
        CharacterMovementController.Instance.isRunning = true;
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        CharacterInteractController.Instance.Interact();
    }
    private void CancelOnPerformed(InputAction.CallbackContext obj)
    {
        CharacterInteractController.Instance.Cancel();
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
        CharacterInteractController.Instance.DropIngredientsInChoppingCountertop();
    }

    private void HapticChallengeSecondOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.StartStirChallenge();
        CharacterInteractController.Instance.DropIngredientsInGrindingCountertop();
    }
    
    private void ScytheOnPerformed(InputAction.CallbackContext obj)
    {
        CollectHapticChallengeManager.Instance.CheckScythingInput();
    }
    
    private void Unearth1OnPerformed(InputAction.CallbackContext obj)
    {
        CollectHapticChallengeManager.Instance.CheckUnearthingInputPressed(1);
    }
    
    private void Unearth2OnPerformed(InputAction.CallbackContext obj)
    {
        CollectHapticChallengeManager.Instance.CheckUnearthingInputPressed(2);
    }
    
    private void Unearth1OnCanceled(InputAction.CallbackContext obj)
    {
        CollectHapticChallengeManager.Instance.CheckUnearthingInputReleased(1);
    }
    
    private void Unearth2OnCanceled(InputAction.CallbackContext obj)
    {
        CollectHapticChallengeManager.Instance.CheckUnearthingInputReleased(2);
    }
    
    private void HarvestOnPerformed(InputAction.CallbackContext obj)
    {
        CollectHapticChallengeManager.Instance.CheckHarvestInputPressed();
    }
    
    private void HarvestOnCanceled(InputAction.CallbackContext obj)
    {
        CollectHapticChallengeManager.Instance.CheckHarvestInputReleased();
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
    
    private void ChoppingHapticChallenge4OnPerformed(InputAction.CallbackContext obj)
    {
        ChoppingHapticChallengeManager.Instance.NextChoppingTurn(4);
    }
    
    private void ChoppingHapticChallenge5OnPerformed(InputAction.CallbackContext obj)
    {
        ChoppingHapticChallengeManager.Instance.NextChoppingTurn(5);
    }
    
    private void GrindingHapticChallenge1OnPerformed(InputAction.CallbackContext obj)
    {
        GrindingHapticChallengeManager.Instance.CheckInputGrindingChallenge(1);
    }
    
    private void GrindingHapticChallenge2OnPerformed(InputAction.CallbackContext obj)
    {
        GrindingHapticChallengeManager.Instance.CheckInputGrindingChallenge(2);
    }
    
    private void PushBellowsOnPerformed(InputAction.CallbackContext obj)
    {
        TemperatureHapticChallengeManager.Instance.IncreaseTemperature();
    }
    
    private void HapticChallengeJoystickOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.JoystickInputValue = obj.ReadValue<Vector2>();
        GrindingHapticChallengeManager.Instance.JoystickInputValue = obj.ReadValue<Vector2>();
        CollectHapticChallengeManager.Instance.JoystickInputValue = obj.ReadValue<Vector2>();
    }
    
    private void HapticChallengeJoystickHorizontalAxisOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.JoystickInputValue = new Vector2(obj.ReadValue<float>(), StirHapticChallengeManager.Instance.JoystickInputValue.y);
        GrindingHapticChallengeManager.Instance.JoystickInputValue = new Vector2(obj.ReadValue<float>(), GrindingHapticChallengeManager.Instance.JoystickInputValue.y);
        CollectHapticChallengeManager.Instance.JoystickInputValue = new Vector2(obj.ReadValue<float>(), CollectHapticChallengeManager.Instance.JoystickInputValue.y);
    }
    
    private void HapticChallengeJoystickVerticalAxisOnPerformed(InputAction.CallbackContext obj)
    {
        StirHapticChallengeManager.Instance.JoystickInputValue = new Vector2(StirHapticChallengeManager.Instance.JoystickInputValue.x, obj.ReadValue<float>());
        GrindingHapticChallengeManager.Instance.JoystickInputValue = new Vector2(GrindingHapticChallengeManager.Instance.JoystickInputValue.x, obj.ReadValue<float>());
        CollectHapticChallengeManager.Instance.JoystickInputValue = new Vector2(CollectHapticChallengeManager.Instance.JoystickInputValue.x, obj.ReadValue<float>());
    }

    #endregion


    #region Codex Input Callbacks

    private void CodexOnPerformed(InputAction.CallbackContext obj)
    {
        CharacterMovementController.Instance.Move(Vector2.zero);
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
        
        AutoFlip.instance.PlayerInputNavigateBookmarks(true);
    }
    
    private void BookMarkRightOnPerformed(InputAction.CallbackContext obj)
    {
        if (!showCodex)return;
        
        AutoFlip.instance.PlayerInputNavigateBookmarks(false);
    }

    private void PinRightOnPerformed(InputAction.CallbackContext obj)
    {
        OnSelectRecipe?.Invoke(true);
    }

    private void PinLeftOnPerformed(InputAction.CallbackContext obj)
    {
        OnSelectRecipe?.Invoke(false);
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
        if (!MailBoxBehaviour.instance) 
            return;
        MailBoxBehaviour.instance.PassToNextLetter();
    }

    #endregion
}
