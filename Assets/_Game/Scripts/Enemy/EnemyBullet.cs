using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private bool isDeflectable;
    [SerializeField] private bool willTrackTarget;
    [SerializeField] private float trackingForce;
    [SerializeField] private float trackingDuration;
    [SerializeField] private float maxSpeed;

    private bool _wasDeflected;

    private Transform _target;
    private int _bulletDamage;
    private float _speed;
    private GameObject _originObject;
    private Rigidbody _rigidbody;

    private float _trackingTime;

    private Coroutine _killCoroutine;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _trackingTime = trackingDuration;
    }

    public void Initialize(GameObject originObject, Vector3 position, int damage, Vector3 direction, float speed, Transform parent, Transform? target)
    {
        _target = target;

        _wasDeflected = false;
        transform.position = position;
        _bulletDamage = damage;
        _speed = speed;

        _rigidbody.velocity = direction * speed;
        _originObject = originObject;

        if (parent)
            transform.parent = parent;
        else
            transform.parent = null;

        _killCoroutine = StartCoroutine(KillCoroutine());
    }

    private void FixedUpdate()
    {
        if (_target == null || _trackingTime < 0 || _wasDeflected)
            return;

        var direction = _target.position - transform.position;
        var flatDirection = new Vector3(direction.x, 0f, direction.z);
        _rigidbody.AddForce(flatDirection.normalized * trackingForce * _trackingTime, ForceMode.Acceleration);

        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);

        _trackingTime -= Time.deltaTime;
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public void ShieldHit(bool deflect, float deflectMultiplier)
    {
        if (deflect && isDeflectable)
        {
            if (!_wasDeflected)
            {
                StopCoroutine(_killCoroutine);

                var deflectDirection = _originObject.transform.parent.position - transform.position;
                _rigidbody.velocity = deflectDirection.normalized * _speed * deflectMultiplier;

                var playerBulletsLayer = LayerMask.NameToLayer("PlayerBullets");
                gameObject.layer = playerBulletsLayer;

                _wasDeflected = true;

                _killCoroutine = StartCoroutine(KillCoroutine());
            }
        }
        else
            DestroyBullet();
    }

    public int GetDamage()
    {
        return _bulletDamage;
    }

    public bool WasDeflected()
    {
        return _wasDeflected;
    }

    public bool IsDeflectable()
    {
        return isDeflectable;
    }

    protected IEnumerator KillCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        DestroyBullet();
    }

    private void OnTriggerEnter(Collider other)
    {
        var isPlayer = other.GetComponent<PlayerCombatController>();
        var isShield = other.GetComponent<PlayerShieldController>();
        var isEnemyParent = other.transform.parent.TryGetComponent<Enemy>(out Enemy enemyParent);

        if (isPlayer == null && isShield == null && !isEnemyParent)
        {
            DestroyBullet();
        }

        if (isEnemyParent && _wasDeflected)
        {
            var hitNormal = other.ClosestPoint(transform.position) - transform.position;

            enemyParent.TakeDamage(-hitNormal, true);
            DestroyBullet();
        }
    }
}
