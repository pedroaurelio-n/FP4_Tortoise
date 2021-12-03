using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TA_Move : TriggerAction
{
    [SerializeField] private Vector3 newPositionFromOrigin;
    [SerializeField] private float actionDuration;
    [SerializeField] private Ease ease;
    
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
        transform.parent.DOMove(transform.position+newPositionFromOrigin, actionDuration).SetEase(ease).OnComplete(delegate {isActionOnProgress = false;});
    }
}
