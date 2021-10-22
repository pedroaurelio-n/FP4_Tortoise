using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerMain playerMain;
    [SerializeField] private int maxHealth;

    private int _currentHealth;

    [Header("KnockBack Configs")]
    [SerializeField] private float knockbackHorizontal;
    [SerializeField] private float knockbackVertical;
    [SerializeField] private float knockbackTime;

    private void DecreaseHealth(Vector3 direction)
    {
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
