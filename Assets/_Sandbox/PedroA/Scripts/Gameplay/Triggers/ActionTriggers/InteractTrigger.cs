using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : ActionTrigger
{
    public delegate void SetInteractionButtonUI(bool status);
    public static event SetInteractionButtonUI onSetInteractionButton;

    private bool isInInteractionArea;

    private void TryToInteract()
    {
        if (isInInteractionArea)
            StartCoroutine(base.CheckAction(reference));
    }

    private void OnTriggerEnter(Collider other)
    {
        isInInteractionArea = true;

        if (onSetInteractionButton != null)
            onSetInteractionButton(isInInteractionArea);
    }

    private void OnTriggerExit(Collider other)
    {
        isInInteractionArea = false;
        
        if (onSetInteractionButton != null)
            onSetInteractionButton(isInInteractionArea);  
    }

    private void OnEnable()
    {
        PlayerInteractController.onInteractInput += TryToInteract;
    }

    private void OnDisable()
    {
        PlayerInteractController.onInteractInput -= TryToInteract;
    }
}
