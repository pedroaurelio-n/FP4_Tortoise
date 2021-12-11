using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CompanionAttackController : MonoBehaviour
{
    public delegate void AttackPlacementCompleted();
    public static event AttackPlacementCompleted onAttackPlacementCompleted;
    
    [Header("Entity References")]
    [SerializeField] private CompanionMain companionMain;
    [SerializeField] private PlayerCombatController playerCombatController;

    [Header("Attack Configs")]
    [SerializeField] private float movementToAttackDuration;
    [SerializeField] private float attackToMovementDuration;
    [SerializeField] private float attackToMovementDelay;
    [SerializeField] private Ease attackEase;

    public bool isAttacking;
    private bool isReturningFromAttack;

    private void ActivateAttackMovement()
    {
        if (!isReturningFromAttack)
        {
            companionMain.companionAlertMessage.HideMessage(movementToAttackDuration);
            StartCoroutine(StartAttack());
        }
    }

    private void DeactivateAttackMovement()
    {
        StartCoroutine(ReturnFromAttack());
    }

    private IEnumerator StartAttack()
    {
        yield return null;

        isAttacking = true;
        companionMain.companionEntityMesh.transform.DOScale(Vector3.zero, movementToAttackDuration).SetEase(attackEase);
        companionMain.companionEntityMesh.transform.DOMove(companionMain.playerCompanionSocket.position, movementToAttackDuration).SetEase(attackEase).OnComplete(delegate {
            if (onAttackPlacementCompleted != null)
            {
                onAttackPlacementCompleted();
            }

            companionMain.companionEntityMesh.SetActive(false);
            companionMain.companionSocketTrail.enabled = true;
        });
    }

    private IEnumerator ReturnFromAttack()
    {
        isReturningFromAttack = true;
        yield return new WaitForSeconds(attackToMovementDelay);
        
        transform.position = companionMain.companionDesiredPlacement.position;
        companionMain.companionEntityMesh.transform.position = companionMain.playerCompanionSocket.position;
        companionMain.companionEntityMesh.SetActive(true);
        companionMain.companionSocketTrail.enabled = false;
        companionMain.companionEntityMesh.transform.DOScale(Vector3.one, attackToMovementDuration).SetEase(attackEase);
        companionMain.companionEntityMesh.transform.DOMove(transform.position, attackToMovementDuration).SetEase(attackEase).OnComplete(delegate {
            isAttacking = false; 
            isReturningFromAttack = false; 
            companionMain.companionEntityMesh.SetActive(true);
            playerCombatController.ResetAttackState();
        });
    }

    private void OnEnable()
    {
        PlayerCombatController.onAttackStart += ActivateAttackMovement;
        PlayerCombatController.onAttackEnd += DeactivateAttackMovement;
    }

    private void OnDisable()
    {
        PlayerCombatController.onAttackStart -= ActivateAttackMovement;
        PlayerCombatController.onAttackEnd -= DeactivateAttackMovement;
    }
}
