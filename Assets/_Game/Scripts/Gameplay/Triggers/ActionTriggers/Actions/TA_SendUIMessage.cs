using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TA_SendUIMessage : TriggerAction
{
    public delegate void SendTextMessage(string message);
    public static event SendTextMessage onMessageSent;

    [SerializeField] private string message;

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
        if (onMessageSent != null)
            onMessageSent(message);
    }
}
