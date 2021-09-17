using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager2 : MonoBehaviour
{
    [SerializeField] private PlayerMain2 playerMain;

    private PlayerControls playerControls;

    private Vector2 _inputMovementVector;
    public float MoveAmount;
    public float HorizontalInput;
    public float VerticalInput;

    public bool jumpInput;
    public bool sprintInput;
    public bool glideInput;
    public bool anyglideInput;

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleJumpingInput();
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
            jumpInput = false;
            playerMain.PlayerMovement.HandleJumping();
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

            playerControls.PlayerActions.Glide.performed += ctx => glideInput = true;
            playerControls.PlayerActions.Glide.canceled += ctx => glideInput = false;

            playerControls.PlayerActions.AnyGlide.performed += ctx => anyglideInput = true;
            playerControls.PlayerActions.AnyGlide.canceled += ctx => anyglideInput = false;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
