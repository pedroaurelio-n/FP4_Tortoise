using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class TriggerAction : MonoBehaviour
{    
    public delegate void SendFailMessage(string message);
    public static event SendFailMessage onFailMessageSent;
    
    public bool isActionOnProgress;

    public void SendFailEvent(string message)
    {
        if (onFailMessageSent != null)
            onFailMessageSent(message);
    }

    public abstract bool TryToActivateAction();
    protected abstract bool CanActivateAction();
    protected abstract void ActivateAction();
}
