using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, ICollectable
{
    public virtual void Collect()
    {
        //Debug.Log("Item Collected");
    }
}
