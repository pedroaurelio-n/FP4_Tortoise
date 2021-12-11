using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public delegate void AttackStarted();
    public static event AttackStarted onAttackStart;
    public delegate void AttackEnded();
    public static event AttackEnded onAttackEnd;

    public delegate void PlayerDamageHit(int damage);
    public static event PlayerDamageHit onPlayerDamageHit;

    [SerializeField] private PlayerMain playerMain;

    [Header("KnockBack Configs")]
    [SerializeField] private float knockbackHorizontal;
    [SerializeField] private float knockbackVertical;
    [SerializeField] private float knockbackTime;
    [SerializeField] private float invincibilityTime;

    public bool isAttacking;
    public bool isInvincible;

    private bool isComboPossible;
    private int _comboStep;

    private void Start()
    {
        isAttacking = false;
        isComboPossible = true;
        _comboStep = 0;
    }

    public void ResetAttackState()
    {
        isAttacking = false;
        isComboPossible = true;
        _comboStep = 0;
    }

    /*public void ComboPossible()
    {
        isComboPossible = true;
    }

    public void NextAttack()
    {
        if (_comboStep == 2)
        {
            Debug.Log("Launch attack 2");
            playerMain.PlayerAnimationManager.PlayAnimation("Attack_Hit2");
            //playerMain.PlayerAnimationManager.SetTrigger("Attack2");
            //playerMain.PlayerAnimationManager.SetAttackBool(1, false);
            //playerMain.PlayerAnimationManager.SetAttackBool(2, true);
        }
        else if (_comboStep == 3)
        {
            Debug.Log("Launch attack 3");
            playerMain.PlayerAnimationManager.PlayAnimation("Attack_Hit3");
            //playerMain.PlayerAnimationManager.SetTrigger("Attack3");
            //playerMain.PlayerAnimationManager.SetAttackBool(2, false);
            //playerMain.PlayerAnimationManager.SetAttackBool(3, true);
        }
        else
            Debug.Log("Error");

    }

    public void ResetCombo()
    {
        Debug.Log("Combo reset");
        isComboPossible = false;
        isAttacking = false;

        if (onAttackEnd != null)
            onAttackEnd();

        playerMain.PlayerAnimationManager.SetRootMotion(false);
        _comboStep = 0;
        playerMain.PlayerAnimationManager.SetAttackBool(1, false);
        playerMain.PlayerAnimationManager.SetAttackBool(2, false);
        playerMain.PlayerAnimationManager.SetAttackBool(3, false);
    }

    public void LaunchAttack()
    {
        isAttacking = true;

        playerMain.PlayerAnimationManager.SetRootMotion(true);
        if (_comboStep == 0)
        {
            if (onAttackStart != null)
                onAttackStart();

            return;
        }

        else
        {
            if (isComboPossible)
            {
                isComboPossible = false;
                _comboStep++;
            }
        }
    }*/

    public void LaunchAttack()
    {
        if (!playerMain.PlayerMovement.groundCheck.isGrounded)
            return;
            
        if (isComboPossible)
            _comboStep++;
        
        if (_comboStep == 1)
        {
            if (onAttackStart != null)
            {
                onAttackStart();
            }
        }
    }

    public void ComboCheck()
    {
        isComboPossible = false;

        if (playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Attack_Hit1") && _comboStep == 1)
        {
            playerMain.PlayerAnimationManager.SetComboStepInt(0);
            isComboPossible = true;
            isAttacking = false;
            _comboStep = 0;
            StartCoroutine(DisableRootMotion());

            if (onAttackEnd != null)
                onAttackEnd();
        }

        else if (playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Attack_Hit1") && _comboStep >= 2)
        {
            playerMain.PlayerAnimationManager.SetComboStepInt(2);
            isComboPossible = true;
        }

        else if (playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Attack_Hit2") && _comboStep == 2)
        {
            playerMain.PlayerAnimationManager.SetComboStepInt(0);
            isComboPossible = true;
            isAttacking = false;
            _comboStep = 0;
            StartCoroutine(DisableRootMotion());

            if (onAttackEnd != null)
                onAttackEnd();
        }

        else if (playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Attack_Hit2") && _comboStep >= 3)
        {
            playerMain.PlayerAnimationManager.SetComboStepInt(3);
            isComboPossible = true;
        }

        else if (playerMain.PlayerAnimationManager.GetCurrentAnimation().IsName("Attack_Hit3"))
        {
            playerMain.PlayerAnimationManager.SetComboStepInt(0);
            isComboPossible = true;
            isAttacking = false;
            _comboStep = 0;
            StartCoroutine(DisableRootMotion());

            if (onAttackEnd != null)
                onAttackEnd();
        }
    }

    public void StartAttack()
    {
        //Debug.Log("Launch attack 1");
        //playerMain.PlayerAnimationManager.SetAttackBool(1, true);
        isAttacking = true;
        playerMain.PlayerAnimationManager.SetRootMotion(true);
        playerMain.PlayerAnimationManager.SetComboStepInt(1);
        //_comboStep = 1;
    }

    private IEnumerator DisableInvincibility()
    {
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    private void ReceiveDamage(Enemy enemy)
    {
        var hitNormal = transform.position - enemy.transform.position;
        hitNormal.y = 0;
        hitNormal.Normalize();

        Debug.Log(-enemy.GetAttackDamage());

        if (onPlayerDamageHit != null)
            onPlayerDamageHit(-enemy.GetAttackDamage());

        playerMain.PlayerMovement.TriggerKnockback(hitNormal, knockbackHorizontal, knockbackVertical, knockbackTime);

        isInvincible = true;
        StartCoroutine(DisableInvincibility());
    }

    private IEnumerator DisableRootMotion()
    {
        yield return new WaitForSeconds(0.5f);
        playerMain.PlayerAnimationManager.SetRootMotion(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out Enemy enemy) && !isInvincible)
        {
            if (!enemy.CanDamagePlayer())
                return;
                
            ReceiveDamage(enemy);
        }
    }

    private void OnEnable()
    {
        CompanionAttackController.onAttackPlacementCompleted += StartAttack;
    }

    private void OnDisable()
    {
        CompanionAttackController.onAttackPlacementCompleted -= StartAttack;
    }
}