using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTrigger : Trigger
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractController player))
        {
            StartCoroutine(base.CheckAction(reference));
        }
    }
}