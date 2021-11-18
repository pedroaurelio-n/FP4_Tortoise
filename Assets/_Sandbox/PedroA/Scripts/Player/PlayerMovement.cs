using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerMain playerMain;
    public PlayerGroundCheck groundCheck;
    //[SerializeField] private PlayerCameraFollow cameraFollow;
    [SerializeField] private Camera mainCamera;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isFalling;
    public bool isGliding;

    [Header("Speeds")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Jump & Falling Configs")]
    [SerializeField] private float waitForFall;
    [SerializeField] private float normalJumpHeight;
    [SerializeField] private float doubleJumpHeightDecreaser;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float gravityAmplifierGrounded;
    [SerializeField] private float gravityAmplifierMidAir;
    [SerializeField] private int additionalJumps;
    [SerializeField] private float glideGravityReducer;


    private Vector3 _movementDirection;
    private Vector3 _playerVelocity;
    private Vector3 _knockbackVelocity;

    private bool isOnKnockback;
    private int _jumpsQuantity;
    private Coroutine _fallCoroutine;

    private CharacterController playerCharacterController;
    private float _startStepOffset;

    private void Awake()
    {
        playerCharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _startStepOffset = playerCharacterController.stepOffset;
        isFalling = false;
        isOnKnockback = false;
    }

    public void HandleUpdateMovements()
    {
        if (groundCheck.isGrounded && _playerVelocity.y < 0)
            _playerVelocity.y = 0f;
    }

    public void HandleFixedUpdateMovements()
    {
        HandleFalling();
        HandleMovement();
        HandleRotation();
    }

    public void HandleLateUpdateMovements()
    {
        playerMain.PlayerAnimationManager.SetGroundedBool(groundCheck.isGrounded);
        
        if (groundCheck.isGrounded)
        {
            playerCharacterController.stepOffset = _startStepOffset;
            _jumpsQuantity = additionalJumps;
            
            if (isFalling)
            {
                //cameraFollow.OnHitGround();
                isFalling = false;
                StopCoroutine(_fallCoroutine);
            }
        }

        else
        {
            if (!isFalling && !isOnKnockback)
            {
                isFalling = true;
                //groundCheck.ActivateGroundCheck();
                _fallCoroutine = StartCoroutine(TriggerFalling());
            }
            
            playerCharacterController.stepOffset = 0f;
        }
    }

    private void HandleMovement()
    {
        _movementDirection = mainCamera.transform.forward * playerMain.PlayerInputManager.VerticalInput
                            + mainCamera.transform.right * playerMain.PlayerInputManager.HorizontalInput;
        
        _movementDirection.y = 0;
        _movementDirection.Normalize();

        Vector3 movementVelocity;
        isSprinting = playerMain.PlayerInputManager.sprintInput;

        var moveAmount = playerMain.PlayerInputManager.MoveAmount;

        if (isSprinting && groundCheck.isGrounded)
        {
            movementVelocity = _movementDirection * sprintingSpeed * moveAmount;
        }

        else
        {
            if (playerMain.PlayerInputManager.MoveAmount >= 0.5f)
            {
                movementVelocity = _movementDirection * runningSpeed * moveAmount;
            }

            else
            {
                movementVelocity = _movementDirection * walkingSpeed * moveAmount;
            }
        }

        playerCharacterController.Move(new Vector3(movementVelocity.x, _playerVelocity.y, movementVelocity.z) * Time.deltaTime + _knockbackVelocity * Time.deltaTime);

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
        if (groundCheck.isGrounded)
        {
            isGliding = false;
            _playerVelocity.y += Physics.gravity.y * -gravityAmplifierGrounded * Time.deltaTime;
        }

        else
        {
            if (_playerVelocity.y < 0)
            {
                groundCheck.ActivateGroundCheck();
            }

            bool canGlide = playerMain.PlayerInputManager.glideInput && _playerVelocity.y < 0 && _jumpsQuantity <= 0;
            bool canAnyGlide = playerMain.PlayerInputManager.anyglideInput && _playerVelocity.y < 0;

            
            if (canGlide || canAnyGlide)
            {
                isGliding = true;
                _playerVelocity.y += Physics.gravity.y * (-gravityAmplifierMidAir * glideGravityReducer) * Time.deltaTime;
            }

            else
            {
                isGliding = false;

                if (!playerMain.PlayerInputManager.jumpInput)
                    _playerVelocity.y += Physics.gravity.y * -gravityAmplifierMidAir * lowJumpMultiplier * Time.deltaTime;
                
                else
                    _playerVelocity.y += Physics.gravity.y * -gravityAmplifierMidAir * Time.deltaTime;

            }
        }
    }

    public void HandleJumping()
    {
        if (_jumpsQuantity > 0)
        {
            groundCheck.DeactivateGroundCheck();
            var doubleJumpHeight = normalJumpHeight * doubleJumpHeightDecreaser;
            float jumpHeight;

            bool canDoubleJump = !groundCheck.isGrounded && (playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Jump") ||
                                        playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Falling") ||
                                        playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Movement"));

            if (canDoubleJump)
            {
                jumpHeight = doubleJumpHeight;
                playerMain.PlayerAnimationManager.SetTrigger("hasDoubleJumped");
                Debug.Log("DoubleJump");
                _jumpsQuantity--;
            }

            else
            {
                jumpHeight = normalJumpHeight;
                playerMain.PlayerAnimationManager.SetTrigger("hasJumped");
            }

            _playerVelocity.y = Mathf.Sqrt(jumpHeight * gravityAmplifierMidAir * Physics.gravity.y);
        }
    }

    public void TriggerKnockback(Vector3 direction, float knockbackHorizontal, float knockbackVertical, float knockbackTime)
    {
        isOnKnockback = true;
        _knockbackVelocity = direction * knockbackHorizontal * Time.deltaTime;
        _knockbackVelocity.y = knockbackVertical;
        StartCoroutine(ResetKnockbackVelocity(knockbackTime));
    }

    private IEnumerator TriggerFalling()
    {
        yield return new WaitForSeconds(waitForFall);

        if (!groundCheck.isGrounded && playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Movement") && _playerVelocity.y < 0)
        {
            Debug.Log("Trigger Fall");
            playerMain.PlayerAnimationManager.SetTrigger("isFalling");
        }
    }

    private IEnumerator ResetKnockbackVelocity(float knockbackTime)
    {
        float elapsedTime = 0;
        Vector3 start = _knockbackVelocity;

        while (elapsedTime < knockbackTime)
        {
            _knockbackVelocity = Vector3.Lerp(start, Vector3.zero, (elapsedTime/knockbackTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _knockbackVelocity = Vector3.zero;
        isOnKnockback = false;
        yield return null;
    }

    public Vector3 GetPlayerVelocity() { return playerCharacterController.velocity; }
    public int GetAvalilableJumps() { return _jumpsQuantity; }
}
