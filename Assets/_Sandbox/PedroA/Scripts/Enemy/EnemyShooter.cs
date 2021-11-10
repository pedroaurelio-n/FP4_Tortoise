using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyShooter : Enemy
{
    [Header("Object References")]
    [SerializeField] private Transform shootLocation;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform target;
    [SerializeField] private Transform _Dynamic;

    [Header("Shoot Configs")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float startShootingDelay;
    [SerializeField] private float intervalBetweenShots;
    [SerializeField] private float turnSpeed;

    [Header("Move Configs")]
    [SerializeField] private float minimumRange;
    [SerializeField] private float moveY;
    [SerializeField] private float moveDuration;

    private Coroutine shootCoroutine;
    private bool isActive;
    private bool isDead;
    private float _yStartPosition;

    private void Awake()
    {
        InitializeValues();
    }

    private void Start()
    {
        _yStartPosition = transform.position.y;

        isActive = false;
        isDead = false;

        if (minimumRange == 0)
        {
            isActive = true;
            Activate();            
        }
    }

    private void Update()
    {
        if (isDead)
            return;

        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.position) < minimumRange && !isActive)
            {
                isActive = true;
                Activate();
            }

            else if (Vector3.Distance(transform.position, target.position) > minimumRange && isActive)
            {
                isActive = false;
                Deactivate();
            }

            LookAtTarget();
        }
    }

    private void LookAtTarget()
    {
        var targetRelativePosition = Quaternion.LookRotation(target.position - transform.position);
        var newRotation = Quaternion.RotateTowards(transform.rotation, targetRelativePosition, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(transform.rotation.x, newRotation.eulerAngles.y, transform.rotation.z);
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(startShootingDelay);
        
        if (isDead)
            yield break;

        while (isActive)
        {
            var temp = Instantiate(bulletPrefab, shootLocation.position, Quaternion.identity, _Dynamic);
            temp.GetComponent<EnemyShooterBullet>().StartMovement(transform.forward, bulletSpeed);
            
            yield return new WaitForSeconds(intervalBetweenShots);
        }
    }

    private void Activate()
    {
        transform.DOMoveY(_yStartPosition + moveY, moveDuration).OnComplete(delegate {
            shootCoroutine = StartCoroutine(Shoot());
        });
    }

    private void Deactivate()
    {
        transform.DOMoveY(_yStartPosition, moveDuration).OnComplete(delegate {
            StopCoroutine(shootCoroutine);
        });
    }

    protected override void Move() {}

    protected override void DamageFeedback(Vector3 hitNormal) {}

    protected override void Die()
    {
        base.Die();
        
        Deactivate();
        GetComponent<MeshRenderer>().material.color = Color.red;

        isDead = true;
        isActive = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minimumRange);
    }
}
