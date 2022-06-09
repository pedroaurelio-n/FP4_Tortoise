using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldController : MonoBehaviour
{
    [SerializeField] private PlayerMain playerMain;
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private GameObject shildDamageParticle;
    [SerializeField] private float shieldMaxHealth;
    [SerializeField] private float shieldRechargeRate;
    [SerializeField] private float shieldRechargeDelay;
    [SerializeField] private float deflectActivationTime;
    [SerializeField] private float deflectCooldown;
    [SerializeField] private float deflectSpeedMultiplier;

    private float _maxHealth;
    private float _health;

    public float Health { get { return _health; } }
    public float MaxHealth { get { return _maxHealth; } }

    private Coroutine _rechargeCoroutine;

    private bool _isShieldActive;
    private bool _isShieldBroken;
    private bool _willDeflect;
    private bool _isOnDeflectCooldown;

    private float _timeElapsed;

    private Color _normalColor;

    private SphereCollider _collider;
    private MeshRenderer _shieldMesh;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _shieldMesh = shieldObject.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _maxHealth = shieldMaxHealth;
        _health = _maxHealth;

        _normalColor = _shieldMesh.material.GetColor("Color_63659391");
        
        SetShieldActive(false);
    }

    public bool IsShieldBroken()
    {
        return _isShieldBroken;
    }
    
    public bool IsOnDeflectCooldown()
    {
        return _isOnDeflectCooldown;
    }

    public void HandleShield()
    {
        if (playerMain.PlayerInputManager.protectInput && !_isShieldBroken)
        {
            ActivateShield();
        }

        else if (!playerMain.PlayerInputManager.protectInput && _isShieldActive)
        {
            SetShieldActive(false);
            _willDeflect = false;

            _timeElapsed = 0;
        }
    }

    private void ActivateShield()
    {
        if (!_isShieldActive)
            SetShieldActive(true);
        
        if (_timeElapsed < deflectActivationTime)
        {
            if (_isOnDeflectCooldown)
            {
                _timeElapsed = deflectActivationTime;
                return;
            }

            _timeElapsed += Time.smoothDeltaTime;
            _willDeflect = true;

            _shieldMesh.material.SetColor("Color_63659391", _normalColor);
        }
        else
        {
            _willDeflect = false;
            _shieldMesh.material.SetColor("Color_63659391", Color.cyan);
        }
    }

    private void SetShieldActive(bool value)
    {
        _isShieldActive = value;
        shieldObject.SetActive(value);
        _collider.enabled = value;
    }

    private IEnumerator ShieldDeflectCooldown()
    {
        _isOnDeflectCooldown = true;
        yield return new WaitForSeconds(deflectCooldown);
        _isOnDeflectCooldown = false;
    }

    private void ShieldDamage(EnemyBullet bullet)
    {
        if (bullet.WasDeflected())
            return;
        
        if (bullet.IsDeflectable() && _willDeflect)
        {
            StartCoroutine(ShieldDeflectCooldown());
            return;
        }

        _health -= bullet.GetDamage();
        Instantiate(shildDamageParticle, transform.position + new Vector3(0f, 0.85f, 0f), Quaternion.identity);

        if (_health <= 0)
        {
            _health = 0;
            _isShieldBroken = true;
            _timeElapsed = 0;
            SetShieldActive(false);
            _willDeflect = false;
        }

        if (_rechargeCoroutine != null)
        {
            StopCoroutine(_rechargeCoroutine);
            _rechargeCoroutine = null;
        }

        _rechargeCoroutine = StartCoroutine(RechargeShield());
    }

    private IEnumerator RechargeShield()
    {
        yield return new WaitForSeconds(shieldRechargeDelay);

        while (_health < _maxHealth)
        {
            _health += Time.deltaTime * shieldRechargeRate;
            yield return null;
        }

        _health = _maxHealth;
        _isShieldBroken = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isShieldActive && other.TryGetComponent<EnemyBullet>(out EnemyBullet bullet))
        {
            ShieldDamage(bullet);
            bullet.ShieldHit(_willDeflect, deflectSpeedMultiplier);
        }
    }
}
