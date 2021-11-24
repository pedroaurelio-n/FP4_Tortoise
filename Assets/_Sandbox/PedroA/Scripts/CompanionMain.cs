using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CompanionMain : MonoBehaviour
{
    public delegate void AttackPlacementCompleted();
    public static event AttackPlacementCompleted onPlacementCompleted;

    [SerializeField] private GameObject mesh;
    [SerializeField] private CompanionNavMesh companionNavMesh;
    public Transform playerCompanionPlacement;
    [SerializeField] private Transform playerCompanionSocket;
    [SerializeField] private PlayerGroundCheck playerGroundCheck;
    [SerializeField] private float smoothTime;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private Vector3 lookOffset;

    [SerializeField] private float upHoverOffset;
    [SerializeField] private float downHoverOffset;
    [SerializeField] private float hoverDuration;
    [SerializeField] private Ease hoverEase;
    [SerializeField] private float movementToAttackDuration;
    [SerializeField] private float attackToMovementDuration;
    [SerializeField] private Ease attackEase;

    private Rigidbody rb;
    private Vector3 _velocityVector;
    private float startYPosition;
    private Coroutine hoverCoroutine;
    private bool isAttacking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startYPosition = playerCompanionPlacement.position.y;

        hoverCoroutine = StartCoroutine(StartHover());
    }

    private void Update()
    {
        companionNavMesh.FollowPlayer();

        var lookDirection = Quaternion.LookRotation(playerCompanionPlacement.position + lookOffset - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (isAttacking)
            return;

        if (playerGroundCheck.isGrounded)
        {
            if (!companionNavMesh.gameObject.activeInHierarchy)
            {
                companionNavMesh.transform.position = transform.position;
                companionNavMesh.gameObject.SetActive(true);
            }
            var navMeshPosition = companionNavMesh.transform.position;
            var desiredPosition = new Vector3(navMeshPosition.x, playerCompanionPlacement.position.y, navMeshPosition.z);

            /*if (!isCoroutineActive && companionNavMesh.canHover)
                hoverCoroutine = StartCoroutine(StartHover());
            
            /*else if (isCoroutineActive && hoverCoroutine != null && !companionNavMesh.canHover)
            {
                StopCoroutine(hoverCoroutine);
                isCoroutineActive = false;

                desiredPosition.y = navMeshPosition.y;
                //playerCompanionSocket.DOLocalMoveY(startYPosition, hoverDuration).SetEase(hoverEase);
            }*/

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocityVector, smoothTime, maxSpeed);

        }

        else
        {
            if (companionNavMesh.gameObject.activeInHierarchy)
            {
                companionNavMesh.gameObject.SetActive(false);
            }

            /*if (isCoroutineActive && hoverCoroutine != null)
            {
                StopCoroutine(hoverCoroutine);
                isCoroutineActive = false;

                playerCompanionSocket.DOMoveY(startYPosition, hoverDuration).SetEase(hoverEase);
            }*/
            
            var desiredPosition = playerCompanionPlacement.position;
            rb.MovePosition(Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocityVector, smoothTime, maxSpeed));

        }

        var lookDirection = Quaternion.LookRotation(playerCompanionPlacement.position + lookOffset - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed);
    }

    private IEnumerator StartHover()
    {
        while (true)
        {
            playerCompanionPlacement.DOLocalMoveY(startYPosition + downHoverOffset, hoverDuration).SetEase(hoverEase);
            yield return new WaitForSeconds(hoverDuration);

            playerCompanionPlacement.DOLocalMoveY(startYPosition + upHoverOffset, hoverDuration).SetEase(hoverEase);
            yield return new WaitForSeconds(hoverDuration);

            yield return null;
        }
    }

    private void ActivateAttackMovement()
    {
        isAttacking = true;
        mesh.transform.DOMove(playerCompanionSocket.position, movementToAttackDuration).SetEase(attackEase).OnComplete(delegate {

            if (onPlacementCompleted != null)
                onPlacementCompleted();

            mesh.SetActive(false);
            });
    }

    private void DeactivateAttackMovement()
    {
        mesh.transform.position = playerCompanionSocket.position;
        mesh.SetActive(true);
        mesh.transform.DOMove(transform.position, attackToMovementDuration).SetEase(attackEase).OnComplete(delegate {isAttacking = false;});
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
