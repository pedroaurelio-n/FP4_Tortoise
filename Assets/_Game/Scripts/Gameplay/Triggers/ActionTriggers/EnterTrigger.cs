using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTrigger : ActionTrigger
{
    private bool _isInArea;

    private bool CanProceedText()
    {
        return reference.actionList[0] is TA_Dialogue && _isInArea;
    }

    private void DialogueInteract()
    {
        if (CanProceedText())
            StartCoroutine(base.CheckAction(reference));
    }

    private void OnTriggerEnter(Collider other)
    {
        _isInArea = true;
        if (other.TryGetComponent(out PlayerInteractController player))
        {
            StartCoroutine(base.CheckAction(reference));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _isInArea = true;
    }

    private void OnEnable()
    {
        PlayerInteractController.onInteractInput += DialogueInteract;
    }

    private void OnDisable()
    {
        PlayerInteractController.onInteractInput -= DialogueInteract;
    }
}