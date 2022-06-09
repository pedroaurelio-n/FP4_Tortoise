using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyWaveTrigger wave;
    [SerializeField] private int maxHealth;
    [SerializeField] private int attackDamage;
    [SerializeField] private bool isActiveOnStart;
    [SerializeField] private GameObject deathParticle;

    protected int _maxHealth;
    protected int _attackDamage;
    protected int _currentHealth;
    protected bool canDamagePlayer;
    protected bool isAlive;

    protected void InitializeValues()
    {
        _maxHealth = maxHealth;
        _attackDamage = attackDamage;
        _currentHealth = _maxHealth;
        isAlive = isActiveOnStart;
    }

    public int GetAttackDamage() { return _attackDamage; }
    public bool CanDamagePlayer() { return canDamagePlayer; }
    public bool IsAlive() { return isAlive; }

    protected abstract void Move();
    public virtual void TakeDamage(Vector3 hitNormal, bool byBullet)
    {
        if (isAlive)
        {
            _currentHealth--;

            if (_currentHealth <= 0)
                Die();

            else
            {
                DamageFeedback(hitNormal);
            }
        }
    }
    protected abstract void DamageFeedback(Vector3 hitNormal);
    protected virtual void Die()
    {
        isAlive = false;

        if (wave != null)
            wave.RemoveEnemy(this);

        var temp = Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

public enum EnemyMovementType
{
    WANDER,
    FOLLOW,
    IDLE
}