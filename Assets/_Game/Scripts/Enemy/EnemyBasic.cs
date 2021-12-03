using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class EnemyBasic : Enemy
{
    [SerializeField] private float delayBetweenPoints;
    [SerializeField] private float minimumWanderDistance;
    [SerializeField] private float minimumFollowDistance;
    [SerializeField] private float maximumFollowDistance;
    [SerializeField] private float followSpeedMultiplier;
    [SerializeField] private float delayOnDamage;
    [SerializeField] private float knockbackforce;
    [SerializeField] private Transform target;
    [SerializeField] private Transform point;
    [SerializeField] private Transform _Dynamic;
    [SerializeField] private BoxCollider wanderArea;

    public EnemyMovementType MovementType;

    private Coroutine wanderMovement;
    private Coroutine followMovement;
    private Coroutine recoverFromDamageFeedback;
    private bool isAlive;
    private Color currentColor;

    private Rigidbody rigidBody;
    private NavMeshAgent navMeshAgent;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        InitializeValues();

        rigidBody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        isAlive = true;
        canDamagePlayer = true;

        point.parent = _Dynamic;

        Move();
    }

    private void Update()
    {
        if (MovementType == EnemyMovementType.WANDER && IsCloseToPlayer())
        {
            MovementType = EnemyMovementType.FOLLOW;
            Move();
        }

        //Vector3.Distance(transform.position, wanderArea.center) > maximumFollowDistance
        if (MovementType == EnemyMovementType.FOLLOW && !IsCloseToPlayer())
        {
            MovementType = EnemyMovementType.WANDER;
            Move();
        }
    }

    private bool IsCloseToPlayer()
    {
        return Vector3.Distance(transform.position, target.position) < minimumFollowDistance;
    }

    protected override void Move()
    {
        switch (MovementType)
        {
            case EnemyMovementType.WANDER:                
                wanderMovement = StartCoroutine(MoveWander());
            break;

            case EnemyMovementType.FOLLOW:
                followMovement = StartCoroutine(MoveFollow());
            break;

            case EnemyMovementType.IDLE: return;
        }
    }

    private IEnumerator MoveWander()
    {
        if (followMovement != null)
            StopCoroutine(followMovement);

        currentColor = Color.green;
        meshRenderer.material.DOColor(currentColor, 0f);

        if (!isAlive)
            yield break;

        while (MovementType == EnemyMovementType.WANDER)
        {
            var currentWaypoint = GetRandomPointInsideCollider(wanderArea);
            point.position = currentWaypoint;

            /*var colliders = Physics.OverlapSphere(point.position, 0f, groundLayer);

            while (colliders.Length > 0)
            {
                Debug.Log("Waypoint colliding");
                currentWaypoint = GetRandomPointInsideCollider(wanderArea);
                point.position = currentWaypoint;
                yield return null;
            }*/


            navMeshAgent.isStopped = false;

            while (Vector3.Distance(transform.position, currentWaypoint) > minimumWanderDistance)
            {
                navMeshAgent.SetDestination(currentWaypoint);
                yield return null;
            }

            yield return new WaitForSeconds(delayBetweenPoints);

            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            yield return null;
        }
        yield return null;
    }

    private IEnumerator MoveFollow()
    {
        if (wanderMovement != null)
            StopCoroutine(wanderMovement);

        currentColor = Color.yellow;
        meshRenderer.material.DOColor(currentColor, 0f);

        if (!isAlive)
            yield break;

        navMeshAgent.isStopped = false;

        while (MovementType == EnemyMovementType.FOLLOW)
        {
            point.position = target.position;
            navMeshAgent.SetDestination(target.position);
            yield return null;
        }
        yield return null;
    }

    private IEnumerator RecoverFromDamageFeedback()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        canDamagePlayer = false;
        yield return new WaitForSeconds(delayOnDamage);

        navMeshAgent.isStopped = false;
        rigidBody.velocity = Vector3.zero;
        canDamagePlayer = true;
    }

    private Vector3 GetRandomPointInsideCollider(BoxCollider boxCollider)
    {
        var extents = boxCollider.size * 0.5f;
        var point = new Vector3(
            Random.Range(-extents.x, extents.x),
            Random.Range(-extents.y, extents.y),
            Random.Range(-extents.z, extents.z)
        ) + boxCollider.center;

        return boxCollider.transform.TransformPoint(point);
    }

    protected override void DamageFeedback(Vector3 hitNormal)
    {
        if (recoverFromDamageFeedback != null)
            StopCoroutine(recoverFromDamageFeedback);
        
        recoverFromDamageFeedback = StartCoroutine(RecoverFromDamageFeedback());
        rigidBody.AddForce(hitNormal.normalized * knockbackforce, ForceMode.Impulse);

        Tween damageWhite = meshRenderer.material.DOColor(Color.white, delayOnDamage * 0.05f);
        Tween damageWhite2 = meshRenderer.material.DOColor(Color.white, delayOnDamage * 0.05f);
        Tween damageCurrent = meshRenderer.material.DOColor(currentColor, delayOnDamage * 0.05f);

        Sequence damageSequence = DOTween.Sequence();
        damageSequence.Append(damageWhite)
        .Append(damageCurrent)
        .Append(damageWhite2)
        .Append(meshRenderer.material.DOColor(currentColor, delayOnDamage * 0.85f));

        damageSequence.Play();
    }

    protected override void Die()
    {
        base.Die();
        
        if (recoverFromDamageFeedback != null)
            StopCoroutine(recoverFromDamageFeedback);
        
        isAlive = false;
        navMeshAgent.isStopped = true;
        canDamagePlayer = false;
        navMeshAgent.velocity = Vector3.zero;

        GetComponent<Collider>().enabled = false;

        currentColor = Color.red;
        meshRenderer.material.DOColor(currentColor, 0f);

        transform.DOScale(transform.localScale * 1.25f, 0.1f).OnComplete(delegate {
            transform.DOScale(Vector3.zero, 0.75f).OnComplete(delegate {
                Destroy(gameObject);
            });
        });
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minimumFollowDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maximumFollowDistance);
    }
}
