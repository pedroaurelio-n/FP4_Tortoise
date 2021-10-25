using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out IDamageable target))
        {
            Vector3 hitNormal = other.ClosestPoint(transform.position) - transform.position;

            target.TakeDamage(hitNormal);
        }
    }
}
