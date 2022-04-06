using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public delegate void TriggerDialogue(TextAsset json);
    public static event TriggerDialogue onDialogue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    //============VISUAL REFERENCES============
    [Header("Visual Cues")]
    [SerializeField] private Material interactFalse;
    [SerializeField] private Material interactTrue;
    [SerializeField] private Material interactOngoing;
    [SerializeField] private MeshRenderer npcBody;

    private bool _isPlayerInRange;

    private void Start()
    {
        _isPlayerInRange = false;

        //============DISABLE VISUAL LOGIC============
        npcBody.material = interactFalse;
    }

    private void TryToInteract()
    {
        if (_isPlayerInRange)
        {
            if (onDialogue != null)
            {
                onDialogue(inkJSON);

                //============ACTIVATE DIALOGUE EFFECTS/EVENTS============
                npcBody.material = interactOngoing;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //============CHECK FOR PLAYER COMPONENT============
        if (other.TryGetComponent(out CharacterController player))
        {
            _isPlayerInRange = true;

            //============ENABLE VISUAL LOGIC============
            npcBody.material = interactTrue;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //============CHECK FOR PLAYER COMPONENT============
        if (other.TryGetComponent(out CharacterController player))
        {
            _isPlayerInRange = true;

            //============ENABLE VISUAL LOGIC============
            npcBody.material = interactTrue;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //============CHECK FOR PLAYER COMPONENT============
        if (other.TryGetComponent(out CharacterController player))
        {
            _isPlayerInRange = false;

            //============DISABLE VISUAL LOGIC============
            npcBody.material = interactFalse;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //============CHECK FOR PLAYER COMPONENT============
        if (other.TryGetComponent(out CharacterController player))
        {
            _isPlayerInRange = false;

            //============DISABLE VISUAL LOGIC============
            npcBody.material = interactFalse;
        }
    }

    //============SUBSCRIBE TO INTERACTION EVENTS============
    private void OnEnable()
    {
        PlayerInteractController.onInteractInput += TryToInteract;
    }

    private void OnDisable()
    {
        PlayerInteractController.onInteractInput -= TryToInteract;
    }
}
