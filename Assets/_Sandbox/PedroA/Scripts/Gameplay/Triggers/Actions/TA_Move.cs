using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TA_Move : TriggerAction
{
    //[SerializeField] private GameObject affectedObject;
    [SerializeField] private Vector3 newPositionFromOrigin;
    [SerializeField] private float actionDuration;
    //[SerializeField] private Transform _Dynamic;

    private void Start()
    {
        //transform.parent = _Dynamic;
    }
    
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
        isActionOnProgress = true;
        transform.parent.DOMove(transform.position+newPositionFromOrigin, actionDuration).OnComplete(delegate {isActionOnProgress = false;});
    }
}
