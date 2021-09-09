using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField] private PlayerMain2 playerMain;
    [SerializeField] private Camera mainCamera;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool canDoubleJump;

    [Header("Speeds")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Jump & Falling Configs")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityAmplifierGrounded;
    [SerializeField] private float gravityAmplifierMidAir;
    [SerializeField] private int additionalJumps;
    [SerializeField] private float groundToFallingDelay;


    private Vector3 _movementDirection;
    private Vector3 _playerVelocity;

    private int jumpsQuantity;

    private CharacterController playerCharacterController;

    private void Awake()
    {
        playerCharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        canDoubleJump = false;
        isGrounded = true;
    }

    private void GroundCheck(bool check)
    {
        if (isGrounded != check)
            isGrounded = check;
    }

    public void HandleUpdateMovements()
    {
        if (isGrounded && _playerVelocity.y < 0)
            _playerVelocity.y = 0f;
    }

    public void HandleFixedUpdateMovements()
    {
        HandleMovement();
        HandleRotation();
    }

    public void HandleLateUpdateMovements()
    {
        GroundCheck(playerCharacterController.isGrounded);
        playerMain.PlayerAnimationManager.SetGroundedBool(isGrounded);
        
        if (isGrounded)
            jumpsQuantity = additionalJumps;
    }

    private void HandleMovement()
    {
        _movementDirection = mainCamera.transform.forward * playerMain.PlayerInputManager.VerticalInput
                            + mainCamera.transform.right * playerMain.PlayerInputManager.HorizontalInput;
        
        _movementDirection.Normalize();
        _movementDirection.y = 0;

        Vector3 movementVelocity;
        isSprinting = playerMain.PlayerInputManager.sprintInput;


        if (isSprinting && isGrounded)
        {
            movementVelocity = _movementDirection * sprintingSpeed;
        }

        else
        {
            if (playerMain.PlayerInputManager.MoveAmount >= 0.5f)
            {
                movementVelocity = _movementDirection * runningSpeed;
            }

            else
            {
                movementVelocity = _movementDirection * walkingSpeed;
            }
        }

        playerCharacterController.Move(movementVelocity * Time.deltaTime);

        if (isGrounded)
        {
            _playerVelocity.y += Physics.gravity.y * -gravityAmplifierGrounded * Time.deltaTime;
            playerCharacterController.Move(_playerVelocity * Time.deltaTime);
        }

        else
        {
            _playerVelocity.y += Physics.gravity.y * -gravityAmplifierMidAir * Time.deltaTime;
            playerCharacterController.Move(_playerVelocity * Time.deltaTime);
        }

        playerMain.PlayerAnimationManager.UpdateAnimatorValues(0, playerMain.PlayerInputManager.MoveAmount, isSprinting);
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = _movementDirection;
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    private void HandleFalling()
    {
        //create transition movement -> falling from:
            //height bigger than x
            //or
            //fall time bigger than y
    }

    public void HandleJumping()
    {
        if (jumpsQuantity > 0)
        {
            bool canDoubleJump = playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Jump") ||
                                        playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Falling") ||
                                        (!isGrounded && playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Movement"));

            if (canDoubleJump)
                playerMain.PlayerAnimationManager.HandleJumpingAnimation("hasDoubleJumped");

            else
                playerMain.PlayerAnimationManager.HandleJumpingAnimation("hasJumped");

            jumpsQuantity--;
            _playerVelocity.y = Mathf.Sqrt(jumpHeight * gravityAmplifierMidAir * Physics.gravity.y);
        
        }
    }
}
