using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Star : Item
{
    public delegate void StarCollected(int value);
    public static event StarCollected onStarCollected;

    [SerializeField] private int value;
    [SerializeField] private float scaleToZeroTime;
    [SerializeField] private GameObject collectParticles;
    [SerializeField] private Transform _Dynamic;

    public override void Collect()
    {
        base.Collect();
        Debug.Log("Star Collected");

        if (onStarCollected != null)
            onStarCollected(value);

        var temp = Instantiate(collectParticles, transform.position, Quaternion.identity, _Dynamic);
        float particleDuration = temp.GetComponent<ParticleSystem>().main.duration;

        transform.DOScale(Vector3.zero, scaleToZeroTime).OnComplete(delegate { Destroy(gameObject); });

        Destroy(temp, particleDuration);
    }
}
