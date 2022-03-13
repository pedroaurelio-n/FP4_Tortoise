using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TA_InvokeEvent : TriggerAction
{
    [SerializeField] private List<UnityEvent> events;

    protected override void ActivateAction()
    {
        for (int i = 0; i < events.Count; i++)
        {
            events[0]?.Invoke();
        }
    }
}
