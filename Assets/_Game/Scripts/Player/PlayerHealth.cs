using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public delegate void HealthChange(float newHealth, float maxHealth, float value);
    public static event HealthChange onHealthChange;

    [SerializeField] private float maxHealth;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;

        //if (onHealthChange != null)
        //{
        //    onHealthChange(_currentHealth, maxHealth, 0);
        //}
    }

    private void Start()
    {
        if (onHealthChange != null)
        {
            onHealthChange(_currentHealth, maxHealth, 0);
        }
    }

    private void DecreaseHealth(int damage)
    {
        _currentHealth += damage;
        if (onHealthChange != null)
            onHealthChange(_currentHealth, maxHealth, damage);
    }

    private void OnEnable()
    {
        PlayerCombatController.onPlayerDamageHit += DecreaseHealth;
    }

    private void OnDisable()
    {
        PlayerCombatController.onPlayerDamageHit -= DecreaseHealth;
    }
}
