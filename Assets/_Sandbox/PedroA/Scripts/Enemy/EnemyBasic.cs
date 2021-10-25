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
    [SerializeField] private Transform target;
    [SerializeField] private Transform point;
    [SerializeField] private BoxCollider wanderArea;

    public EnemyMovementType MovementType;

    private Coroutine wanderMovement;
    private Coroutine followMovement;
    private bool isAlive;
    private Color currentColor;

    private NavMeshAgent navMeshAgent;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        isAlive = true;

        point.parent = null;

        CurrentHealth = MaxHealth;

        Move();
    }

    private void Update()
    {
        if (MovementType == EnemyMovementType.WANDER && IsCloseToPlayer())
        {
            MovementType = EnemyMovementType.FOLLOW;
            Debug.Log("Start Follow");
            Move();
        }

        //Vector3.Distance(transform.position, wanderArea.center) > maximumFollowDistance
        if (MovementType == EnemyMovementType.FOLLOW && !IsCloseToPlayer())
        {
            MovementType = EnemyMovementType.WANDER;
            Debug.Log("Start Wander");
            Move();
        }
    }

    private bool IsCloseToPlayer()
    {
        return Vector3.Distance(transform.position, target.position) < minimumFollowDistance;
    }

    public override void Move()
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
            var currentDestination = GetRandomPointInsideCollider(wanderArea);
            point.position = currentDestination;

            navMeshAgent.isStopped = false;

            while (Vector3.Distance(transform.position, currentDestination) > minimumWanderDistance)
            {
                navMeshAgent.SetDestination(currentDestination);
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

    private IEnumerator DamageFeedback()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        yield return new WaitForSeconds(delayBetweenPoints);

        navMeshAgent.isStopped = false;
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

    public override void TakeDamage()
    {
        CurrentHealth--;

        if (CurrentHealth <= 0)
            Die();

        else
        {
            StartCoroutine(DamageFeedback());
            meshRenderer.material.DOColor(Color.white, 0.1f).OnComplete(delegate {
                meshRenderer.material.DOColor(currentColor, delayBetweenPoints);
            });
        }

    }

    public override void Die()
    {
        isAlive = false;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;

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
