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

    public bool isAttacking;
    public bool isInvincible;

    private void Start()
    {
        isAttacking = false;
    }

    public void LaunchAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            playerMain.PlayerAnimationManager.SetTrigger("hasAttacked");

            StartCoroutine(WaitForCurrentAnimation());
        }
    }

    private IEnumerator WaitForCurrentAnimation()
    {
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
    }

    private IEnumerator DisableInvincibility()
    {
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out Enemy enemy) && !isInvincible)
        {
            Vector3 hitNormal = transform.position - enemy.transform.position;

            if (onPlayerDamageHit != null)
                onPlayerDamageHit(-enemy.GetAttackDamage());

            playerMain.PlayerMovement.TriggerKnockback(hitNormal, knockbackHorizontal, knockbackVertical, knockbackTime);

            isInvincible = true;
            StartCoroutine(DisableInvincibility());
        }
    } 
}