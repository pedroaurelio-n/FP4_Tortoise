using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CompanionMain : MonoBehaviour
{
    [HideInInspector] public TrailRenderer companionSocketTrail;

    [Header("Companion Components")]
    public CompanionNavMesh companionNavMesh;
    public CompanionAlertMessage companionAlertMessage;
    public CompanionGlideController companionGlideController;
    public CompanionAttackController companionAttackController;
    public GameObject companionEntityMesh;

    [Header("Player References")]
    public PlayerMovement playerMovement;
    [SerializeField] private PlayerGroundCheck playerGroundCheck;
    public Transform companionDesiredPlacement;
    public Transform playerCompanionSocket;
    [SerializeField] private Transform lookAt;

    [Header("Movement/Rotation Configs")]
    [SerializeField] private float smoothTime;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxSpeed;

    private Rigidbody rb;
    private Vector3 _velocityVector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        companionSocketTrail = playerCompanionSocket.gameObject.GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        if (transform.parent.TryGetComponent(out PlayerMain player))
            transform.parent = transform.parent.parent;

        companionSocketTrail.enabled = false;
    }

    private void Update()
    {
        companionGlideController.HandleGliding();

        if (Vector3.Distance(companionEntityMesh.transform.position, transform.position) > 100)
            companionEntityMesh.transform.position = transform.position;
    }

    private void FixedUpdate()
    {
        if (companionAttackController.isAttacking)
            return;

        if (playerGroundCheck.isGrounded && !playerGroundCheck.isOnMovingPlatform)
        {
            if (!companionNavMesh.gameObject.activeInHierarchy)
            {
                companionNavMesh.transform.position = companionDesiredPlacement.position;
                companionNavMesh.gameObject.SetActive(true);
            }
            var navMeshPosition = companionNavMesh.transform.position;
            var desiredPosition = new Vector3(navMeshPosition.x, companionDesiredPlacement.position.y, navMeshPosition.z);

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocityVector, smoothTime, maxSpeed, Time.smoothDeltaTime);

        }

        else
        {
            if (companionNavMesh.gameObject.activeInHierarchy)
            {
                companionNavMesh.gameObject.SetActive(false);
            }
            
            var desiredPosition = companionDesiredPlacement.position;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocityVector, smoothTime, maxSpeed, Time.smoothDeltaTime);
            //rb.MovePosition(Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocityVector, smoothTime, maxSpeed, Time.smoothDeltaTime));

        }

        var lookDirection = Quaternion.LookRotation(lookAt.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed);
    }
}
