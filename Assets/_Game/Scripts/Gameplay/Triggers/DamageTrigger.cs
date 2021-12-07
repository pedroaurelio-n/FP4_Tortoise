using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : Enemy
{
    private void Start()
    {
        InitializeValues();
        canDamagePlayer = true;
    }
    
    protected override void DamageFeedback(Vector3 hitNormal)
    {
        throw new System.NotImplementedException();
    }

    protected override void Move()
    {
        throw new System.NotImplementedException();
    }
}
