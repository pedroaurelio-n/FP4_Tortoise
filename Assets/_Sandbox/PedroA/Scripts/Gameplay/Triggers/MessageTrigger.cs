using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    public delegate void SendTextMessage(string message);
    public static event SendTextMessage onMessageSent;

    [SerializeField] private string message;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractController player))
        {
            Debug.Log("trigger");
            if (onMessageSent != null)
                onMessageSent(message);
        }
    }
}
