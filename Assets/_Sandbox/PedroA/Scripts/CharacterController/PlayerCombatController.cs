using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public delegate void PlayerDamageHit(Vector3 hitNormal);
    public static event PlayerDamageHit onPlayerDamageHit;

    [SerializeField] private PlayerMain playerMain;
    [SerializeField] private float attackDuration;

    public bool isAttacking;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out EnemyMain enemy))
        {
            Debug.Log("Player Damaged");
            Vector3 hitNormal = transform.position - enemy.transform.position;

            if (onPlayerDamageHit != null)
                onPlayerDamageHit(hitNormal);
        }
    }    
}