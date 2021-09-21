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
    public bool isGliding;

    [Header("Speeds")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Grounded Configs")]
    public Vector3 platformOffset; 
    [SerializeField] private Vector3 offset;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump & Falling Configs")]
    [SerializeField] private float normalJumpHeight;
    [SerializeField] private float doubleJumpHeightDecreaser;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float gravityAmplifierGrounded;
    [SerializeField] private float gravityAmplifierMidAir;
    [SerializeField] private int additionalJumps;
    [SerializeField] private float glideGravityReducer;


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
        HandleFalling();
        HandleRotation();
    }

    public void HandleLateUpdateMovements()
    {
        //GroundCheck(playerCharacterController.isGrounded);

        var colliders = Physics.OverlapSphere(transform.position + offset + platformOffset, radius, groundLayer);
        bool isColliding = colliders.Length != 0;

        GroundCheck(isColliding);

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
        if (isGrounded)
        {
            isGliding = false;
            _playerVelocity.y += Physics.gravity.y * -gravityAmplifierGrounded * Time.deltaTime;
            playerCharacterController.Move(_playerVelocity * Time.deltaTime);
        }

        else
        {
            bool canGlide = playerMain.PlayerInputManager.glideInput && _playerVelocity.y < 0 && jumpsQuantity <= 0;
            bool canAnyGlide = playerMain.PlayerInputManager.anyglideInput && _playerVelocity.y < 0;

            
            if (canGlide || canAnyGlide)
            {
                isGliding = true;
                _playerVelocity.y += Physics.gravity.y * (-gravityAmplifierMidAir * glideGravityReducer) * Time.deltaTime;
                playerCharacterController.Move(_playerVelocity * Time.deltaTime);
            }

            else
            {
                isGliding = false;
                //_playerVelocity.y += Physics.gravity.y * -gravityAmplifierMidAir * Time.deltaTime;

                if (!playerMain.PlayerInputManager.jumpInput)
                    _playerVelocity.y += Physics.gravity.y * -gravityAmplifierMidAir * lowJumpMultiplier * Time.deltaTime;
                
                else
                    _playerVelocity.y += Physics.gravity.y * -gravityAmplifierMidAir * Time.deltaTime;

                playerCharacterController.Move(_playerVelocity * Time.deltaTime);
            }
        }
    }

    public void HandleJumping()
    {
        if (jumpsQuantity > 0)
        {
            var doubleJumpHeight = normalJumpHeight * doubleJumpHeightDecreaser;
            float jumpHeight;

            bool canDoubleJump = playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Jump") ||
                                        playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Falling") ||
                                        (!isGrounded && playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Movement"));

            if (canDoubleJump)
            {
                jumpHeight = doubleJumpHeight;
                playerMain.PlayerAnimationManager.HandleJumpingAnimation("hasDoubleJumped");
            }

            else
            {
                jumpHeight = normalJumpHeight;
                playerMain.PlayerAnimationManager.HandleJumpingAnimation("hasJumped");
            }

            jumpsQuantity--;
            _playerVelocity.y = Mathf.Sqrt(jumpHeight * gravityAmplifierMidAir * Physics.gravity.y);        
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + offset + platformOffset, radius);
    }
}
