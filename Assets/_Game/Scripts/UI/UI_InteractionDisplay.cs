using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InteractionDisplay : MonoBehaviour
{
    [SerializeField] private GameObject interactionDisplay;

    private void SetInteractionDisplay(bool status)
    {
        interactionDisplay.SetActive(status);
    }

    private void OnEnable()
    {
        InteractTrigger.onSetInteractionButton += SetInteractionDisplay;
    }

    private void OnDisable()
    {
        InteractTrigger.onSetInteractionButton -= SetInteractionDisplay;
    }
}
