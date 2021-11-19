using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public delegate void PlayerDamageHit(int damage);
    public static event PlayerDamageHit onPlayerDamageHit;

    [SerializeField] private PlayerMain playerMain;
    [SerializeField] private float attackDuration;

    [Header("KnockBack Configs")]
    [SerializeField] private float knockbackHorizontal;
    [SerializeField] private float knockbackVertical;
    [SerializeField] private float knockbackTime;
    [SerializeField] private float invincibilityTime;

    [Header("Combo Timing Configs")]
    [Range(0f, 1f)]
    [SerializeField] private float firstComboTimingPercentage;
    [Range(0f, 1f)]
    [SerializeField] private float secondComboTimingPercentage;

    public bool isAttacking;
    public bool isInvincible;

    private bool isComboPossible;
    private int _comboStep;

    private void Start()
    {
        isAttacking = false;
    }

    public void ComboPossible()
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
            Debug.Log("Launch attack 1");
            playerMain.PlayerAnimationManager.SetAttackBool(1, true);
            _comboStep = 1;
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

        if (onPlayerDamageHit != null)
            onPlayerDamageHit(-enemy.GetAttackDamage());

        playerMain.PlayerMovement.TriggerKnockback(hitNormal, knockbackHorizontal, knockbackVertical, knockbackTime);

        isInvincible = true;
        StartCoroutine(DisableInvincibility());
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
}