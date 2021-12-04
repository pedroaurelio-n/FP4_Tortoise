using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TA_RemoveStar : TriggerAction
{
    public delegate void StarRemove(int value);
    public static event StarRemove onStarRemove;

    [SerializeField] private int starsNumberToRemove;
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
        return StarManager.GetStarCount() >= starsNumberToRemove;
    }

    protected override void ActivateAction()
    {
        if (onStarRemove != null)
            onStarRemove(-starsNumberToRemove);
    }
}
