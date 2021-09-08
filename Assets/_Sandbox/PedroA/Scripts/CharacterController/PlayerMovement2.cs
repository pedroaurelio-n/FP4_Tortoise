using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public delegate void TestHeight();
    public static event TestHeight testHeight;


    [SerializeField] private PlayerMain2 playerMain;
    [SerializeField] private Camera mainCamera;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;

    [Header("Speeds")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Jump Configs")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityAmplifierGrounded;
    [SerializeField] private float gravityAmplifierMidAir;


    private Vector3 _movementDirection;
    private Vector3 _playerVelocity;

    private CharacterController playerCharacterController;

    private void Awake()
    {
        playerCharacterController = GetComponent<CharacterController>();
    }

    public void HandleUpdateMovements()
    {
        isGrounded = playerCharacterController.isGrounded;
        playerMain.PlayerAnimationManager.SetGroundedBool(isGrounded);

        if (isGrounded && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }
    }

    public void HandleFixedUpdateMovements()
    {
        HandleMovement();
        HandleRotation();
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

        //Vector3 movementVelocity = new Vector3(_movementDirection.x, 0, _movementDirection.y);
        //movementVelocity = _movementDirection;
        //playerRigidBody.AddForce(movementVelocity, ForceMode.Force);


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

    public void HandleJumping()
    {
        if (isGrounded)
        {
            playerMain.PlayerAnimationManager.HandleJumpingAnimation();
            _playerVelocity.y = Mathf.Sqrt(jumpHeight * gravityAmplifierMidAir * Physics.gravity.y);

            if (testHeight != null)
                testHeight();
            //playerMain.PlayerAnimationManager.PlayTargetAnimation("EmptyLanding");
        }
    }
}
