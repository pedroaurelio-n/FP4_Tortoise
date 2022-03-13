using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TA_ActivateMovingPlatform : TriggerAction
{
    [SerializeField] private MovingPlatform platform;

    protected override void ActivateAction()
    {
        if (platform.GetSequential())
        {
            isActionOnProgress = true;
            platform.canStart = true;
            platform.CheckStart();
            isActionOnProgress = false;
        }
        else
        {
            platform.GoToNextPoint();
        }
    }
}
