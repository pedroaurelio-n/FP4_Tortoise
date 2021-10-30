using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyShooter : Enemy
{
    [SerializeField] private Transform shootLocation;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float intervalBetweenShots;
    [SerializeField] private bool hasMinimumRange;
    [SerializeField] private float minimumRange;
    [SerializeField] private Transform target;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Transform _Dynamic;

    private void Awake()
    {
        InitializeValues();
    }

    private void Start()
    {
        StartCoroutine(Shoot());
    }

    private void Update()
    {
        if (target != null)
            LookAtTarget();
    }

    private void LookAtTarget()
    {
        var targetRelativePosition = Quaternion.LookRotation(target.position - transform.position);
        var newRotation = Quaternion.RotateTowards(transform.rotation, targetRelativePosition, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(transform.rotation.x, newRotation.eulerAngles.y, transform.rotation.z);
    }

    private IEnumerator Shoot()
    {
        while (_currentHealth > 0)
        {
            var temp = Instantiate(bulletPrefab, shootLocation.position, Quaternion.identity, _Dynamic);
            temp.GetComponent<EnemyShooterBullet>().StartMovement(transform.forward, bulletSpeed);
            
            yield return new WaitForSeconds(intervalBetweenShots);
        }
    }

    protected override void Move() {}

    protected override void DamageFeedback(Vector3 hitNormal)
    {
        throw new System.NotImplementedException();
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
}
