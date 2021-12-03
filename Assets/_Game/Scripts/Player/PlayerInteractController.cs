using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    public delegate void InteractInput();
    public static event InteractInput onInteractInput;

    public void SendInteractionEvent()
    {
        if (onInteractInput != null)
            onInteractInput();
    }
}
