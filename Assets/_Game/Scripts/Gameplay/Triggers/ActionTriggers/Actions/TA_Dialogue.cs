using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TA_Dialogue : TriggerAction
{
    public delegate void TriggerDialogue(TextAsset json);
    public static event TriggerDialogue onDialogue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private bool willPreventInput;

    //============VISUAL REFERENCES============
    //[Header("Visual Cues")]

    //private bool _isPlayerInRange;

    protected override void ActivateAction()
    {
        if (willPreventInput)
            GameManager.canInput = false;
            
        onDialogue?.Invoke(inkJSON);
    }

    // private void Start()
    // {
    //     _isPlayerInRange = false;

    //     //============DISABLE VISUAL LOGIC============
    // }

    // private void TryToInteract()
    // {
    //     if (_isPlayerInRange)
    //     {
    //         if (onDialogue != null)
    //         {
    //             onDialogue(inkJSON);

    //             //============ACTIVATE DIALOGUE EFFECTS/EVENTS============
    //         }
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     //============CHECK FOR PLAYER COMPONENT============
    //     if (other.TryGetComponent(out CharacterController player))
    //     {
    //         _isPlayerInRange = true;

    //         //============ENABLE VISUAL LOGIC============
    //     }
    // }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     //============CHECK FOR PLAYER COMPONENT============
    //     if (other.TryGetComponent(out CharacterController player))
    //     {
    //         _isPlayerInRange = true;

    //         //============ENABLE VISUAL LOGIC============
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     //============CHECK FOR PLAYER COMPONENT============
    //     if (other.TryGetComponent(out CharacterController player))
    //     {
    //         _isPlayerInRange = false;

    //         //============DISABLE VISUAL LOGIC============
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     //============CHECK FOR PLAYER COMPONENT============
    //     if (other.TryGetComponent(out CharacterController player))
    //     {
    //         _isPlayerInRange = false;

    //         //============DISABLE VISUAL LOGIC============
    //     }
    // }

    // //============SUBSCRIBE TO INTERACTION EVENTS============
    // private void OnEnable()
    // {
    //     PlayerInteractController.onInteractInput += TryToInteract;
    // }

    // private void OnDisable()
    // {
    //     PlayerInteractController.onInteractInput -= TryToInteract;
    // }
}
