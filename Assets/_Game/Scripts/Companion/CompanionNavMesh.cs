using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionNavMesh : MonoBehaviour
{
    [SerializeField] private CompanionMain companionMain;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float minimumSpeed;
    [SerializeField] private float minimumDistance;
    [SerializeField] private Transform _Dynamic;


    public bool canHover;
    //private PlayerMovement playerMovement;
    private PlayerInputManager playerInputManager;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerInputManager = companionMain.playerCompanionPlacement.gameObject.GetComponentInParent<PlayerInputManager>();
    }

    private void Start()
    {
        transform.parent = _Dynamic;
    }

    public void FollowPlayer()
    {
        var isAgentCloseToTarget = Vector3.Distance(companionMain.playerCompanionPlacement.position + offset, transform.position) < minimumDistance;
        var isAgentSomewhatClose = Vector3.Distance(companionMain.playerCompanionPlacement.position + offset, transform.position) < minimumDistance+0.5f;

        if (companionMain.playerMovement.isSprinting)
            navMeshAgent.speed = sprintingSpeed;
        else
        {
            var moveSpeed = Mathf.Lerp(minimumSpeed, runningSpeed, playerInputManager.MoveAmount);
            navMeshAgent.speed = moveSpeed;
        }

        if (!isAgentCloseToTarget)
        {
            navMeshAgent.SetDestination(companionMain.playerCompanionPlacement.position + offset);

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
        Gizmos.DrawWireSphere(companionMain.playerCompanionPlacement.position + offset, minimumDistance);
    }
}
