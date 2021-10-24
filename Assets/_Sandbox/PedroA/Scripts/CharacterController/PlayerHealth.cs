using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public delegate void HealthChange(float newHealth, float maxHealth, float value);
    public static event HealthChange onHealthChange;

    [SerializeField] private PlayerMain playerMain;
    [SerializeField] private float maxHealth;

    private float _currentHealth;

    [Header("KnockBack Configs")]
    [SerializeField] private float knockbackHorizontal;
    [SerializeField] private float knockbackVertical;
    [SerializeField] private float knockbackTime;

    private void Awake()
    {
        _currentHealth = maxHealth;

        if (onHealthChange != null)
        {
            onHealthChange(_currentHealth, maxHealth, 0);
        }
    }

    private void DecreaseHealth(Vector3 direction, int damage)
    {
        _currentHealth += damage;
        if (onHealthChange != null)
            onHealthChange(_currentHealth, maxHealth, damage);

        playerMain.PlayerMovement.TriggerKnockback(direction, knockbackHorizontal, knockbackVertical, knockbackTime);
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
