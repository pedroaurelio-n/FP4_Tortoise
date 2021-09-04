using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraObject;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header("Movement Speeds")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float rotationSpeed;
    private Vector3 _moveDirection;

    [Header("Jump Configs")]
    public float jumpHeight;
    public float gravityIntensity;

    [Header("Air Configs")]
    public float raycastHeightOffset;
    public float spherecastRadius;
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float groundHitLerpRate;
    public LayerMask groundLayer;

    private InputManager inputManager;
    private PlayerManager playerManager;
    private PlayerAnimatorManager animatorManager;
    private Rigidbody playerRigidBody;
    private CapsuleCollider playerCapsuleCollider;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<PlayerAnimatorManager>();

        playerRigidBody = GetComponent<Rigidbody>();
        playerCapsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

         if (playerManager.isInteracting)
             return;
            
        if (isJumping)
            return;

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        _moveDirection = cameraObject.forward * inputManager.VerticalInput;
        _moveDirection = _moveDirection + cameraObject.right * inputManager.HorizontalInput;
        _moveDirection.Normalize();
        _moveDirection.y = 0;

        if (isSprinting)
        {
            _moveDirection = _moveDirection * sprintingSpeed;
        }

        else
        {
            if (inputManager.MoveAmount >= 0.5f)
            {
                _moveDirection = _moveDirection * runningSpeed;
            }

            else
            {
                _moveDirection = _moveDirection * walkingSpeed;
            }
        }

        Vector3 movementVelocity = _moveDirection;
        playerRigidBody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.VerticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.HorizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        Vector3 targetPosition;
        raycastOrigin.y = raycastOrigin.y + raycastHeightOffset;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidBody.AddForce(transform.forward * leapingVelocity);
            playerRigidBody.AddForce(Vector3.down * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(raycastOrigin, spherecastRadius, Vector3.down, out hit, groundLayer))
        {
            if (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
            }

            Vector3 raycastHitPoint = hit.point;
            targetPosition.y = raycastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }

        else
            isGrounded = false;

        if (isGrounded && !isJumping)
        {
            if (playerManager.isInteracting || inputManager.MoveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / groundHitLerpRate);
            }

            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.HandleJumpingAnimation();

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = _moveDirection;
            playerVelocity.y = jumpingVelocity;

            playerRigidBody.velocity = playerVelocity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y = raycastOrigin.y + raycastHeightOffset;

        Gizmos.DrawWireSphere(raycastOrigin, spherecastRadius);
    }
}
