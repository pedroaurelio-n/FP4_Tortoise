using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAnimatorManager playerAnimatorManager;

    private Vector2 _movementInput;
    public float MoveAmount;
    [HideInInspector] public float VerticalInput;
    [HideInInspector] public float HorizontalInput;

    public bool sprintInput;
    public bool jumpInput;

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
    }

    private void HandleMovementInput()
    {
        VerticalInput = _movementInput.y;
        HorizontalInput = _movementInput.x;
        MoveAmount = Mathf.Clamp01(Mathf.Abs(HorizontalInput) + Mathf.Abs(VerticalInput));
        playerAnimatorManager.UpdateAnimatorValues(0, MoveAmount, playerMovement.isSprinting);
    }

    private void HandleSprintingInput()
    {
        if (sprintInput && MoveAmount > 0.5f)
            playerMovement.isSprinting = true;

        else
            playerMovement.isSprinting = false;
    }

    private void HandleJumpingInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            playerMovement.HandleJumping();
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();

            playerControls.PlayerActions.Sprint.performed += ctx => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += ctx => sprintInput = false;

            playerControls.PlayerActions.Jump.performed += ctx => jumpInput = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
