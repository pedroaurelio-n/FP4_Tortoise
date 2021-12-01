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
    [SerializeField] private Transform lookAt;
    [HideInInspector] public PlayerMovement playerMovement;
    [SerializeField] private Transform playerCompanionSocket;
    [SerializeField] private PlayerCombatController playerCombatController;
    [SerializeField] private PlayerGroundCheck playerGroundCheck;
    [SerializeField] private float smoothTime;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxSpeed;

    [SerializeField] private float upHoverOffset;
    [SerializeField] private float downHoverOffset;
    [SerializeField] private float hoverDuration;
    [SerializeField] private Ease hoverEase;
    [SerializeField] private float movementToAttackDuration;
    [SerializeField] private float attackToMovementDuration;
    [SerializeField] private float attackToMovementDelay;
    [SerializeField] private Ease attackEase;
    [SerializeField] private float glideMovementDuration;

    private Rigidbody rb;
    private TrailRenderer meshTrail;
    private Vector3 _velocityVector;
    private float startYPosition;
    private Coroutine hoverCoroutine;

    private bool isAttacking;
    private bool isReturningFromAttack;
    private bool isOnGlide;

    private void Awake()
    {
        playerMovement = playerCompanionPlacement.gameObject.GetComponentInParent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        meshTrail = playerCompanionSocket.gameObject.GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        startYPosition = playerCompanionPlacement.position.y;

        hoverCoroutine = StartCoroutine(StartHover());
        meshTrail.enabled = false;
    }

    private void Update()
    {
        companionNavMesh.FollowPlayer();

        if (playerMovement.isGliding && !isOnGlide)
        {
            StartCoroutine(ActivateGlideMovement());
        }

        if (!playerMovement.isGliding && isOnGlide)
        {
            StartCoroutine(DeactivateGlideMovement());
            meshTrail.enabled = false;
        }

        if (Vector3.Distance(mesh.transform.position, transform.position) > 100)
            mesh.transform.position = transform.position;
    }

    private void FixedUpdate()
    {
        if (isAttacking)
            return;

        if (playerGroundCheck.isGrounded && !playerGroundCheck.isOnMovingPlatform)
        {
            if (!companionNavMesh.gameObject.activeInHierarchy)
            {
                companionNavMesh.gameObject.SetActive(true);
                companionNavMesh.transform.position = transform.position;
            }
            var navMeshPosition = companionNavMesh.transform.position;
            var desiredPosition = new Vector3(navMeshPosition.x, playerCompanionPlacement.position.y, navMeshPosition.z);

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocityVector, smoothTime, maxSpeed);

        }

        else
        {
            if (companionNavMesh.gameObject.activeInHierarchy)
            {
                companionNavMesh.gameObject.SetActive(false);
            }
            
            var desiredPosition = playerCompanionPlacement.position;
            rb.MovePosition(Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocityVector, smoothTime, maxSpeed));

        }

        var lookDirection = Quaternion.LookRotation(lookAt.position - transform.position, Vector3.up);
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
        if (!isReturningFromAttack)
            StartCoroutine(StartAttack());
    }

    private void DeactivateAttackMovement()
    {
        StartCoroutine(ReturnFromAttack());
    }

    private IEnumerator StartAttack()
    {
        yield return null;

        isAttacking = true;
        mesh.transform.DOScale(Vector3.zero, movementToAttackDuration).SetEase(attackEase);
        mesh.transform.DOMove(playerCompanionSocket.position, movementToAttackDuration).SetEase(attackEase).OnComplete(delegate {
        //mesh.transform.DOScale(Vector3.zero, movementToAttackDuration).SetEase(attackEase).OnComplete(delegate {

            if (onPlacementCompleted != null)
            {
                Debug.Log("Placement completed");
                onPlacementCompleted();
            }

            mesh.SetActive(false);
            meshTrail.enabled = true;
        });
    }

    private IEnumerator ReturnFromAttack()
    {
        isReturningFromAttack = true;
        yield return new WaitForSeconds(attackToMovementDelay);
        
        transform.position = playerCompanionPlacement.position;
        mesh.transform.position = playerCompanionSocket.position;
        mesh.SetActive(true);
        meshTrail.enabled = false;
        mesh.transform.DOScale(Vector3.one, attackToMovementDuration).SetEase(attackEase);
        mesh.transform.DOMove(transform.position, attackToMovementDuration).SetEase(attackEase).OnComplete(delegate {
            isAttacking = false; 
            isReturningFromAttack = false; 
            mesh.SetActive(true);
            playerCombatController.ResetAttackState();
        });
        //mesh.transform.DOScale(Vector3.zero, attackToMovementDuration).SetEase(attackEase).OnComplete(delegate {isAttacking = false;});
    }

    private IEnumerator ActivateGlideMovement()
    {
        yield return null;
        isOnGlide = true;
        mesh.transform.DOScale(Vector3.zero, glideMovementDuration).SetEase(attackEase);
        mesh.transform.DOMove(playerCompanionSocket.position, glideMovementDuration).SetEase(attackEase).OnComplete(delegate {
        //mesh.transform.DOScale(Vector3.zero, movementToAttackDuration).SetEase(attackEase).OnComplete(delegate {
            mesh.SetActive(false);
            meshTrail.enabled = true;
        });
    }

    private IEnumerator DeactivateGlideMovement()
    {
        yield return null;
        isOnGlide = false;
        transform.position = playerCompanionPlacement.position;
        mesh.transform.position = playerCompanionSocket.position;
        mesh.SetActive(true);
        mesh.transform.DOScale(Vector3.one, 0f).SetEase(attackEase);
        mesh.transform.DOMove(transform.position, glideMovementDuration).SetEase(attackEase).OnComplete(delegate { mesh.SetActive(true);});
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
