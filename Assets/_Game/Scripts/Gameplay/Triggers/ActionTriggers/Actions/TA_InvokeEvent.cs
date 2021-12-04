using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TA_InvokeEvent : TriggerAction
{
    [SerializeField] private List<UnityEvent> events;
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
        for (int i = 0; i < events.Count; i++)
        {
            events[0]?.Invoke();
        }
    }
}
