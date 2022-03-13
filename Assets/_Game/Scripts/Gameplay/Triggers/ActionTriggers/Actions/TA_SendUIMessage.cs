using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TA_SendUIMessage : TriggerAction
{
    public delegate void SendTextMessage(string message);
    public static event SendTextMessage onMessageSent;

    [SerializeField] private string message;

    protected override void ActivateAction()
    {
        if (onMessageSent != null)
            onMessageSent(message);
    }
}
