using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TA_Rotate : TriggerAction
{
    [SerializeField] private Vector3 newRotationFromOrigin;
    [SerializeField] private float actionDuration;
    [SerializeField] private Ease ease;

    protected override void ActivateAction()
    {
        isActionOnProgress = true;
        transform.parent.DORotate(newRotationFromOrigin, actionDuration, RotateMode.WorldAxisAdd).SetEase(ease).OnComplete(delegate {isActionOnProgress = false;});
    }
}
