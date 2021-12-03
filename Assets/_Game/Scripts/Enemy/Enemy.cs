using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyWaveTrigger wave;
    [SerializeField] private int maxHealth;
    [SerializeField] private int attackDamage;

    protected int _maxHealth;
    protected int _attackDamage;
    protected int _currentHealth;
    protected bool canDamagePlayer;

    protected void InitializeValues()
    {
        _maxHealth = maxHealth;
        _attackDamage = attackDamage;
        _currentHealth = _maxHealth;
    }

    public int GetAttackDamage() { return _attackDamage; }
    public bool CanDamagePlayer() { return canDamagePlayer; }

    protected abstract void Move();
    public virtual void TakeDamage(Vector3 hitNormal)
    {
        _currentHealth--;

        if (_currentHealth <= 0)
            Die();

        else
        {
            DamageFeedback(hitNormal);
        }
    }
    protected abstract void DamageFeedback(Vector3 hitNormal);
    protected virtual void Die()
    {
        if (wave != null)
            wave.RemoveEnemy(this);
    }
}

public enum EnemyMovementType
{
    WANDER,
    FOLLOW,
    IDLE
}