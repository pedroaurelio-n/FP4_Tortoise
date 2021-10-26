using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TA_Rotate : TriggerAction
{
    //[SerializeField] private GameObject affectedObject;
    [SerializeField] private Vector3 newRotationFromOrigin;
    [SerializeField] private float actionDuration;
    //[SerializeField] private Transform _Dynamic;
    
    private void Start()
    {
        //transform.parent = _Dynamic;
    }

    public override void TryToActivateAction()
    {
        if (CanActivateAction())
            ActivateAction();
        else
            Debug.Log("Couldn't perform action");
    }

    protected override bool CanActivateAction()
    {
        return true;
    }

    protected override void ActivateAction()
    {
        isActionOnProgress = true;
        transform.parent.DORotate(transform.eulerAngles + newRotationFromOrigin, actionDuration).OnComplete(delegate {isActionOnProgress = false;});
    }
}
