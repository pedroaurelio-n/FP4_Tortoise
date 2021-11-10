using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TA_MoveOnStarCount : TriggerAction
{
    [SerializeField] private Vector3 newPositionFromOrigin;
    [SerializeField] private float actionDuration;
    [SerializeField] private Ease ease;
    [SerializeField] private int minimunStarsRequired;
    [SerializeField] private string failMessage;
    
    public override bool TryToActivateAction()
    {
        if (CanActivateAction())
            ActivateAction();
        else
        {
            SendFailEvent(failMessage);
        }

        return CanActivateAction();
    }

    protected override bool CanActivateAction()
    {
        return StarManager.GetStarCount() >= minimunStarsRequired;
    }

    protected override void ActivateAction()
    {
        isActionOnProgress = true;
        transform.parent.DOMove(transform.position+newPositionFromOrigin, actionDuration).SetEase(ease).OnComplete(delegate {isActionOnProgress = false;});
    }
}
