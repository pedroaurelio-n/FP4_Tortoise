using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooterBullet : Enemy
{
    [SerializeField] private float triggerInterval;
    [SerializeField] private Rigidbody rb;
    private bool willDestroyOnTrigger;

    private void Awake()
    {
        InitializeValues();

        willDestroyOnTrigger = false;
        canDamagePlayer = true;
    }

    private IEnumerator DestroyOnTrigger()
    {
        yield return new WaitForSeconds(triggerInterval);
        willDestroyOnTrigger = true;
    }

    public void StartMovement(Vector3 direction, float speed)
    {
        StartCoroutine(DestroyOnTrigger());
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Die();
    }

    protected override void Move(){}

    protected override void DamageFeedback(Vector3 hitNormal){}

    protected override void Die()
    {
        if (willDestroyOnTrigger)
            Destroy(gameObject);
    }
}
