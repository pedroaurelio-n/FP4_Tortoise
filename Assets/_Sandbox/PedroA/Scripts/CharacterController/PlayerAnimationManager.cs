using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private bool applyAnimationSnapping;
    private Animator playerAnimator;

    private int _horizontal;
    private int _vertical;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();

        _horizontal = Animator.StringToHash("Horizontal");
        _vertical = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float horizontalValue;
        float verticalValue;

        if (applyAnimationSnapping)
        {
            Vector2 snappedValue = ApplyAnimationSnapping(horizontalMovement, verticalMovement);

            horizontalValue = snappedValue.x;
            verticalValue = snappedValue.y;
        }

        else
        {
            horizontalValue = horizontalMovement;
            verticalValue = verticalMovement;
        }

        if (isSprinting && verticalMovement > 0.5f)
        {
            verticalValue = 2;
        }

        playerAnimator.SetFloat(_horizontal, horizontalValue, 0.1f, Time.deltaTime);
        playerAnimator.SetFloat(_vertical, verticalValue, 0.1f, Time.deltaTime);
    }

    private Vector2 ApplyAnimationSnapping(float horizontalMovement, float verticalMovement)
    {
        float snappedHorizontal;
        float snappedVertical;

        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            snappedHorizontal = 0.5f;

        else if (horizontalMovement > 0.55f)
            snappedHorizontal = 1;

        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            snappedHorizontal = -0.5f;

        else if (horizontalMovement < -0.55f)
            snappedHorizontal = -1;

        else
            snappedHorizontal = 0;

        
        if (verticalMovement > 0 && verticalMovement < 0.55f)
            snappedVertical = 0.5f;

        else if (verticalMovement > 0.55f)
            snappedVertical = 1;

        else if (verticalMovement < 0 && verticalMovement > -0.55f)
            snappedVertical = -0.5f;

        else if (verticalMovement < -0.55f)
            snappedVertical = -1;

        else
            snappedVertical = 0;

        return new Vector2(snappedHorizontal, snappedVertical);
    }

    public bool GetInteractionBool()
    {
        return playerAnimator.GetBool("isInteracting");
    }

    public bool GetJumpingBool()
    {
        return playerAnimator.GetBool("isJumping");
    }

    public AnimatorStateInfo GetCurrentAnimation()
    {
        return playerAnimator.GetCurrentAnimatorStateInfo(0);
    }

    public void HandleJumpingAnimation(string jump)
    {
        playerAnimator.SetTrigger(jump);
    }

    public void SetGroundedBool(bool isGrounded)
    {
        playerAnimator.SetBool("isGrounded", isGrounded);
    }

    public void PlayTargetAnimation(string targetAnimation)
    {
        playerAnimator.CrossFade(targetAnimation, 0.2f);
    }
}
