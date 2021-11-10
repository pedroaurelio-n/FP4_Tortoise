using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TA_ActivateObject : TriggerAction
{
    [SerializeField] private GameObject Object;

    public override bool TryToActivateAction()
    {
        if (CanActivateAction())
            ActivateAction();
        else
            Debug.Log("Couldn't perform action");

        return CanActivateAction();
    }

    protected override bool CanActivateAction()
    {
        return true;
    }

    protected override void ActivateAction()
    {
        isActionOnProgress = true;
        Object.SetActive(true);
        isActionOnProgress = false;
    }
}
