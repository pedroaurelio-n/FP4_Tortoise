using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int attackDamage;

    protected int _maxHealth;
    protected int _attackDamage;
    protected int _currentHealth;

    protected void InitializeValues()
    {
        _maxHealth = maxHealth;
        _attackDamage = attackDamage;
        _currentHealth = _maxHealth;
    }

    public int GetAttackDamage() { return _attackDamage; }

    protected abstract void Move();
    public abstract void TakeDamage(Vector3 hitNormal);
    protected abstract void Die();
}

public enum EnemyMovementType
{
    WANDER,
    FOLLOW,
    IDLE
}