using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private GameObject normalObject;
    [SerializeField] private List<Rigidbody> brokenObjects;
    [SerializeField] private float explosionForce;
    [SerializeField] private GameObject itemDrop;
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private GameObject dropParticles;

    private int _health;

    private void Start()
    {
        _health = maxHealth;
        normalObject.SetActive(true);

        if (brokenObjects.Count > 0)
        {
            foreach (Rigidbody rb in brokenObjects)
            {
                rb.gameObject.SetActive(false);
            }
        }
    }

    public void TakeDamage()
    {
        _health--;

        if (_health <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        normalObject.SetActive(false);
        
        if (hitParticles != null)
        {
            var particle = Instantiate(hitParticles, transform.position, Quaternion.identity);
        }

        if (brokenObjects.Count > 0)
        {
            foreach (Rigidbody rb in brokenObjects)
            {
                rb.gameObject.SetActive(true);
                rb.AddExplosionForce(explosionForce,transform.position, 1);
            }
        }

        if (itemDrop != null && dropParticles != null)
        {
            var drop = Instantiate(itemDrop, transform.position, Quaternion.identity);
            var particle = Instantiate(dropParticles, transform.position, Quaternion.identity);
            GetComponent<Collider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerHitbox>(out PlayerHitbox player))
        {
            TakeDamage();
        }
    }
}
