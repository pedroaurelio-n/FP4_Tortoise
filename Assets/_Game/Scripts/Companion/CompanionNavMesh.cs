using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionNavMesh : MonoBehaviour
{
    [Header("Companion References")]
    [SerializeField] private CompanionMain companionMain;
    [SerializeField] private Animator animator;

    [Header("NavMesh Configs")]
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float minimumSpeed;
    [SerializeField] private float minimumDistance;

    [Header("Runtime Transform")]
    [SerializeField] private Transform _Dynamic;

    public bool canHover;
    private PlayerInputManager playerInputManager;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerInputManager = companionMain.companionDesiredPlacement.gameObject.GetComponentInParent<PlayerInputManager>();
    }

    private void Start()
    {
        transform.parent = _Dynamic;
    }

    public void FollowPlayer()
    {
        var isAgentCloseToTarget = Vector3.Distance(companionMain.companionDesiredPlacement.position + offset, transform.position) < minimumDistance;
        var isAgentSomewhatClose = Vector3.Distance(companionMain.companionDesiredPlacement.position + offset, transform.position) < minimumDistance + 0.5f;

        if (companionMain.playerMovement.isSprinting)
            navMeshAgent.speed = sprintingSpeed;
        else
        {
            var moveSpeed = Mathf.Lerp(minimumSpeed, runningSpeed, playerInputManager.MoveAmount);
            navMeshAgent.speed = moveSpeed;
        }

        if (!isAgentCloseToTarget)
        {
            navMeshAgent.SetDestination(companionMain.companionDesiredPlacement.position + offset);

            animator.SetBool("isMoving", true);
            
            if (!isAgentSomewhatClose && canHover)
                canHover = false;
        }

        else
        {
            animator.SetBool("isMoving", false);

            if (isAgentSomewhatClose && !canHover)
                canHover = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(companionMain.companionDesiredPlacement.position + offset, minimumDistance);
    }
}
