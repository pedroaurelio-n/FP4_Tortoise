using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullets : MonoBehaviour
{
    [SerializeField] private List<EnemyBullet> bulletPrefabs;
    [SerializeField] private bool startShooting;
    [SerializeField] private bool shootAutomatically;
    [SerializeField] private Enemy enemyParent;
    [SerializeField] private int bulletDamage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootInterval;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 originOffset;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private Transform _Dynamic;

    private int _bulletIndex;
    private bool _willShoot;

    private Coroutine _shootCoroutine;

    private void Start()
    {
        _bulletIndex = 0;
        _willShoot = startShooting;

        if (_willShoot)
            StartShooting();
    }

    public float GetShootInterval()
    {
        return shootInterval;
    }

    public void StartShooting()
    {
        _willShoot = true;

        if (shootAutomatically)
            _shootCoroutine = StartCoroutine(ShootLoop());

        else
            Shoot();
    }

    public void StopShooting()
    {
        _willShoot = false;

        if (shootAutomatically)
        {
            if (_shootCoroutine != null)
            {
                StopCoroutine(_shootCoroutine);
                _shootCoroutine = null;
            }
        }
    }

    public void ChangeBulletIndex()
    {
        _bulletIndex++;

        if (_bulletIndex >= bulletPrefabs.Count)
            _bulletIndex = 0;
    }

    public void ChangeBulletIndex(int index)
    {
        _bulletIndex = index;

        if (_bulletIndex >= bulletPrefabs.Count)
            _bulletIndex = 0;
    }

    private IEnumerator ShootLoop()
    {
        var interval = new WaitForSeconds(shootInterval);

        while (_willShoot)
        {
            var canShoot = enemyParent != null ? enemyParent.IsAlive() : true;

            if (canShoot)
                Shoot();
                
            yield return interval;
        }
    }

    private void Shoot()
    {
        var offsetOriginPos = transform.TransformVector(originOffset);
        var offsetTargetPos = transform.InverseTransformDirection(targetOffset);

        var direction = (target.position + offsetTargetPos) - transform.position;

        var bullet = Instantiate(bulletPrefabs[_bulletIndex], transform.position + offsetOriginPos, Quaternion.identity, _Dynamic);
        bullet.Initialize(gameObject, transform.position + offsetOriginPos, bulletDamage, direction.normalized, bulletSpeed, _Dynamic, target);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + originOffset, 0.5f);
    }
}
