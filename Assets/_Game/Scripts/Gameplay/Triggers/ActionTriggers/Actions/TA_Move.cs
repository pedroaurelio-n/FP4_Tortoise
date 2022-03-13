using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TA_Move : TriggerAction
{
    [SerializeField] private Vector3 newPositionFromOrigin;
    [SerializeField] private float actionDuration;
    [SerializeField] private Ease ease;

    protected override void ActivateAction()
    {
        isActionOnProgress = true;
        transform.parent.DOMove(transform.position+newPositionFromOrigin, actionDuration).SetEase(ease).OnComplete(delegate {isActionOnProgress = false;});
    }
}
