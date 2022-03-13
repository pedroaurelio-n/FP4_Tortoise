using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class TriggerAction : MonoBehaviour
{    
    public delegate void SendFailMessage(string message);
    public static event SendFailMessage onFailMessageSent;
    
    [HideInInspector] public bool isActionOnProgress;
    public int minimumStarsRequired;
    public bool willReduceStars;
    [SerializeField] private string failMessage;

    public void SendFailEvent(string message)
    {
        if (onFailMessageSent != null)
            onFailMessageSent(message);
    }

    public bool TryToActivateAction()
    {
        if (CanActivateAction())
        {
            ActivateAction();
        }
        else
            SendFailEvent(failMessage);

        return CanActivateAction();
    }

    protected bool CanActivateAction()
    {
        if (minimumStarsRequired > 0)
            return StarManager.GetStarCount() >= minimumStarsRequired;
        
        else
            return true;
    }

    protected abstract void ActivateAction();
}
