using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class TriggerAction : MonoBehaviour
{    
    public delegate void SendFailMessage(TextAsset message);
    public static event SendFailMessage onFailMessageSent;
    
    [HideInInspector] public bool isActionOnProgress;
    public int minimumStarsRequired;
    public bool willReduceStars;
    [SerializeField] private TextAsset failMessageinkJSON;
    [SerializeField] private bool willPreventInput;

    public void SendFailEvent(TextAsset message)
    {
        if (onFailMessageSent != null)
        {
            if (willPreventInput)
                GameManager.canInput = false;
            
            onFailMessageSent(message);
        }
    }

    public bool TryToActivateAction()
    {
        if (CanActivateAction())
        {
            ActivateAction();
        }
        else
            SendFailEvent(failMessageinkJSON);

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
