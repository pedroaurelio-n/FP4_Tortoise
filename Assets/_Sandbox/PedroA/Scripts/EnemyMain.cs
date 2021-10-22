using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMain : MonoBehaviour, IDamageable
{
    [SerializeField] private Material dead;

    public void TakeDamage()
    {
        Debug.Log("Enemy Damaged");
        
        gameObject.GetComponent<MeshRenderer>().material = dead;

        transform.DOScale(transform.localScale * 1.2f, 0.1f).OnComplete( delegate {
            transform.DOScale(Vector3.zero, 0.7f);
        });
    }
}
