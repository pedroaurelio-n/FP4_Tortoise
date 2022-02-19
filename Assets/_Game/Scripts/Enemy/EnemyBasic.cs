using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEditor;

[CustomEditor(typeof(EnemyBasic))]
public class SetEnemyActiveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemyBasic enemyBasic = (EnemyBasic)target;
        if (GUILayout.Button("Toggle Enemy"))
        {
            enemyBasic.ToggleEnemy();
        }
    }
}

public class EnemyBasic : Enemy
{
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private List<AudioClip> damageClips;
    [SerializeField] private GameObject hitParticle;
    [SerializeField] private float delayBetweenPoints;
    [SerializeField] private float minimumWanderDistance;
    [SerializeField] private float minimumFollowDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float delayOnDamage;
    [SerializeField] private float knockbackforce;
    [SerializeField] private Transform target;
    [SerializeField] private Transform point;
    [SerializeField] private Transform _Dynamic;
    [SerializeField] private BoxCollider wanderArea;

    private EnemyMovementType MovementType;

    private Coroutine wanderMovement;
    private Coroutine followMovement;
    private Coroutine recoverFromDamageFeedback;
    private bool isAttacking;
    private Color currentColor;

    private Rigidbody rigidBody;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        InitializeValues();

        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        MovementType = EnemyMovementType.IDLE;

        animator.SetBool("Activated", isAlive);

        canDamagePlayer = true;

        if (_Dynamic == null)
            point.parent = null;
        else
            point.parent = _Dynamic;

        if (wanderArea == null)
            Debug.LogWarning("Wander Area is null. Enemy will be stationary.");

        if (target == null)
            Debug.LogWarning("Target is null. Enemy will not follow player.");

        Move();
    }

    public void ToggleEnemy()
    {
        if (_currentHealth > 0)
        {
            var value = !isAlive;
            isAlive = value;
            animator.SetBool("Activated", isAlive);
        }
    }

    private void Update()
    {
        if (!isAlive && MovementType != EnemyMovementType.IDLE)
        {
            ChangeMovementState(EnemyMovementType.IDLE);
        }

        if (isAlive && wanderArea != null && MovementType == EnemyMovementType.IDLE)
        {
            ChangeMovementState(EnemyMovementType.WANDER);
        }

        if (isAlive && target != null && (MovementType == EnemyMovementType.WANDER || MovementType == EnemyMovementType.IDLE) && IsCloseToPlayer())
        {
            ChangeMovementState(EnemyMovementType.FOLLOW);
        }

        if (isAlive && MovementType == EnemyMovementType.FOLLOW && !IsCloseToPlayer())
        {
            if (wanderArea != null)
                ChangeMovementState(EnemyMovementType.WANDER);
            else
                ChangeMovementState(EnemyMovementType.IDLE);
        }
    }

    private bool IsCloseToPlayer()
    {
        if (target == null)
            return false;
        else
            return Vector3.Distance(transform.position, target.position) < minimumFollowDistance;
    }

    private void ChangeMovementState(EnemyMovementType newState)
    {
        MovementType = newState;
        Move();
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

            case EnemyMovementType.IDLE: 
                animator.SetBool("Run", false); 
                navMeshAgent.isStopped = true;
            break;
        }
    }

    private IEnumerator MoveWander()
    {
        if (followMovement != null)
            StopCoroutine(followMovement);

        if (!isAlive && isAttacking)
            yield break;

        animator.SetBool("Run", false);
        animator.SetBool("Activated", true);
        //meshRenderer.material.DOColor(currentColor, 0f);

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

        if (!isAlive && isAttacking)
            yield break;

        animator.SetBool("Run", true);
        //meshRenderer.material.DOColor(currentColor, 0f);

        navMeshAgent.isStopped = false;

        while (MovementType == EnemyMovementType.FOLLOW)
        {
            point.position = target.position;
            navMeshAgent.SetDestination(target.position);
            yield return null;

            if (Vector3.Distance(target.position, transform.position) < attackDistance && !isAttacking)
            {
                TriggerAttack();
            }
        }
        yield return null;
    }

    public void TriggerAttack()
    {
        if (!isAlive)
            return;

        var lookDirection = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = lookDirection;

        isAttacking = true;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetTrigger("Attack");
    }

    public void ResetAttack()
    {
        navMeshAgent.isStopped = false;
        isAttacking = false;
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
        var randomClip = Random.Range(0, damageClips.Count);
        AudioManager.Instance.PlayAudio(damageClips[randomClip]);

        var temp = Instantiate(hitParticle, transform.position, Quaternion.identity);
        var lenght = temp.GetComponent<ParticleSystem>().main.duration;
        Destroy(temp, lenght);

        if (recoverFromDamageFeedback != null)
            StopCoroutine(recoverFromDamageFeedback);

        
        recoverFromDamageFeedback = StartCoroutine(RecoverFromDamageFeedback());
        rigidBody.AddForce(hitNormal.normalized * knockbackforce, ForceMode.Impulse);

        Tween damageWhite = meshRenderer.material.DOColor(Color.black, delayOnDamage * 0.05f);
        Tween damageWhite2 = meshRenderer.material.DOColor(Color.black, delayOnDamage * 0.05f);
        Tween damageCurrent = meshRenderer.material.DOColor(Color.white, delayOnDamage * 0.05f);

        Sequence damageSequence = DOTween.Sequence();
        damageSequence.Append(damageWhite)
        .Append(damageCurrent)
        .Append(damageWhite2)
        .Append(meshRenderer.material.DOColor(Color.white, delayOnDamage * 0.85f));

        damageSequence.Play();
    }

    protected override void Die()
    {
        base.Die();

        var randomClip = Random.Range(0, damageClips.Count);
        AudioManager.Instance.PlayAudio(damageClips[randomClip]);

        var temp = Instantiate(hitParticle, transform.position, Quaternion.identity);
        var lenght = temp.GetComponent<ParticleSystem>().main.duration;
        Destroy(temp, lenght);
        
        if (recoverFromDamageFeedback != null)
            StopCoroutine(recoverFromDamageFeedback);
        
        navMeshAgent.isStopped = true;
        canDamagePlayer = false;
        navMeshAgent.velocity = Vector3.zero;
        //navMeshAgent.enabled = false;
        //rigidBody.useGravity = true;

        //GetComponent<Collider>().enabled = false;

        currentColor = Color.black;
        animator.SetBool("Run", false);
        animator.SetBool("Activated", false);

        meshRenderer.material.DOColor(currentColor, 0f);

        //transform.DOScale(transform.localScale * 1.25f, 0.1f).OnComplete(delegate {
        //    transform.DOScale(Vector3.zero, 0.75f).OnComplete(delegate {
        //        Destroy(gameObject);
        //    });
        //});
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minimumFollowDistance);
    }
}
