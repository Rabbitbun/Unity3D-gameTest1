using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : ScriptableObject, PlayerInputs.IPlayerActions, PlayerInputs.IMenuActions
{
    public PlayerInputs playerInput;

    // GamePlay
    public event UnityAction<Vector2> moveEvent = delegate { };
    public event UnityAction<Vector2, bool> cameraMoveEvent = delegate { };

    public event UnityAction attackEvent = delegate { };

    public event UnityAction<int> useAbility1Event = delegate { };
    public event UnityAction<int> useAbility2Event = delegate { };
    public event UnityAction<int> useAbility3Event = delegate { };
    public event UnityAction<int> useAbility4Event = delegate { };

    public event UnityAction switchStyleEvent = delegate { };
    public event UnityAction switchAbilityListEvent = delegate { };

    public event UnityAction startedRunning = delegate { };
    public event UnityAction stoppedRunning = delegate { };

    public event UnityAction crouchEvent = delegate { };

    public event UnityAction jumpEvent = delegate { };
    public event UnityAction jumpCanceledEvent = delegate { };

    public event UnityAction startedChanting = delegate { };
    public event UnityAction stoppedChanting = delegate { };

    public event UnityAction startedAiming = delegate { };
    public event UnityAction stoppedAiming = delegate { };

    public event UnityAction pauseEvent = delegate { };
    public event UnityAction interactEvent = delegate { };
    public event UnityAction lockCameraEvent = delegate { };

    public event UnityAction MouseLeftClickEvent = delegate { };
    public event UnityAction MouseRightClickEvent = delegate { };

    // Menu
    public event UnityAction moveSelectionEvent = delegate { };
    public event UnityAction menuMouseMoveEvent = delegate { };
    public event UnityAction menuLeftClickEvent = delegate { };
    public event UnityAction menuRightClickEvent = delegate { };
    public event UnityAction menuUnpauseEvent = delegate { };
    public event UnityAction menuBackViewEvent = delegate { };

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInputs();
            playerInput.Player.SetCallbacks(this);
            playerInput.Menu.SetCallbacks(this);
            //...
        }

        EnableGameplayInput();
    }
    private void OnDisable()
    {
        DisableAllInput();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            attackEvent.Invoke();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            startedAiming.Invoke();

        if (context.phase == InputActionPhase.Canceled)
            stoppedAiming.Invoke();
    }

    public void OnChant(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            startedChanting.Invoke();

        if (context.phase == InputActionPhase.Canceled)
            stoppedChanting.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            crouchEvent.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            interactEvent.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            jumpEvent.Invoke();

        if (context.phase == InputActionPhase.Canceled)
            jumpCanceledEvent.Invoke();
    }

    public void OnLockCamera(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            MouseRightClickEvent.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cameraMoveEvent.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
    }

    private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

    public void OnMove(InputAction.CallbackContext context)
    {
        moveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            pauseEvent.Invoke();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                startedRunning.Invoke();
                break;
            case InputActionPhase.Canceled:
                stoppedRunning.Invoke();
                break;
        }
    }

    public void OnSwitchStyle(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            switchStyleEvent.Invoke();
    }

    public void OnUseAbility1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            useAbility1Event.Invoke(0);
    }
    public void OnUseAbility2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            useAbility2Event.Invoke(1);
    }
    public void OnUseAbility3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            useAbility3Event.Invoke(2);
    }
    public void OnUseAbility4(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            useAbility4Event.Invoke(3);
    }

    public void OnSwitchAbilityList(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            switchAbilityListEvent.Invoke();
    }

    // menu
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            MouseLeftClickEvent.Invoke();
    }
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            MouseRightClickEvent.Invoke();
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            menuMouseMoveEvent();
    }

    public void OnMoveSelection(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            moveSelectionEvent();
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            menuUnpauseEvent();
    }

    public void OnBackView(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            menuBackViewEvent();
    }

    // other 
    public void EnableGameplayInput()
    {
        playerInput.Menu.Disable();
        playerInput.Player.Enable();
    }

    public void EnableMenuInput()
    {
        playerInput.Menu.Enable();
        playerInput.Player.Disable();
    }

    public void DisableAllInput()
    {
        playerInput.Menu.Disable();
        playerInput.Player.Disable();
    }
}
