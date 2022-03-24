using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerMain playerMain;
    public PlayerGroundCheck groundCheck;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject jumpParticle;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isFalling;
    public bool isGliding;

    [Header("Speeds")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float defaultRotationSpeed;
    [Range(0f, 1f)] [SerializeField] private float attackRotationSpeedMultiplier;

    [Header("Jump & Falling Configs")]
    [SerializeField] private float stepOffset;
    [SerializeField] private float waitForFall;
    [SerializeField] private float normalJumpHeight;
    [SerializeField] private float doubleJumpHeightDecreaser;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float gravityAmplifierGrounded;
    [SerializeField] private float gravityAmplifierMidAir;
    [SerializeField] private int additionalJumps;
    [SerializeField] private float glideVelocityDecreaseDuration;
    [Range(0.05f, 0.9f)] [SerializeField] private float glideGravityReducer = 0.1f;
    [SerializeField] private bool isGlideConstant;
    [Range(-5, -0.1f)] [SerializeField] private float glideYVelocity = -2.5f;

    [Header("Slope Detection Configs")]
    [SerializeField] private float sphereRadius;
    [SerializeField] private Vector3 sphereOffset;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask groudMask;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float minimumSlopeAngle;


    private Vector3 _movementDirection;
    private Vector3 _playerVelocity;
    private Vector3 _playerGlideVelocity;
    private Vector3 _knockbackVelocity;

    private bool isOnPushForceArea;
    private bool isPushForceUp;
    private bool isPushForceOngoing;
    private Vector3 _pushForceVelocity;
    private Coroutine _startPushCoroutine;
    private Coroutine _endPushCoroutine;

    private bool isOnKnockback;
    private int _jumpsQuantity;
    private Coroutine _fallCoroutine;

    private bool isSettingStepOffset;

    private Vector3 _hitNormal;
    private float _hitNormalAngle;
    private bool isOnSlope;

    private CharacterController playerCharacterController;

    private float _airTimeElapsed;

    private void Awake()
    {
        playerCharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        isFalling = false;
        isOnKnockback = false;
    }

    public void HandleUpdateMovements()
    {
        if (groundCheck.isGrounded && _playerVelocity.y < 0)
        {
            _playerVelocity = Vector3.zero;
        }
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
            playerCharacterController.stepOffset = stepOffset;
            _jumpsQuantity = additionalJumps;
            
            if (isFalling)
            {
                isFalling = false;
                StopCoroutine(_fallCoroutine);
            }
        }

        else
        {
            if (!isFalling && !isOnKnockback && !isGliding)
            {
                isFalling = true;
                _fallCoroutine = StartCoroutine(TriggerFalling());
            }
            
            if (!isSettingStepOffset)
                StartCoroutine(SetZeroStepOffset());
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

        if (isSprinting)
        {
            if (groundCheck.isGrounded)
            {
                movementVelocity = _movementDirection * sprintingSpeed * moveAmount;
                _airTimeElapsed = 0;
            }
            
            else
            {
                if (_airTimeElapsed < glideVelocityDecreaseDuration)
                {
                    float actualSpeed = Mathf.Lerp(sprintingSpeed, runningSpeed, _airTimeElapsed/glideVelocityDecreaseDuration);
                    _airTimeElapsed += Time.deltaTime;
                    movementVelocity = _movementDirection * actualSpeed * moveAmount;
                }

                else
                    movementVelocity = _movementDirection * runningSpeed * moveAmount;
            }
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

        if (!playerMain.PlayerCombatController.isAttacking)
        {
            var movementVector = new Vector3(movementVelocity.x + _playerVelocity.x, _playerVelocity.y, movementVelocity.z + _playerVelocity.z);

            playerCharacterController.Move(movementVector * Time.deltaTime + _knockbackVelocity * Time.deltaTime + _pushForceVelocity * Time.deltaTime);
            playerMain.PlayerAnimationManager.UpdateAnimatorValues(0, playerMain.PlayerInputManager.MoveAmount, isSprinting);
        }

    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = _movementDirection;
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        float rotationSpeed;

        if (playerMain.PlayerCombatController.isAttacking)
            rotationSpeed = defaultRotationSpeed * attackRotationSpeedMultiplier;
        else
            rotationSpeed = defaultRotationSpeed;
        
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    private void HandleFalling()
    {
        CheckSphereCast();

        Vector3 slopeForce;

        if (playerMain.PlayerInputManager.jumpInput)
        {
            slopeForce = CalculateSlopeForce(slideSpeed * 0.3f);
        }

        else
            slopeForce = CalculateSlopeForce(slideSpeed);

        if (groundCheck.isGrounded)
        {            
            isGliding = false;
            _playerVelocity.y += Physics.gravity.y * -gravityAmplifierGrounded * Time.deltaTime;
            _playerGlideVelocity = Vector3.zero;
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

                if (isGlideConstant)
                    _playerGlideVelocity.y = glideYVelocity;
                else
                    _playerGlideVelocity.y += Physics.gravity.y * (-gravityAmplifierMidAir * glideGravityReducer) * Time.deltaTime;

                if (!isOnPushForceArea)
                {
                    _playerVelocity.y = _playerGlideVelocity.y;
                }
            }

            else
            {
                isGliding = false;
                
                if (!isOnPushForceArea || !isPushForceUp)
                {
                    if (!playerMain.PlayerInputManager.jumpInput)
                    {
                        _playerVelocity.y += Physics.gravity.y * -gravityAmplifierMidAir * lowJumpMultiplier * Time.deltaTime;
                    }
                    
                    else
                    {
                        _playerVelocity.y += Physics.gravity.y * -gravityAmplifierMidAir * Time.deltaTime;
                    }
                }
            }

            _playerVelocity.x += slopeForce.x;
            _playerVelocity.z += slopeForce.z;

            playerMain.PlayerAnimationManager.SetGlidingBool(isGliding);
        }
    }

    public void HandleJumping()
    {
        if (_jumpsQuantity > 0 && !playerMain.PlayerCombatController.isAttacking)
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
                var temp = Instantiate(jumpParticle, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
                var lenght = temp.GetComponent<ParticleSystem>().main.duration;
                Destroy(temp, lenght);
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

    private void CheckSphereCast()
    {
        RaycastHit hit;

        var sphereCast = Physics.SphereCast(transform.position + sphereOffset, sphereRadius, Vector3.down, out hit, maxDistance, groudMask);

        if (sphereCast)
        {
            _hitNormal = hit.normal;
            _hitNormalAngle = Vector3.Angle(Vector3.up, _hitNormal);

            if (_hitNormalAngle > minimumSlopeAngle)
                isOnSlope = true;
            else
            {
                _playerVelocity.x = 0f;
                _playerVelocity.z = 0f;
                isOnSlope = false;
            }
        }
        else
        {
            _playerVelocity.x = 0f;
            _playerVelocity.z = 0f;
            isOnSlope = false;
        }
    }

    private Vector3 CalculateSlopeForce(float slideSpeed)
    {
        Vector3 slopeDirection;
        float slideX = 0f;
        float slideZ = 0f;

        if (isOnSlope)
        {
            slideX += ((1f - _hitNormal.y) * _hitNormal.x) * slideSpeed;
            slideZ += ((1f - _hitNormal.y) * _hitNormal.z) * slideSpeed;

            slopeDirection = new Vector3(slideX, 0, slideZ);
        }
        else
        {
            slopeDirection = Vector3.zero;
        }
        
        return slopeDirection;
    }

    private IEnumerator TriggerFalling()
    {
        yield return new WaitForSeconds(waitForFall);

        if ((!groundCheck.isGrounded && (playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Movement") || playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Landing")) && _playerVelocity.y < 0) ||
            (!groundCheck.isGrounded && (playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Movement") || playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Landing")) && _pushForceVelocity.y > 0))
        {
            playerMain.PlayerAnimationManager.SetTrigger("isFalling");
        }
    }

    public void TriggerKnockback(Vector3 direction, float knockbackHorizontal, float knockbackVertical, float knockbackTime)
    {
        isOnKnockback = true;
        _knockbackVelocity = direction * knockbackHorizontal * Time.deltaTime;
        _knockbackVelocity.y = knockbackVertical;
        StartCoroutine(ResetKnockbackVelocity(knockbackTime));
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

    public void SetPushForceAreaBool(bool value)
    {
        isOnPushForceArea = value;
    }

    public void SetPushForceUpBool(bool value)
    {
        isPushForceUp = value;
    }

    public void ActivatePushForce(Vector3 direction, float force, float startDuration)
    {       
        if (_endPushCoroutine != null)
        {
            StopCoroutine(_endPushCoroutine);
            _endPushCoroutine = null;
            _pushForceVelocity = Vector3.zero;
        }

        if (direction.y > 0)
        {
            groundCheck.DeactivateGroundCheck();
            _fallCoroutine = StartCoroutine(TriggerFalling());
        }

        _startPushCoroutine = StartCoroutine(StartPushForce(direction, force, startDuration));
    }

    private IEnumerator StartPushForce(Vector3 direction, float force, float startDuration)
    {
        /*isPushForceOngoing = true;
        float elapsedTime = 0;
        Vector3 start = Vector3.zero;

        while (elapsedTime < startDuration)
        {
            _pushForceVelocity = Vector3.Lerp(start, direction * force, (elapsedTime/startDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _pushForceVelocity = direction * force;
        _startPushCoroutine = null;
        yield return null;*/

        isPushForceOngoing = true;        
        var maxMagnitude = direction.magnitude * force;
        while (isOnPushForceArea)
        {
            if (_playerVelocity.y < 0)
                _playerVelocity.y += direction.y * force * 2 * Time.deltaTime;

            _pushForceVelocity += direction * force * Time.deltaTime;

            if (_pushForceVelocity.magnitude > maxMagnitude)
                _pushForceVelocity = direction * force;
            
            yield return null;
        }

        _pushForceVelocity = direction * force;
        _startPushCoroutine = null;
        yield return null;
    }

    public void DeactivatePushForce(float endDuration)
    {
        if (isOnPushForceArea || !isPushForceOngoing || _pushForceVelocity == Vector3.zero)
        {
            return;
        }

        if (_startPushCoroutine != null)
        {
            StopCoroutine(_startPushCoroutine);
            _startPushCoroutine = null;
        }
        
        _endPushCoroutine = StartCoroutine(EndPushForce(endDuration));
    }

    private IEnumerator EndPushForce(float endDuration)
    {
        float elapsedTime = 0;
        Vector3 start = _pushForceVelocity;

        while (elapsedTime < endDuration)
        {
            _pushForceVelocity = Vector3.Lerp(start, Vector3.zero, (elapsedTime/endDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isPushForceOngoing = false;
        _pushForceVelocity = Vector3.zero;
        _endPushCoroutine = null;
        yield return null;
    }

    private IEnumerator SetZeroStepOffset()
    {
        isSettingStepOffset = true;
        yield return new WaitForSeconds(0.1f);
        playerCharacterController.stepOffset = 0f;

        isSettingStepOffset = false;
    }

    public Vector3 GetPlayerVelocity() { return playerCharacterController.velocity; }
    public int GetAvalilableJumps() { return _jumpsQuantity; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, sphereRadius);
    }
}
