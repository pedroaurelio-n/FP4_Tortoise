using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private PlayerMain playerMain;
    [SerializeField] private List<AudioClip> stepClips;
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

    public void PlayAnimation(string animation)
    {
        playerAnimator.Play(animation);
    }

    public AnimatorStateInfo GetCurrentAnimation()
    {
        return playerAnimator.GetCurrentAnimatorStateInfo(0);
    }

    public bool IsAnimatorInTransition()
    {
        return playerAnimator.IsInTransition(0);
    }

    public void SetGroundedBool(bool isGrounded)
    {
        playerAnimator.SetBool("isGrounded", isGrounded);
    }

    public void SetAttackBool(int index, bool value)
    {
        playerAnimator.SetBool("Attack" + index, value);
    }

    public void SetGlidingBool(bool isGliding)
    {
        playerAnimator.SetBool("isGliding", isGliding);
    }

    public void SetTrigger(string param)
    {
        playerAnimator.SetTrigger(param);
    }

    public void SetRootMotion(bool value)
    {
        playerAnimator.applyRootMotion = value;
    }

    public void SetComboStepInt(int value)
    {
        playerAnimator.SetInteger("ComboStep", value);
    }

    public void CombatControllerEvent_ComboCheck()
    {
        playerMain.PlayerCombatController.ComboCheck();
    }

    public void CombatControllerEvent_SetComboPossibleTrue()
    {
        playerMain.PlayerCombatController.SetComboPossible(true);
    }

    public void TriggerFootstepSFX()
    {
        var randomClip = Random.Range(0, stepClips.Count);
        AudioManager.Instance.PlayAudio(stepClips[randomClip]);
    }

    /*public void CombatControllerEvent_ComboPossible()
    {
        playerMain.PlayerCombatController.ComboPossible();
    }

    public void CombatControllerEvent_NextAttack()
    {
        playerMain.PlayerCombatController.NextAttack();
    }

    public void CombatControllerEvent_ResetCombo()
    {
        playerMain.PlayerCombatController.ResetCombo();
    }*/
}
