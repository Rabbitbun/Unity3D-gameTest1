using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;

    public Vector2 move { get; private set; }
    public Vector2 look { get; private set; }
    public bool jump { get; private set; }
    public bool run { get; private set; }
    public bool crouch { get; private set; }
    public bool rightClick { get; private set; }
    public bool leftClick { get; private set; }
    public bool tabClick { get; private set; }
    public bool Casting { get; set; }
    public bool Aiming { get; set; }
    public bool RightClickPressed { get; private set; }

    public bool EscPressed { get; private set; }

    public PlayerInputs PlayerInput;

    private void OnEnable()
    {
        PlayerInput.Player.Enable();
    }
    private void OnDisable()
    {
        PlayerInput.Player.Disable();
    }

    public void Awake()
    {
        PlayerInput = MasterManager.Instance.playerInputActions;

        Instance = this;
        //PlayerInput.Player.Enable();
    }

    public void Start()
    {
        PlayerInput.Player.Look.started += OnLookInput;
        PlayerInput.Player.Look.performed += OnLookInput;
        PlayerInput.Player.Look.canceled += OnLookInput;

        //PlayerInput.Player.LeftClick.started += OnLeftClickInput;
        ////_playerInput.Player.LeftClick.performed += OnLeftClickInput;
        //PlayerInput.Player.LeftClick.canceled += OnLeftClickInput;

        //PlayerInput.Player.RightClick.started += OnRightClickInput;
        ////_playerInput.Player.RightClick.performed += OnRightClickInput;
        ////_playerInput.Player.RightClick.canceled += OnRightClickInput;

        //PlayerInput.Player.RightClick.started += OnRightClickPressed;
        ////PlayerInput.Player.RightClick.performed += OnRightClickPressed;
        //PlayerInput.Player.RightClick.canceled += OnRightClickPressed;

        PlayerInput.Player.Move.performed += OnMoveInput;
        PlayerInput.Player.Move.canceled += OnMoveInput;

        PlayerInput.Player.Jump.performed += OnJumpInput;
        PlayerInput.Player.Jump.canceled += OnJumpInput;

        PlayerInput.Player.Run.performed += OnRunInput;
        PlayerInput.Player.Run.canceled += OnRunInput;

        PlayerInput.Player.Crouch.performed += OnCrouchInput;
        PlayerInput.Player.Crouch.canceled += OnCrouchInput;

        PlayerInput.Player.Pause.started += OnEscInput;

        //PlayerInput.Player.CastSpell.started += OnCasting;
        ////PlayerInput.Player.CastSpell.performed += OnCasting;
        ////PlayerInput.Player.CastSpell.canceled += OnCasting;

        PlayerInput.Player.Aim.started += OnAimming;
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    public void OnRunInput(InputAction.CallbackContext context)
    {
        run = context.ReadValueAsButton();
    }
    public void OnCrouchInput(InputAction.CallbackContext context)
    {
        crouch = context.ReadValueAsButton();
    }
    public void OnRightClickInput(InputAction.CallbackContext context)
    {
        rightClick = !rightClick;
    }

    public void OnLeftClickInput(InputAction.CallbackContext context)
    {
        //if (context.phase == InputActionPhase.Performed)
        //{
        //    leftClick = true;
        //}
        //else
        //{
        //    leftClick = false;
        //}
        leftClick = context.ReadValueAsButton();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        jump = context.ReadValueAsButton();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnEscInput(InputAction.CallbackContext context)
    {
        //EscPressed = !EscPressed;
        MasterManager.Instance.GameEventManager.PauseGame();
    }

    public void OnCasting(InputAction.CallbackContext context)
    {
        //Casting = context.ReadValueAsButton();
        if (Aiming)
        {
            Casting = false;
        }
        else
        {
            Casting = !Casting;
        }
        Debug.Log(Casting);
    }

    public void OnAimming(InputAction.CallbackContext context)
    {
        if (Casting)
        {
            Casting = false;
        }
        Aiming = !Aiming;
    }

    public void OnRightClickPressed(InputAction.CallbackContext context)
    {
        RightClickPressed = context.ReadValueAsButton();
    }
}
