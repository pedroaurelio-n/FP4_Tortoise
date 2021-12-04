using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public delegate void ControlSchemeChanged(PlayerInput input);
    public static event ControlSchemeChanged onControlSchemeChange;

    [SerializeField] private PlayerMain playerMain;

    private PlayerControls playerControls;
    private PlayerInput playerInput;

    private Vector2 _inputMovementVector;
    public float MoveAmount;
    public float HorizontalInput;
    public float VerticalInput;

    public bool jumpInput;
    public bool sprintInput;
    public bool glideInput;
    public bool anyglideInput;
    public bool attackInput;
    public bool interactInput;

    public bool canTriggerJumpInput = true;

    private string _currentControlScheme;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if (onControlSchemeChange != null)
        {
            onControlSchemeChange(playerInput);
            Debug.Log("Send Control Start Event");
        }
    }

    public void CheckForControlSchemeChange()
    {
        var checkNewScheme = playerInput.currentControlScheme;
        if (checkNewScheme != _currentControlScheme)
        {
            _currentControlScheme = checkNewScheme;

            if (onControlSchemeChange != null)
            {
                onControlSchemeChange(playerInput);
                Debug.Log("Send Control Change Event");
            }
        }
    }

    public void HandleAllInputs()
    {
        if (GameManager.canInput)
        {
            HandleMovementInput();
            HandleJumpingInput();
            HandleAttackInput();
            HandleInteractInput();
        }
    }

    private void HandleMovementInput()
    {
        VerticalInput = _inputMovementVector.y;
        HorizontalInput = _inputMovementVector.x;

        MoveAmount = Mathf.Clamp01(Mathf.Abs(HorizontalInput) + Mathf.Abs(VerticalInput));
    }

    private void HandleJumpingInput()
    {
        if (jumpInput)
        {
            if (canTriggerJumpInput)
            {
                playerMain.PlayerMovement.HandleJumping();
                canTriggerJumpInput = false;
            }
        }

        else
            canTriggerJumpInput = true;
    }

    private void HandleAttackInput()
    {
        if (attackInput)
        {
            attackInput = false;
            playerMain.PlayerCombatController.LaunchAttack();
        }
    }

    private void HandleInteractInput()
    {
        if (interactInput)
        {
            interactInput = false;
            playerMain.PlayerInteractController.SendInteractionEvent();
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += ctx => _inputMovementVector = ctx.ReadValue<Vector2>();

            playerControls.PlayerActions.Sprint.performed += ctx => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += ctx => sprintInput = false;

            playerControls.PlayerActions.Jump.performed += ctx => jumpInput = true;
            playerControls.PlayerActions.Jump.canceled += ctx => jumpInput = false;

            playerControls.PlayerActions.Glide.performed += ctx => glideInput = true;
            playerControls.PlayerActions.Glide.canceled += ctx => glideInput = false;

            playerControls.PlayerActions.AnyGlide.performed += ctx => anyglideInput = true;
            playerControls.PlayerActions.AnyGlide.canceled += ctx => anyglideInput = false;

            playerControls.PlayerActions.Attack.performed += ctx => attackInput = true;

            playerControls.PlayerActions.Interact.performed += ctx => interactInput = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
