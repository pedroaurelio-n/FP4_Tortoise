using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    public int attackDamage;
    public int MaxHealth;

    [HideInInspector] public int CurrentHealth;

    public abstract void Move();
    public abstract void TakeDamage();
    public abstract void Die();
}

public enum EnemyMovementType
{
    WANDER,
    FOLLOW,
    IDLE
}