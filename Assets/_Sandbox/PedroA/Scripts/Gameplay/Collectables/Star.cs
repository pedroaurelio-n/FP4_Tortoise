using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Item
{
    public delegate void StarCollected(int value);
    public static event StarCollected onStarCollected;

    [SerializeField] private int value;

    public override void Collect()
    {
        base.Collect();
        Debug.Log("Star Collected");

        if (onStarCollected != null)
            onStarCollected(value);
    }
}
