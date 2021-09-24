using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain : MonoBehaviour, IDamageable
{
    public void TakeDamage()
    {
        Debug.Log("Enemy Damaged");
    }
}
