using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerAction : MonoBehaviour
{
    public bool isActionOnProgress;

    public abstract void TryToActivateAction();
    protected abstract bool CanActivateAction();
    protected abstract void ActivateAction();
}
