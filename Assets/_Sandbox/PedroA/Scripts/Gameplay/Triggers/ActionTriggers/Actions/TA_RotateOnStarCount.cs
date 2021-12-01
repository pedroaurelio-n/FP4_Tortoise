using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TA_RotateOnStarCount : TriggerAction
{
    [SerializeField] private Vector3 newRotationFromOrigin;
    [SerializeField] private float actionDuration;
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
        transform.parent.DORotate(transform.eulerAngles + newRotationFromOrigin, actionDuration).OnComplete(delegate {isActionOnProgress = false;});
    }
}
