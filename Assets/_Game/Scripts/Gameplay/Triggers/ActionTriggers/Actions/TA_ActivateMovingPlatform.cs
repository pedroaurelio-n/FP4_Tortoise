using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TA_ActivateMovingPlatform : TriggerAction
{
    [SerializeField] private MovingPlatform platform;
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
        platform.canStart = true;
        platform.CheckStart();
        isActionOnProgress = false;
    }
}
