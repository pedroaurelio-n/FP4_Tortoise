using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    public delegate void SendTextMessage(string message);
    public static event SendTextMessage onMessageSent;

    [SerializeField] private string message;
    [SerializeField] private bool willActivateEverytime;

    private bool canActivate;

    private void Start()
    {
        canActivate = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractController player))
        {
            if (canActivate)
            {
                if (onMessageSent != null)
                    onMessageSent(message);
            }

            if (!willActivateEverytime)
                canActivate = false;
        }
    }
}
