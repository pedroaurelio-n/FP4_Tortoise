using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
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
            Debug.Log("attack");

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
}
